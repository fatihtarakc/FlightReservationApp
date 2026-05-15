import { useEffect, useState, useMemo } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { flightApi } from '../api/flightApi';
import { bookingApi } from '../api/bookingApi';
import type { FlightDetails, SeatModel } from '../types';
import { SeatClass, SeatColumn, BodyType, Currency, CurrencyLabels } from '../types';
import LoadingSpinner from '../components/LoadingSpinner';
import AlertMessage from '../components/AlertMessage';

const CLASS_STYLE: Record<SeatClass, { bg: string; active: string; label: string }> = {
  [SeatClass.First]:         { bg: '#fff3cd', active: '#ffc107', label: 'First Class' },
  [SeatClass.Business]:      { bg: '#cfe2ff', active: '#0d6efd', label: 'Business' },
  [SeatClass.PremiumEconomy]:{ bg: '#d1ecf1', active: '#0dcaf0', label: 'Premium Economy' },
  [SeatClass.Economy]:       { bg: '#f8f9fa', active: '#6c757d', label: 'Economy' },
};

const COL_LABELS: Record<SeatColumn, string> = {
  [SeatColumn.A]: 'A', [SeatColumn.B]: 'B', [SeatColumn.C]: 'C',
  [SeatColumn.D]: 'D', [SeatColumn.E]: 'E', [SeatColumn.F]: 'F',
  [SeatColumn.G]: 'G', [SeatColumn.H]: 'H', [SeatColumn.J]: 'J',
  [SeatColumn.K]: 'K',
};

const NARROW_LEFT  = [SeatColumn.A, SeatColumn.B, SeatColumn.C];
const NARROW_RIGHT = [SeatColumn.D, SeatColumn.E, SeatColumn.F];
const WIDE_LEFT    = [SeatColumn.A, SeatColumn.B, SeatColumn.C];
const WIDE_MID     = [SeatColumn.D, SeatColumn.E];
const WIDE_RIGHT   = [SeatColumn.F, SeatColumn.G, SeatColumn.H, SeatColumn.J, SeatColumn.K];

function SeatBtn({ seat, selected, onClick }: {
  seat: SeatModel;
  selected: boolean;
  onClick: () => void;
}) {
  const style = CLASS_STYLE[seat.seatClass];
  const label = `${seat.row}${COL_LABELS[seat.column]}`;
  const title = [
    label,
    seat.isWindowSeat   ? 'Window'         : '',
    seat.isAisleSeat    ? 'Aisle'          : '',
    seat.hasExtraLegRoom? 'Extra Legroom'  : '',
  ].filter(Boolean).join(' | ');

  if (!seat.isAvailable) {
    return (
      <div
        title={`${label} — Occupied`}
        style={{
          width: 36, height: 36, borderRadius: 4,
          background: '#dee2e6', color: '#adb5bd',
          display: 'flex', alignItems: 'center', justifyContent: 'center',
          fontSize: 10, fontWeight: 600, cursor: 'not-allowed',
          border: '1px solid #ced4da', userSelect: 'none',
        }}
      >
        {label}
      </div>
    );
  }

  return (
    <button
      title={title}
      onClick={onClick}
      style={{
        width: 36, height: 36, borderRadius: 4,
        background: selected ? style.active : style.bg,
        color: selected ? '#fff' : '#212529',
        border: `2px solid ${selected ? style.active : '#ced4da'}`,
        display: 'flex', alignItems: 'center', justifyContent: 'center',
        fontSize: 10, fontWeight: 700, cursor: 'pointer',
        transition: 'all 0.15s',
      }}
    >
      {label}
    </button>
  );
}

