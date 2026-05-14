import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { SeatClass } from '../types';

export default function HomePage() {
  const navigate = useNavigate();
  const tomorrow = new Date();
  tomorrow.setDate(tomorrow.getDate() + 1);

  const [form, setForm] = useState({
    departureIata: '',
    arrivalIata: '',
    departureDate: tomorrow.toISOString().split('T')[0],
    passengers: 1,
    seatClass: SeatClass.Economy,
  });

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    const params = new URLSearchParams({
      from: form.departureIata.toUpperCase(),
      to: form.arrivalIata.toUpperCase(),
      date: form.departureDate,
      pax: String(form.passengers),
      class: String(form.seatClass),
    });
    navigate(`/flights/search?${params}`);
  };

  return (
    <>
      <div
        className="rounded-3 text-white py-5 px-4 mb-5"
        style={{ background: 'linear-gradient(135deg, #1a1a2e 0%, #16213e 50%, #0f3460 100%)' }}
      >
        <div className="row align-items-center">
          <div className="col-lg-6">
            <h1 className="display-4 fw-bold mb-3">✈ Fly Anywhere</h1>
            <p className="lead opacity-75 mb-4">
              Search and book flights to hundreds of destinations worldwide.
              Best prices, seamless experience.
            </p>
          </div>
          <div className="col-lg-6 text-center d-none d-lg-flex justify-content-center align-items-center">
            <span style={{ fontSize: '8rem', opacity: 0.3 }}>✈</span>
          </div>
        </div>
      </div>

      <div className="bg-light rounded-3 p-4 mb-5 shadow-sm">
        <h4 className="fw-bold mb-3">Quick Search</h4>
        <form onSubmit={handleSubmit}>
          <div className="row g-3 align-items-end">
            <div className="col-md-2">
              <label className="form-label fw-semibold">From (IATA)</label>
              <input
                className="form-control text-uppercase"
                placeholder="IST"
                maxLength={3}
                value={form.departureIata}
                onChange={e => setForm({ ...form, departureIata: e.target.value.toUpperCase() })}
                required
              />
            </div>
            <div className="col-md-2">
              <label className="form-label fw-semibold">To (IATA)</label>
              <input
                className="form-control text-uppercase"
                placeholder="LHR"
                maxLength={3}
                value={form.arrivalIata}
                onChange={e => setForm({ ...form, arrivalIata: e.target.value.toUpperCase() })}
                required
              />
            </div>
            <div className="col-md-3">
              <label className="form-label fw-semibold">Date</label>
              <input
                type="date"
                className="form-control"
                value={form.departureDate}
                min={new Date().toISOString().split('T')[0]}
                onChange={e => setForm({ ...form, departureDate: e.target.value })}
                required
              />
            </div>
            <div className="col-md-2">
              <label className="form-label fw-semibold">Passengers</label>
              <select
                className="form-select"
                value={form.passengers}
                onChange={e => setForm({ ...form, passengers: Number(e.target.value) })}
              >
                {[1,2,3,4,5,6,7,8,9].map(n => <option key={n} value={n}>{n}</option>)}
              </select>
            </div>
            <div className="col-md-2">
              <label className="form-label fw-semibold">Class</label>
              <select
                className="form-select"
                value={form.seatClass}
                onChange={e => setForm({ ...form, seatClass: Number(e.target.value) })}
              >
                <option value={SeatClass.Economy}>Economy</option>
                <option value={SeatClass.PremiumEconomy}>Premium Economy</option>
                <option value={SeatClass.Business}>Business</option>
                <option value={SeatClass.FirstClass}>First Class</option>
              </select>
            </div>
            <div className="col-md-1">
              <button type="submit" className="btn btn-primary w-100">🔍</button>
            </div>
          </div>
        </form>
      </div>

      <div className="row g-4">
        {[
          { icon: '🛡️', color: 'success', title: 'Secure Booking', desc: 'End-to-end encrypted bookings with instant PNR generation.' },
          { icon: '🔔', color: 'warning', title: 'Flight Reminders', desc: 'Automatic alerts 7 days and 24 hours before departure via email, SMS or WhatsApp.' },
          { icon: '🌍', color: 'info', title: 'Global Coverage', desc: 'Hundreds of routes worldwide with competitive pricing and multiple cabin classes.' },
        ].map(f => (
          <div key={f.title} className="col-md-4">
            <div className="card h-100 border-0 shadow-sm text-center p-4">
              <div style={{ fontSize: '3rem' }}>{f.icon}</div>
              <h5 className="mt-2 fw-bold">{f.title}</h5>
              <p className="text-muted mb-0">{f.desc}</p>
            </div>
          </div>
        ))}
      </div>
    </>
  );
}
