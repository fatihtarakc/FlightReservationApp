import { useNavigate } from 'react-router-dom';
import type { FlightListDto } from '../types';
import { FlightStatusLabels, FlightStatus } from '../types';

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
  return new Date(dt).toLocaleTimeString('en-GB', { hour: '2-digit', minute: '2-digit' });
}

export default function FlightCard({ flight }: Props) {
  const navigate = useNavigate();
  const statusColor =
    flight.flightStatus === FlightStatus.Cancelled
      ? 'danger'
      : flight.flightStatus === FlightStatus.Delayed
      ? 'warning'
      : 'success';

  return (
    <div className="card shadow-sm border-0 mb-3 flight-card">
      <div className="card-body p-3">
        <div className="row align-items-center">
          <div className="col-md-3">
            <div className="fw-bold text-primary fs-5">{flight.airlineName}</div>
            <div className="text-muted small">Flight {flight.number}</div>
            <span className={`badge bg-${statusColor} mt-1`}>
              {FlightStatusLabels[flight.flightStatus]}
            </span>
          </div>
          <div className="col-md-5">
            <div className="d-flex align-items-center justify-content-between">
              <div className="text-center">
                <div className="fw-bold fs-5">{formatTime(flight.departureDateTime)}</div>
                <div className="text-primary fw-semibold small">{flight.departureAirportIata}</div>
                <div className="text-muted small">{flight.departureCity}</div>
              </div>
              <div className="text-center flex-grow-1 px-3">
                <div className="text-muted small">{formatDuration(flight.departureDateTime, flight.arrivalDateTime)}</div>
                <div className="d-flex align-items-center">
                  <div className="border-top flex-grow-1"></div>
                  <span className="mx-2 text-primary">✈</span>
                  <div className="border-top flex-grow-1"></div>
                </div>
                <div className="text-muted small">Direct</div>
              </div>
              <div className="text-center">
                <div className="fw-bold fs-5">{formatTime(flight.arrivalDateTime)}</div>
                <div className="text-primary fw-semibold small">{flight.arrivalAirportIata}</div>
                <div className="text-muted small">{flight.arrivalCity}</div>
              </div>
            </div>
          </div>
          <div className="col-md-2 text-center">
            <div className="fw-bold fs-4 text-success">
              {flight.baseEconomyPrice.toLocaleString()} {flight.currency}
            </div>
            <div className="text-muted small">from / person</div>
            {flight.availableSeats <= 5 && flight.availableSeats > 0 && (
              <span className="badge bg-warning text-dark small">Only {flight.availableSeats} left!</span>
            )}
          </div>
          <div className="col-md-2 text-end">
            <button
              className="btn btn-primary"
              onClick={() => navigate(`/flights/${flight.id}/book`)}
            >
              Select →
            </button>
            <div className="mt-1">
              <button
                className="btn btn-link btn-sm text-muted p-0"
                onClick={() => navigate(`/flights/${flight.id}`)}
              >
                Details
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
