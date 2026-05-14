import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { flightApi } from '../api/flightApi';
import type { FlightListDto } from '../types';
import { SeatClass } from '../types';
import AirportSelect from '../components/AirportSelect';
import FlightCard from '../components/FlightCard';
import LoadingSpinner from '../components/LoadingSpinner';

export default function HomePage() {
  const navigate = useNavigate();
  const tomorrow = new Date();
  tomorrow.setDate(tomorrow.getDate() + 1);

  const [form, setForm] = useState({
    departureIata: 'IST',
    arrivalIata: '',
    departureDate: tomorrow.toISOString().split('T')[0],
    passengers: 1,
    seatClass: SeatClass.Economy,
  });

  const [flights, setFlights] = useState<FlightListDto[]>([]);
  const [loadingFlights, setLoadingFlights] = useState(true);

  useEffect(() => {
    flightApi.getAll()
      .then(res => {
        if (res.data.success && res.data.data) {
          const now = new Date();
          const upcoming = res.data.data
            .filter(f => new Date(f.departureDateTime) > now)
            .sort((a, b) => new Date(a.departureDateTime).getTime() - new Date(b.departureDateTime).getTime())
            .slice(0, 8);
          setFlights(upcoming);
        }
      })
      .catch(() => {})
      .finally(() => setLoadingFlights(false));
  }, []);

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (!form.departureIata || !form.arrivalIata) return;
    const params = new URLSearchParams({
      from: form.departureIata,
      to:   form.arrivalIata,
      date: form.departureDate,
      pax:  String(form.passengers),
      class: String(form.seatClass),
    });
    navigate(`/flights/search?${params}`);
  };

  return (
    <>
      {/* Hero */}
      <div
        className="rounded-4 text-white py-5 px-4 mb-4"
        style={{ background: 'linear-gradient(135deg, #0f3460 0%, #16213e 60%, #1a1a2e 100%)' }}
      >
        <div className="row align-items-center">
          <div className="col-lg-7">
            <h1 className="display-5 fw-bold mb-3">✈ Fly Anywhere</h1>
            <p className="lead opacity-75 mb-0">
              Search and book flights to hundreds of destinations worldwide with the best prices.
            </p>
          </div>
        </div>
      </div>

      {/* Quick Search */}
      <div className="card border-0 shadow mb-4">
        <div className="card-body p-4">
          <h5 className="fw-bold mb-3">🔍 Quick Search</h5>
          <form onSubmit={handleSubmit}>
            <div className="row g-3 align-items-end">
              <div className="col-12 col-sm-6 col-lg-3">
                <AirportSelect
                  label="From"
                  value={form.departureIata}
                  onChange={v => setForm(f => ({ ...f, departureIata: v }))}
                  exclude={form.arrivalIata}
                  required
                />
              </div>
              <div className="col-12 col-sm-6 col-lg-3">
                <AirportSelect
                  label="To"
                  value={form.arrivalIata}
                  onChange={v => setForm(f => ({ ...f, arrivalIata: v }))}
                  exclude={form.departureIata}
                  required
                />
              </div>
              <div className="col-6 col-sm-4 col-lg-2">
                <label className="form-label fw-semibold">Date</label>
                <input
                  type="date"
                  className="form-control"
                  value={form.departureDate}
                  min={new Date().toISOString().split('T')[0]}
                  onChange={e => setForm(f => ({ ...f, departureDate: e.target.value }))}
                  required
                />
              </div>
              <div className="col-3 col-sm-4 col-lg-1">
                <label className="form-label fw-semibold">Pax</label>
                <select
                  className="form-select"
                  value={form.passengers}
                  onChange={e => setForm(f => ({ ...f, passengers: Number(e.target.value) }))}
                >
                  {[1,2,3,4,5,6,7,8,9].map(n => <option key={n} value={n}>{n}</option>)}
                </select>
              </div>
              <div className="col-9 col-sm-6 col-lg-2">
                <label className="form-label fw-semibold">Class</label>
                <select
                  className="form-select"
                  value={form.seatClass}
                  onChange={e => setForm(f => ({ ...f, seatClass: Number(e.target.value) as SeatClass }))}
                >
                  <option value={SeatClass.Economy}>Economy</option>
                  <option value={SeatClass.PremiumEconomy}>Premium Economy</option>
                  <option value={SeatClass.Business}>Business</option>
                  <option value={SeatClass.FirstClass}>First Class</option>
                </select>
              </div>
              <div className="col-12 col-lg-1">
                <button type="submit" className="btn btn-primary w-100 fw-semibold">
                  Search
                </button>
              </div>
            </div>
          </form>
        </div>
      </div>

      {/* Upcoming flights */}
      <div className="mb-5">
        <div className="d-flex justify-content-between align-items-center mb-3">
          <h5 className="fw-bold mb-0">🛫 Upcoming Flights</h5>
          <button
            className="btn btn-outline-primary btn-sm"
            onClick={() => navigate('/flights/search')}
          >
            View all →
          </button>
        </div>

        {loadingFlights && <LoadingSpinner text="Loading flights..." />}

        {!loadingFlights && flights.length === 0 && (
          <div className="text-center py-4 text-muted">
            <div style={{ fontSize: '3rem', opacity: 0.3 }}>✈</div>
            <p className="mt-2">No upcoming flights available at the moment.</p>
          </div>
        )}

        {!loadingFlights && flights.map(f => <FlightCard key={f.id} flight={f} />)}
      </div>

      {/* Feature cards */}
      <div className="row g-4 mb-5">
        {[
          { icon: '🛡️', title: 'Secure Booking', desc: 'End-to-end encrypted bookings with instant PNR generation.' },
          { icon: '🔔', title: 'Flight Reminders', desc: 'Automatic alerts 7 days and 24 hours before departure.' },
          { icon: '🌍', title: 'Global Coverage', desc: 'Hundreds of routes with competitive pricing and multiple classes.' },
        ].map(f => (
          <div key={f.title} className="col-md-4">
            <div className="card h-100 border-0 shadow-sm text-center p-4 rounded-4">
              <div style={{ fontSize: '2.5rem' }}>{f.icon}</div>
              <h6 className="mt-2 fw-bold">{f.title}</h6>
              <p className="text-muted mb-0 small">{f.desc}</p>
            </div>
          </div>
        ))}
      </div>
    </>
  );
}
