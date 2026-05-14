import { useNavigate } from 'react-router-dom';
import type { BookingListDto } from '../types';
import { BookingStatus, BookingStatusLabels, SeatClassLabels } from '../types';

interface Props {
  booking: BookingListDto;
}

export default function BookingCard({ booking }: Props) {
  const navigate = useNavigate();

  const statusColor =
    booking.bookingStatus === BookingStatus.Confirmed
      ? 'success'
      : booking.bookingStatus === BookingStatus.Cancelled
      ? 'danger'
      : booking.bookingStatus === BookingStatus.CheckedIn
      ? 'info'
      : 'secondary';

  return (
    <div className="card shadow-sm border-0 mb-3 booking-card">
      <div className="card-body p-3">
        <div className="row align-items-center">
          <div className="col-md-2">
            <div className="text-muted small">PNR</div>
            <div className="fw-bold fs-5 font-monospace text-primary">{booking.pnrNumber}</div>
            <span className={`badge bg-${statusColor}`}>{BookingStatusLabels[booking.bookingStatus]}</span>
          </div>
          <div className="col-md-4">
            <div className="text-muted small">Flight</div>
            <div className="fw-semibold">{booking.flightNumber}</div>
            <div className="d-flex align-items-center mt-1">
              <span className="fw-bold">{booking.departureCity}</span>
              <span className="mx-2 text-muted">→</span>
              <span className="fw-bold">{booking.arrivalCity}</span>
            </div>
          </div>
          <div className="col-md-2">
            <div className="text-muted small">Departure</div>
            <div className="fw-semibold">
              {new Date(booking.departureDateTime).toLocaleDateString('en-GB', {
                day: '2-digit', month: 'short', year: 'numeric',
              })}
            </div>
            <div className="text-muted small">
              {new Date(booking.departureDateTime).toLocaleTimeString('en-GB', {
                hour: '2-digit', minute: '2-digit',
              })}
            </div>
          </div>
          <div className="col-md-2">
            <div className="text-muted small">Seat</div>
            <div className="fw-semibold">{booking.seatNumber}</div>
            <span className="badge bg-light text-dark border">{SeatClassLabels[booking.seatClass]}</span>
          </div>
          <div className="col-md-1 text-center">
            <div className="text-muted small">Price</div>
            <div className="fw-bold text-success">{booking.totalPrice.toLocaleString()}</div>
          </div>
          <div className="col-md-1 text-end">
            <button
              className="btn btn-outline-primary btn-sm"
              onClick={() => navigate(`/bookings/${booking.id}`)}
            >
              View
            </button>
          </div>
        </div>
      </div>
    </div>
  );
}
