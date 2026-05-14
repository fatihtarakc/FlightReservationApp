import { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { flightApi } from '../api/flightApi';
import type { FlightDto } from '../types';
import { FlightStatusLabels } from '../types';
import LoadingSpinner from '../components/LoadingSpinner';

export default function FlightDetailsPage() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [flight, setFlight] = useState<FlightDto | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    if (!id) return;
    flightApi.getById(id).then(res => {
      if (res.data.success && res.data.data) setFlight(res.data.data);
      else navigate('/flights/search');
    }).finally(() => setLoading(false));
  }, [id]);

  if (loading) return <LoadingSpinner />;
  if (!flight) return null;

  const dep = new Date(flight.departureDateTime);
  const arr = new Date(flight.arrivalDateTime);
  const durMs = arr.getTime() - dep.getTime();
  const durH = Math.floor(durMs / 3600000);
  const durM = Math.floor((durMs % 3600000) / 60000);

  return (
    <>
      <button className="btn btn-link text-muted p-0 mb-3" onClick={() => navigate(-1)}>
        ← Back to Search
      </button>

      <div className="card shadow border-0">
        <div className="card-header" style={{ background: '#0d6efd', color: 'white' }}>
          <div className="d-flex justify-content-between align-items-center p-1">
            <div>
              <h4 className="mb-0 fw-bold">{flight.airlineName} — Flight {flight.number}</h4>
              <small className="opacity-75">{flight.modelName} · {flight.aircraftTailNumber}</small>
            </div>
            <span className="badge bg-light text-dark fs-6">
              {FlightStatusLabels[flight.flightStatus]}
            </span>
          </div>
        </div>
        <div className="card-body p-4">
          <div className="row align-items-center mb-4">
            <div className="col-md-4 text-center">
              <div className="display-6 fw-bold">{dep.toLocaleTimeString('en-GB', { hour: '2-digit', minute: '2-digit' })}</div>
              <div className="fs-5 text-primary fw-semibold">{flight.departureAirportIata}</div>
              <div className="text-muted">{flight.departureCity}</div>
              <div className="text-muted small">{dep.toLocaleDateString('en-GB', { day: '2-digit', month: 'short', year: 'numeric' })}</div>
              {flight.terminal && <span className="badge bg-secondary mt-1 me-1">Terminal {flight.terminal}</span>}
              {flight.gate && <span className="badge bg-info mt-1">Gate {flight.gate}</span>}
            </div>
            <div className="col-md-4 text-center">
              <div className="text-muted small mb-1">{durH}h {durM}m</div>
              <div className="d-flex align-items-center">
                <div className="border-top flex-grow-1"></div>
                <span className="mx-3 text-primary fs-4">✈</span>
                <div className="border-top flex-grow-1"></div>
              </div>
              <div className="text-muted small mt-1">Direct Flight</div>
            </div>
            <div className="col-md-4 text-center">
              <div className="display-6 fw-bold">{arr.toLocaleTimeString('en-GB', { hour: '2-digit', minute: '2-digit' })}</div>
              <div className="fs-5 text-primary fw-semibold">{flight.arrivalAirportIata}</div>
              <div className="text-muted">{flight.arrivalCity}</div>
              <div className="text-muted small">{arr.toLocaleDateString('en-GB', { day: '2-digit', month: 'short', year: 'numeric' })}</div>
            </div>
          </div>

          <hr />
          <h5 className="fw-semibold mb-3">Pricing &amp; Availability</h5>
          <div className="row g-3">
            {[
              { label: 'Economy', price: flight.baseEconomyPrice, seats: flight.availableEconomySeats },
              { label: 'Premium Economy', price: flight.basePremiumEconomyPrice, seats: null },
              { label: 'Business', price: flight.baseBusinessPrice, seats: flight.availableBusinessSeats },
              { label: 'First Class', price: flight.baseFirstClassPrice, seats: flight.availableFirstClassSeats },
            ].map(c => (
              <div key={c.label} className="col-md-3">
                <div className="card border text-center p-3 h-100">
                  <div className="fw-semibold text-muted mb-1">{c.label}</div>
                  <div className="fs-4 fw-bold text-success">{c.price.toLocaleString()}</div>
                  <div className="text-muted small">{flight.currency}</div>
                  {c.seats != null && <span className="badge bg-light text-dark mt-1">{c.seats} seats</span>}
                </div>
              </div>
            ))}
          </div>

          <div className="text-center mt-4">
            <button
              className="btn btn-primary btn-lg px-5"
              onClick={() => navigate(`/flights/${flight.id}/book`)}
            >
              🎫 Book This Flight
            </button>
          </div>
        </div>
      </div>
    </>
  );
}
