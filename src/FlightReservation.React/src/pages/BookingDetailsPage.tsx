import { useEffect, useState } from 'react';
import { useParams, useNavigate, useLocation } from 'react-router-dom';
import { bookingApi } from '../api/bookingApi';
import type { BookingDetails } from '../types';
import { BookingStatus, BookingStatusLabels, SeatClassLabels, CurrencyLabels } from '../types';
import LoadingSpinner from '../components/LoadingSpinner';
import AlertMessage from '../components/AlertMessage';

export default function BookingDetailsPage() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const location = useLocation();
  const successMsg = (location.state as { success?: string })?.success;

  const [booking, setBooking] = useState<BookingDetails | null>(null);
  const [loading, setLoading] = useState(true);
  const [cancelling, setCancelling] = useState(false);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState(successMsg ?? '');

  useEffect(() => {
    if (!id) return;
    bookingApi.getById(id)
      .then(res => { if (res.data.success && res.data.data) setBooking(res.data.data); })
      .finally(() => setLoading(false));
  }, [id]);

  const handleCancel = async () => {
    if (!id || !window.confirm('Are you sure you want to cancel this booking?')) return;
    setCancelling(true);
    try {
      const res = await bookingApi.cancel(id);
      if (!res.data.success) { setError(res.data.message ?? 'Cancellation failed.'); return; }
      setSuccess('Booking cancelled successfully.');
      setBooking(prev => prev ? { ...prev, bookingStatus: BookingStatus.Cancelled } : null);
    } catch {
      setError('Network error. Please try again.');
    } finally {
      setCancelling(false);
    }
  };

  if (loading) return <LoadingSpinner />;
  if (!booking) return null;

  const statusColor =
    booking.bookingStatus === BookingStatus.Confirmed ? 'success'
    : booking.bookingStatus === BookingStatus.Cancelled ? 'danger'
    : booking.bookingStatus === BookingStatus.CheckedIn ? 'info'
    : 'secondary';

  return (
    <>
      <button className="btn btn-link text-muted p-0 mb-3" onClick={() => navigate('/bookings')}>
        ← Back to My Bookings
      </button>

      {success && <AlertMessage type="success" message={success} onClose={() => setSuccess('')} />}
      {error && <AlertMessage type="danger" message={error} onClose={() => setError('')} />}

      <div className="card shadow border-0">
        <div className="card-header bg-dark text-white p-3">
          <div className="d-flex justify-content-between align-items-center">
            <div>
              <h4 className="mb-0 fw-bold" style={{ fontFamily: 'monospace', letterSpacing: '0.1rem' }}>
                PNR: {booking.pnrNumber}
              </h4>
              <small className="opacity-75">
                Booked on {new Date(booking.createdDate).toLocaleString('en-GB')}
              </small>
            </div>
            <span className={`badge bg-${statusColor} fs-6 px-3 py-2`}>
              {BookingStatusLabels[booking.bookingStatus]}
            </span>
          </div>
        </div>
        <div className="card-body p-4">
          <div className="row g-4">
            <div className="col-md-6">
              <h5 className="fw-semibold mb-3">✈ Flight Details</h5>
              <table className="table table-borderless">
                <tbody>
                  <tr><td className="text-muted w-50">Flight Number</td><td className="fw-semibold">{booking.flightNumber}</td></tr>
                  <tr><td className="text-muted">Route</td><td className="fw-semibold">{booking.departureCity} → {booking.arrivalCity}</td></tr>
                  <tr>
                    <td className="text-muted">Departure</td>
                    <td className="fw-semibold">{new Date(booking.departureDateTime).toLocaleString('en-GB')}</td>
                  </tr>
                </tbody>
              </table>
            </div>
            <div className="col-md-6">
              <h5 className="fw-semibold mb-3">👤 Passenger Details</h5>
              <table className="table table-borderless">
                <tbody>
                  <tr><td className="text-muted w-50">Passenger</td><td className="fw-semibold">{booking.passengerName}</td></tr>
                  <tr><td className="text-muted">Seat</td><td className="fw-semibold">{booking.seatNumber} — {SeatClassLabels[booking.seatClass]}</td></tr>
                  <tr>
                    <td className="text-muted">Total Price</td>
                    <td className="fw-bold text-success fs-5">{booking.totalPrice.toLocaleString()} {CurrencyLabels[booking.currency]}</td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>

          {booking.boardingPassNumber && (
            <div className="alert alert-info mt-2">
              <strong>Boarding Pass:</strong> {booking.boardingPassNumber}
              {booking.checkInTime && (
                <span className="ms-3 text-muted small">
                  Checked in at {new Date(booking.checkInTime).toLocaleTimeString('en-GB')}
                </span>
              )}
            </div>
          )}

          {booking.bookingStatus === BookingStatus.Confirmed &&
           new Date(booking.departureDateTime) > new Date() && (
            <div className="text-end mt-3">
              <button className="btn btn-danger" onClick={handleCancel} disabled={cancelling}>
                {cancelling ? <span className="spinner-border spinner-border-sm me-2" /> : null}
                Cancel Booking
              </button>
            </div>
          )}
        </div>
      </div>
    </>
  );
}
