import { useState, useEffect } from 'react';
import { useSearchParams } from 'react-router-dom';
import { flightApi } from '../api/flightApi';
import type { FlightListDto } from '../types';
import { SeatClass } from '../types';
import FlightCard from '../components/FlightCard';
import LoadingSpinner from '../components/LoadingSpinner';
import AlertMessage from '../components/AlertMessage';

export default function FlightSearchPage() {
  const [searchParams, setSearchParams] = useSearchParams();

  const tomorrow = new Date();
  tomorrow.setDate(tomorrow.getDate() + 1);

  const [form, setForm] = useState({
    departureIata: searchParams.get('from') ?? '',
    arrivalIata: searchParams.get('to') ?? '',
    departureDate: searchParams.get('date') ?? tomorrow.toISOString().split('T')[0],
    passengers: Number(searchParams.get('pax') ?? 1),
    seatClass: Number(searchParams.get('class') ?? SeatClass.Economy) as SeatClass,
  });

  const [results, setResults] = useState<FlightListDto[]>([]);
  const [searched, setSearched] = useState(false);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  const doSearch = async () => {
    setError('');
    setLoading(true);
    setSearched(true);
    try {
      const res = await flightApi.search({
        departureIata: form.departureIata.toUpperCase(),
        arrivalIata: form.arrivalIata.toUpperCase(),
        departureDate: form.departureDate,
        passengers: form.passengers,
        seatClass: form.seatClass,
      });
      if (res.data.success && res.data.data) {
        setResults(res.data.data);
      } else {
        setResults([]);
        setError(res.data.message ?? 'No flights found.');
      }
    } catch {
      setError('Failed to search flights. Please try again.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    if (searchParams.get('from')) {
      doSearch();
    }
  }, []);

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    setSearchParams({
      from: form.departureIata, to: form.arrivalIata,
      date: form.departureDate, pax: String(form.passengers), class: String(form.seatClass),
    });
    doSearch();
  };

  return (
    <>
      <h2 className="fw-bold mb-4">🔍 Search Flights</h2>

      <div className="card shadow-sm border-0 mb-4">
        <div className="card-body p-4">
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
                  onChange={e => setForm({ ...form, seatClass: Number(e.target.value) as SeatClass })}
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
      </div>

      {loading && <LoadingSpinner text="Searching flights..." />}
      {error && !loading && <AlertMessage type="warning" message={error} onClose={() => setError('')} />}

      {!loading && searched && results.length > 0 && (
        <>
          <p className="text-muted mb-3">{results.length} flight(s) found</p>
          {results.map(f => <FlightCard key={f.id} flight={f} />)}
        </>
      )}

      {!loading && searched && results.length === 0 && !error && (
        <div className="text-center py-5">
          <div style={{ fontSize: '5rem', opacity: 0.3 }}>✈</div>
          <h4 className="text-muted">No flights found for this route and date.</h4>
        </div>
      )}
    </>
  );
}
