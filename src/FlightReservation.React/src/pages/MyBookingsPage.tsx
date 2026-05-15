import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { bookingApi } from '../api/bookingApi';
import type { BookingListItem } from '../types';
import BookingCard from '../components/BookingCard';
import LoadingSpinner from '../components/LoadingSpinner';

export default function MyBookingsPage() {
  const navigate = useNavigate();
  const [bookings, setBookings] = useState<BookingListItem[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    bookingApi.myBookings()
      .then(res => {
        if (res.data.success && res.data.data) setBookings(res.data.data);
      })
      .finally(() => setLoading(false));
  }, []);

  if (loading) return <LoadingSpinner />;

  return (
    <>
      <h2 className="fw-bold mb-4">🎫 My Bookings</h2>

      {bookings.length === 0 ? (
        <div className="text-center py-5">
          <div style={{ fontSize: '5rem', opacity: 0.3 }}>🎫</div>
          <h4 className="text-muted mt-3">No bookings yet.</h4>
          <button className="btn btn-primary mt-3" onClick={() => navigate('/flights/search')}>
            Search Flights
          </button>
        </div>
      ) : (
        bookings.map(b => <BookingCard key={b.id} booking={b} />)
      )}
    </>
  );
}
