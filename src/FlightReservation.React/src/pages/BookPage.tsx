import { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { flightApi } from '../api/flightApi';
import { bookingApi } from '../api/bookingApi';
import type { FlightDto, SeatDto } from '../types';
import { SeatClass, SeatClassLabels, SeatColumnLabels } from '../types';
import LoadingSpinner from '../components/LoadingSpinner';
import AlertMessage from '../components/AlertMessage';

export default function BookPage() {
  const { flightId } = useParams<{ flightId: string }>();
  const navigate = useNavigate();
  const [flight, setFlight] = useState<FlightDto | null>(null);
  const [seats, setSeats] = useState<SeatDto[]>([]);
  const [selectedSeatId, setSelectedSeatId] = useState('');
  const [loading, setLoading] = useState(true);
  const [booking, setBooking] = useState(false);
  const [error, setError] = useState('');

  useEffect(() => {
    if (!flightId) return;
    Promise.all([
      flightApi.getById(flightId),
      flightApi.getAvailableSeats(flightId),
    ]).then(([fr, sr]) => {
      if (fr.data.success && fr.data.data) setFlight(fr.data.data);
      if (sr.data.success && sr.data.data) setSeats(sr.data.data);
    }).finally(() => setLoading(false));
  }, [flightId]);

  const handleBook = async () => {
    if (!flightId || !selectedSeatId) return;
    setBooking(true);
    setError('');
    try {
      const res = await bookingApi.book({ flightId, seatId: selectedSeatId });
      if (!res.data.success || !res.data.data) {
        setError(res.data.message ?? 'Booking failed.');
        return;
      }
      navigate(`/bookings/${res.data.data.id}`, { state: { success: `Booking confirmed! PNR: ${res.data.data.pnrNumber}` } });
    } catch {
      setError('Network error. Please try again.');
    } finally {
      setBooking(false);
    }
  };

  if (loading) return <LoadingSpinner />;
  if (!flight) return null;

  const seatClasses = [...new Set(seats.map(s => s.seatClass))].sort();

  return (
    <>
      <button className="btn btn-link text-muted p-0 mb-3" onClick={() => navigate(-1)}>
        ← Back to Flight Details
      </button>

      <div className="row g-4">
        <div className="col-md-8">
          <div className="card shadow border-0 mb-4">
            <div className="card-header bg-primary text-white">
              <h5 className="mb-0">✈ Select Your Seat</h5>
            </div>
            <div className="card-body p-4">
              {error && <AlertMessage type="danger" message={error} onClose={() => setError('')} />}
              {seats.length === 0 ? (
                <div className="text-center py-4">
                  <div style={{ fontSize: '3rem' }}>❌</div>
                  <h5 className="mt-3">No seats available for this flight.</h5>
                </div>
              ) : (
                <>
                  {seatClasses.map(cls => (
                    <div key={cls} className="mb-4">
                      <h6 className="fw-semibold text-muted mb-2">{SeatClassLabels[cls as SeatClass]}</h6>
                      <div className="d-flex flex-wrap gap-2">
                        {seats
                          .filter(s => s.seatClass === cls)
                          .sort((a, b) => a.row - b.row || a.column - b.column)
                          .map(seat => (
                            <button
                              key={seat.id}
                              className={`btn btn-sm ${selectedSeatId === seat.id ? 'btn-primary' : 'btn-outline-primary'}`}
                              onClick={() => setSelectedSeatId(seat.id)}
                              title={[
                                `Row ${seat.row} ${SeatColumnLabels[seat.column]}`,
                                seat.isWindowSeat ? 'Window' : '',
                                seat.isAisleSeat ? 'Aisle' : '',
                                seat.hasExtraLegRoom ? 'Extra Legroom' : '',
                              ].filter(Boolean).join(' | ')}
                            >
                              {seat.row}{SeatColumnLabels[seat.column]}
                            </button>
                          ))}
                      </div>
                    </div>
                  ))}
                  <button
                    className="btn btn-success btn-lg w-100 mt-3"
                    onClick={handleBook}
                    disabled={!selectedSeatId || booking}
                  >
                    {booking ? <span className="spinner-border spinner-border-sm me-2" /> : null}
                    Confirm Booking
                  </button>
                </>
              )}
            </div>
          </div>
        </div>

        <div className="col-md-4">
          <div className="card shadow border-0" style={{ position: 'sticky', top: '1rem' }}>
            <div className="card-header bg-dark text-white">
              <h5 className="mb-0">🧾 Flight Summary</h5>
            </div>
            <div className="card-body p-3">
              {[
                ['Airline', flight.airlineName],
                ['Flight', flight.number],
                ['Route', `${flight.departureCity} (${flight.departureAirportIata}) → ${flight.arrivalCity} (${flight.arrivalAirportIata})`],
                ['Departure', new Date(flight.departureDateTime).toLocaleString('en-GB')],
                ['Arrival', new Date(flight.arrivalDateTime).toLocaleString('en-GB')],
              ].map(([label, value]) => (
                <div key={label} className="mb-2">
                  <div className="text-muted small">{label}</div>
                  <div className="fw-semibold">{value}</div>
                </div>
              ))}
              <hr />
              <div className="text-muted small">Economy from</div>
              <div className="fw-bold fs-5 text-success">
                {flight.baseEconomyPrice.toLocaleString()} {flight.currency}
              </div>
              <div className="alert alert-info p-2 small mt-2 mb-0">
                Final price depends on the selected seat class.
              </div>
            </div>
          </div>
        </div>
      </div>
    </>
  );
}
