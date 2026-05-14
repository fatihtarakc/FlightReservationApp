import { useNavigate } from 'react-router-dom';
import type { FlightListDto } from '../types';
import { FlightStatusLabels, FlightStatus } from '../types';
import { airportLabel } from '../data/airports';

interface Props {
  flight: FlightListDto;
}

function formatDuration(dep: string, arr: string) {
  const diff = new Date(arr).getTime() - new Date(dep).getTime();
  const h = Math.floor(diff / 3600000);
  const m = Math.floor((diff % 3600000) / 60000);
  return `${h}h ${m}m`;
}

function formatTime(dt: string) {
  return new Date(dt).toLocaleTimeString('tr-TR', { hour: '2-digit', minute: '2-digit' });
}

function formatDate(dt: string) {
  return new Date(dt).toLocaleDateString('tr-TR', { day: '2-digit', month: 'short' });
}

export default function FlightCard({ flight }: Props) {
  const navigate = useNavigate();
  const statusColor =
    flight.flightStatus === FlightStatus.Cancelled ? 'danger'
    : flight.flightStatus === FlightStatus.Delayed ? 'warning'
    : 'success';

  return (
    <div className="card shadow-sm border-0 mb-3 flight-card">
      <div className="card-body p-3">
        <div className="row align-items-center g-2">

          {/* Airline */}
          <div className="col-12 col-md-3">
            <div className="fw-bold text-primary">{flight.airlineName}</div>
            <div className="text-muted small">Flight {flight.number}</div>
            <span className={`badge bg-${statusColor} mt-1`}>
              {FlightStatusLabels[flight.flightStatus]}
            </span>
          </div>

          {/* Route timeline */}
          <div className="col-12 col-md-5">
            <div className="d-flex align-items-center justify-content-between">
              <div className="text-center">
                <div className="fw-bold fs-5">{formatTime(flight.departureDateTime)}</div>
                <div className="fw-semibold text-dark">{airportLabel(flight.departureAirportIata)}</div>
                <div className="text-muted small">{formatDate(flight.departureDateTime)}</div>
              </div>
              <div className="text-center flex-grow-1 px-2">
                <div className="text-muted small">{formatDuration(flight.departureDateTime, flight.arrivalDateTime)}</div>
                <div className="d-flex align-items-center">
                  <div className="border-top flex-grow-1 border-2"></div>
                  <span className="mx-2 text-primary fs-6">✈</span>
                  <div className="border-top flex-grow-1 border-2"></div>
                </div>
                <div className="text-muted small">Direct</div>
              </div>
              <div className="text-center">
                <div className="fw-bold fs-5">{formatTime(flight.arrivalDateTime)}</div>
                <div className="fw-semibold text-dark">{airportLabel(flight.arrivalAirportIata)}</div>
                <div className="text-muted small">{formatDate(flight.arrivalDateTime)}</div>
              </div>
            </div>
          </div>

          {/* Price */}
          <div className="col-6 col-md-2 text-center">
            <div className="fw-bold fs-4 text-success">
              {flight.baseEconomyPrice.toLocaleString()}
            </div>
            <div className="text-muted small">{flight.currency} / person</div>
            {flight.availableSeats <= 5 && flight.availableSeats > 0 && (
              <span className="badge bg-warning text-dark mt-1">Only {flight.availableSeats} left!</span>
            )}
          </div>

          {/* Actions */}
          <div className="col-6 col-md-2 text-end">
            <button
              className="btn btn-primary btn-sm w-100"
              onClick={() => navigate(`/flights/${flight.id}/book`)}
            >
              Select →
            </button>
            <button
              className="btn btn-link btn-sm text-muted p-0 mt-1 d-block w-100"
              onClick={() => navigate(`/flights/${flight.id}`)}
            >
              Details
            </button>
          </div>

        </div>
      </div>
    </div>
  );
}
