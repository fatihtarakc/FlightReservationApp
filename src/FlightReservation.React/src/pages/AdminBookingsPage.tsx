import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { adminApi } from '../api/adminApi';
import type { AdminBookingItem } from '../types';
import { BookingStatus, BookingStatusLabels, SeatClassLabels, CurrencyLabels } from '../types';
import LoadingSpinner from '../components/LoadingSpinner';

function statusColor(s: BookingStatus): string {
  switch (s) {
    case BookingStatus.Confirmed:  return 'success';
    case BookingStatus.Cancelled:  return 'danger';
    case BookingStatus.CheckedIn:  return 'info';
    case BookingStatus.Boarded:    return 'primary';
    case BookingStatus.Completed:  return 'success';
    default:                       return 'secondary';
  }
}

export default function AdminBookingsPage() {
  const navigate = useNavigate();
  const [bookings, setBookings] = useState<AdminBookingItem[]>([]);
  const [loading, setLoading]   = useState(true);

  useEffect(() => {
    adminApi.getAllBookings()
      .then(res => { if (res.data.success && res.data.data) setBookings(res.data.data); })
      .finally(() => setLoading(false));
  }, []);

  if (loading) return <LoadingSpinner />;

  const revenue = bookings
    .filter(b => b.bookingStatus === BookingStatus.Confirmed || b.bookingStatus === BookingStatus.CheckedIn)
    .reduce((sum, b) => sum + b.totalPrice, 0);

  const sorted = [...bookings].sort(
    (a, b) => new Date(b.createdDate).getTime() - new Date(a.createdDate).getTime()
  );

  return (
    <>
      <div className="d-flex justify-content-between align-items-center mb-4">
        <h2 className="fw-bold mb-0">📋 All Bookings</h2>
        <button className="btn btn-outline-secondary btn-sm" onClick={() => navigate('/admin')}>
          ← Dashboard
        </button>
      </div>

      {bookings.length === 0 ? (
        <div className="text-center py-5 text-muted">
          <div style={{ fontSize: '4rem', opacity: 0.25 }}>📋</div>
          <h5 className="mt-3">No bookings found.</h5>
        </div>
      ) : (
        <div className="card border-0 shadow-sm">
          <div className="card-header bg-light d-flex justify-content-between align-items-center">
            <span className="fw-semibold">{bookings.length} total bookings</span>
            <span className="text-muted small">Revenue: {revenue.toLocaleString()} (Confirmed + CheckedIn)</span>
          </div>
          <div className="card-body p-0">
            <div className="table-responsive">
              <table className="table table-hover mb-0 small">
                <thead className="table-light">
                  <tr>
                    <th>PNR</th>
                    <th>Passenger</th>
                    <th>Flight</th>
                    <th>Route</th>
                    <th>Departure</th>
                    <th>Seat</th>
                    <th>Price</th>
                    <th>Status</th>
                    <th>Booked</th>
                  </tr>
                </thead>
                <tbody>
                  {sorted.map(b => (
                    <tr key={b.id}>
                      <td className="fw-bold font-monospace text-primary">{b.pnrNumber}</td>
                      <td>{b.passengerName}</td>
                      <td className="fw-semibold">{b.flightNumber}</td>
                      <td className="text-nowrap">{b.departureCity} → {b.arrivalCity}</td>
                      <td className="text-nowrap">
                        {new Date(b.departureDateTime).toLocaleDateString('en-GB', {
                          day: '2-digit', month: 'short',
                        })}{' '}
                        {new Date(b.departureDateTime).toLocaleTimeString('en-GB', {
                          hour: '2-digit', minute: '2-digit',
                        })}
                      </td>
                      <td>
                        <span className="fw-semibold">{b.seatNumber}</span>
                        <span className="badge bg-light text-dark border ms-1">
                          {SeatClassLabels[b.seatClass]}
                        </span>
                      </td>
                      <td className="fw-semibold text-success text-nowrap">
                        {b.totalPrice.toLocaleString()} {CurrencyLabels[b.currency]}
                      </td>
                      <td>
                        <span className={`badge bg-${statusColor(b.bookingStatus)}`}>
                          {BookingStatusLabels[b.bookingStatus]}
                        </span>
                      </td>
                      <td className="text-muted">
                        {new Date(b.createdDate).toLocaleDateString('en-GB', {
                          day: '2-digit', month: 'short',
                        })}
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          </div>
        </div>
      )}
    </>
  );
}