export default function BookPage() {
  const { flightId } = useParams<{ flightId: string }>();
  const navigate = useNavigate();

  const [flight, setFlight]           = useState<FlightDetails | null>(null);
  const [seats, setSeats]             = useState<SeatModel[]>([]);
  const [selectedSeatId, setSelected] = useState('');
  const [loading, setLoading]         = useState(true);
  const [booking, setBooking]         = useState(false);
  const [error, setError]             = useState('');

  useEffect(() => {
    if (!flightId) return;
    Promise.all([
      flightApi.getById(flightId),
      flightApi.getAllSeats(flightId),
    ]).then(([fr, sr]) => {
      if (fr.data.success && fr.data.data) setFlight(fr.data.data);
      if (sr.data.success && sr.data.data) setSeats(sr.data.data);
    }).finally(() => setLoading(false));
  }, [flightId]);

  const bodyType = useMemo(() =>
    seats.some(s => s.column > SeatColumn.F) ? BodyType.WideBody : BodyType.NarrowBody,
  [seats]);

  const leftCols  = bodyType === BodyType.WideBody ? WIDE_LEFT  : NARROW_LEFT;
  const midCols   = bodyType === BodyType.WideBody ? WIDE_MID   : [];
  const rightCols = bodyType === BodyType.WideBody ? WIDE_RIGHT : NARROW_RIGHT;

  const rows = useMemo(() =>
    [...new Set(seats.map(s => s.row))].sort((a, b) => a - b),
  [seats]);

  const seatMap = useMemo(() => {
    const m = new Map<string, SeatModel>();
    seats.forEach(s => m.set(`${s.row}-${s.column}`, s));
    return m;
  }, [seats]);

  const selectedSeat = seats.find(s => s.id === selectedSeatId);

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
      navigate(`/bookings/${res.data.data.id}`, {
        state: { success: `Booking confirmed! PNR: ${res.data.data.pnrNumber}` },
      });
    } catch {
      setError('Network error. Please try again.');
    } finally {
      setBooking(false);
    }
  };

  const renderGroup = (cols: SeatColumn[], row: number) =>
    cols.map(col => {
      const seat = seatMap.get(`${row}-${col}`);
      if (!seat) return <div key={col} style={{ width: 36, height: 36 }} />;
      return (
        <SeatBtn
          key={col}
          seat={seat}
          selected={selectedSeatId === seat.id}
          onClick={() => setSelected(prev => prev === seat.id ? '' : seat.id)}
        />
      );
    });

  if (loading) return <LoadingSpinner />;
  if (!flight) return null;

  const currencyLabel = CurrencyLabels[flight.currency] ?? String(flight.currency);

  return (
    <>
      <button className="btn btn-link text-muted p-0 mb-3" onClick={() => navigate(-1)}>
        ← Back to Flight Details
      </button>

      <div className="row g-4">
        {/* ── Seat map ── */}
        <div className="col-md-8">
          <div className="card shadow border-0 mb-4">
            <div className="card-header text-white" style={{ background: '#0f3460' }}>
              <h5 className="mb-0">
                ✈ Select Your Seat &mdash;{' '}
                {bodyType === BodyType.WideBody ? 'Wide Body' : 'Narrow Body'}
              </h5>
            </div>
            <div className="card-body p-4">
              {error && <AlertMessage type="danger" message={error} onClose={() => setError('')} />}

              {/* Legend */}
              <div className="d-flex flex-wrap gap-3 mb-4 pb-3 border-bottom">
                {([SeatClass.First, SeatClass.Business, SeatClass.PremiumEconomy, SeatClass.Economy] as SeatClass[]).map(cls => (
                  <div key={cls} className="d-flex align-items-center gap-1">
                    <div style={{ width: 16, height: 16, borderRadius: 3, background: CLASS_STYLE[cls].active }} />
                    <span className="small fw-semibold">{CLASS_STYLE[cls].label}</span>
                  </div>
                ))}
                <div className="d-flex align-items-center gap-1">
                  <div style={{ width: 16, height: 16, borderRadius: 3, background: '#dee2e6', border: '1px solid #ccc' }} />
                  <span className="small text-muted">Occupied</span>
                </div>
              </div>

              {seats.length === 0 ? (
                <div className="text-center py-4">
                  <div style={{ fontSize: '3rem' }}>❌</div>
                  <h5 className="mt-3">No seats available for this flight.</h5>
                </div>
              ) : (
                <div style={{ overflowX: 'auto' }}>
                  {/* Column header row */}
                  <div className="d-flex align-items-center gap-1 mb-2" style={{ paddingLeft: 42 }}>
                    {leftCols.map(c => (
                      <div key={c} style={{ width: 36, textAlign: 'center', fontWeight: 700, fontSize: 12, color: '#6c757d' }}>
                        {COL_LABELS[c]}
                      </div>
                    ))}
                    <div style={{ width: 20 }} />
                    {midCols.map(c => (
                      <div key={c} style={{ width: 36, textAlign: 'center', fontWeight: 700, fontSize: 12, color: '#6c757d' }}>
                        {COL_LABELS[c]}
                      </div>
                    ))}
                    {midCols.length > 0 && <div style={{ width: 20 }} />}
                    {rightCols.map(c => (
                      <div key={c} style={{ width: 36, textAlign: 'center', fontWeight: 700, fontSize: 12, color: '#6c757d' }}>
                        {COL_LABELS[c]}
                      </div>
                    ))}
                  </div>

                  {/* Seat rows */}
                  {rows.map(row => {
                    const firstSeat = seatMap.get(`${row}-${leftCols[0]}`);
                    const cls = firstSeat?.seatClass ?? SeatClass.Economy;
                    return (
                      <div
                        key={row}
                        className="d-flex align-items-center gap-1 mb-1"
                        style={{ background: CLASS_STYLE[cls].bg, borderRadius: 4, padding: '2px 4px' }}
                      >
                        <div style={{ width: 32, textAlign: 'right', fontSize: 11, fontWeight: 600, color: '#6c757d', flexShrink: 0 }}>
                          {row}
                        </div>
                        <div style={{ width: 4 }} />
                        <div className="d-flex gap-1">{renderGroup(leftCols, row)}</div>
                        <div style={{ width: 20 }} />
                        {midCols.length > 0 && (
                          <>
                            <div className="d-flex gap-1">{renderGroup(midCols, row)}</div>
                            <div style={{ width: 20 }} />
                          </>
                        )}
                        <div className="d-flex gap-1">{renderGroup(rightCols, row)}</div>
                      </div>
                    );
                  })}
                </div>
              )}

              {seats.length > 0 && (
                <button
                  className="btn btn-success btn-lg w-100 mt-4"
                  onClick={handleBook}
                  disabled={!selectedSeatId || booking}
                >
                  {booking ? <span className="spinner-border spinner-border-sm me-2" /> : null}
                  {selectedSeatId ? 'Confirm Booking — No Payment Required' : 'Select a Seat to Continue'}
                </button>
              )}
            </div>
          </div>
        </div>

        {/* ── Flight summary ── */}
        <div className="col-md-4">
          <div className="card shadow border-0" style={{ position: 'sticky', top: '1rem' }}>
            <div className="card-header bg-dark text-white">
              <h5 className="mb-0">🧾 Flight Summary</h5>
            </div>
            <div className="card-body p-3">
              {([
                ['Airline',   flight.airlineName],
                ['Flight',    flight.number],
                ['Route',     `${flight.departureCity} (${flight.departureAirportIata}) → ${flight.arrivalCity} (${flight.arrivalAirportIata})`],
                ['Departure', new Date(flight.departureDateTime).toLocaleString('en-GB')],
                ['Arrival',   new Date(flight.arrivalDateTime).toLocaleString('en-GB')],
              ] as [string, string][]).map(([label, value]) => (
                <div key={label} className="mb-2">
                  <div className="text-muted small">{label}</div>
                  <div className="fw-semibold">{value}</div>
                </div>
              ))}
              <hr />
              <div className="text-muted small">Economy from</div>
              <div className="fw-bold fs-5 text-success">
                {flight.baseEconomyPrice.toLocaleString()} {currencyLabel}
              </div>

              {selectedSeat ? (
                <div className="alert alert-success p-2 small mt-3 mb-0">
                  <strong>Selected:</strong>{' '}
                  Row {selectedSeat.row}{COL_LABELS[selectedSeat.column]} &mdash; {CLASS_STYLE[selectedSeat.seatClass].label}
                  {selectedSeat.isWindowSeat    && ' · Window'}
                  {selectedSeat.isAisleSeat     && ' · Aisle'}
                  {selectedSeat.hasExtraLegRoom && ' · Extra Legroom'}
                </div>
              ) : (
                <div className="alert alert-info p-2 small mt-2 mb-0">
                  Click a seat on the map to select it.
                </div>
              )}
            </div>
          </div>
        </div>
      </div>
    </>
  );
}
