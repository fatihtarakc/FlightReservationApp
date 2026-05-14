import { useState, useEffect } from 'react';
import { useSearchParams } from 'react-router-dom';
import { flightApi } from '../api/flightApi';
import type { FlightListDto } from '../types';
import { SeatClass } from '../types';
import FlightCard from '../components/FlightCard';
import AirportSelect from '../components/AirportSelect';
import LoadingSpinner from '../components/LoadingSpinner';
import AlertMessage from '../components/AlertMessage';

export default function FlightSearchPage() {
  const [searchParams, setSearchParams] = useSearchParams();

  const tomorrow = new Date();
  tomorrow.setDate(tomorrow.getDate() + 1);

  const [form, setForm] = useState({
    departureIata: searchParams.get('from') ?? '',
    arrivalIata:   searchParams.get('to')   ?? '',
    departureDate: searchParams.get('date') ?? tomorrow.toISOString().split('T')[0],
    passengers:    Number(searchParams.get('pax')   ?? 1),
    seatClass:     Number(searchParams.get('class') ?? SeatClass.Economy) as SeatClass,
  });

  const [results, setResults] = useState<FlightListDto[]>([]);
  const [searched, setSearched] = useState(false);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  const doSearch = async () => {
    if (!form.departureIata || !form.arrivalIata) return;
    setError('');
    setLoading(true);
    setSearched(true);
    try {
      const res = await flightApi.search({
        departureIata: form.departureIata,
        arrivalIata:   form.arrivalIata,
        departureDate: form.departureDate,
        passengers:    form.passengers,
        seatClass:     form.seatClass,
      });
      if (res.data.success && res.data.data) {
        setResults(res.data.data);
      } else {
        setResults([]);
        setError(res.data.message ?? 'No flights found for this route.');
      }
    } catch {
      setError('Failed to search flights. Please try again.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    if (searchParams.get('from') && searchParams.get('to')) {
      doSearch();
    }
  }, []);

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    setSearchParams({
      from:  form.departureIata,
      to:    form.arrivalIata,
      date:  form.departureDate,
      pax:   String(form.passengers),
      class: String(form.seatClass),
    });
    doSearch();
  };

  return (
    <>
      <h2 className="fw-bold mb-4">🔍 Search Flights</h2>

      <div className="card shadow-sm border-0 mb-4 rounded-4">
        <div className="card-body p-4">
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
                <button type="submit" className="btn btn-primary w-100 fw-semibold" disabled={loading}>
                  {loading ? <span className="spinner-border spinner-border-sm" /> : 'Search'}
                </button>
              </div>
            </div>
          </form>
        </div>
      </div>

      {loading && <LoadingSpinner text="Searching flights..." />}
      {error && !loading && <AlertMessage type="warning" message={error} onClose={() => setError('')} />}

      {!loading && searched && results.length > 0 && (
        <>
          <p className="text-muted mb-3 fw-semibold">{results.length} flight(s) found</p>
          {results.map(f => <FlightCard key={f.id} flight={f} />)}
        </>
      )}

      {!loading && searched && results.length === 0 && !error && (
        <div className="text-center py-5">
          <div style={{ fontSize: '4rem', opacity: 0.25 }}>✈</div>
          <h5 className="text-muted mt-3">No flights found for this route and date.</h5>
          <p className="text-muted small">Try different dates or destinations.</p>
        </div>
      )}
    </>
  );
}
