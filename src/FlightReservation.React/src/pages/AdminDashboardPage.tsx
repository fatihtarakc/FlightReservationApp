import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { adminApi } from '../api/adminApi';
import type { AdminDashboard, FlightPassengerStat } from '../types';
import LoadingSpinner from '../components/LoadingSpinner';

export default function AdminDashboardPage() {
  const navigate = useNavigate();
  const [dash, setDash]   = useState<AdminDashboard | null>(null);
  const [stats, setStats] = useState<FlightPassengerStat[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    Promise.all([adminApi.getDashboard(), adminApi.getFlightPassengerStats()])
      .then(([dr, sr]) => {
        if (dr.data.success && dr.data.data) setDash(dr.data.data);
        if (sr.data.success && sr.data.data) setStats(sr.data.data);
      })
      .finally(() => setLoading(false));
  }, []);

  if (loading) return <LoadingSpinner />;
  if (!dash) return <div className="alert alert-danger">Failed to load dashboard.</div>;

  const statCards = [
    { label: 'Total Flights',      value: dash.totalFlights,         color: 'primary' },
    { label: 'Scheduled',          value: dash.scheduledFlights,     color: 'success' },
    { label: 'Cancelled Flights',  value: dash.cancelledFlights,     color: 'danger'  },
    { label: 'Total Bookings',     value: dash.totalBookings,        color: 'info'    },
    { label: 'Confirmed',          value: dash.confirmedBookings,    color: 'success' },
    { label: 'Cancelled Bookings', value: dash.cancelledBookings,    color: 'danger'  },
    { label: 'Checked In',         value: dash.checkedInPassengers,  color: 'warning' },
    { label: 'Avg Occupancy',      value: `${dash.averageOccupancyRate.toFixed(1)}%`, color: 'secondary' },
  ];

  return (
    <>
      <div className="d-flex justify-content-between align-items-center mb-4">
        <h2 className="fw-bold mb-0">⚙ Admin Dashboard</h2>
        <button className="btn btn-outline-secondary btn-sm" onClick={() => navigate('/admin/bookings')}>
          All Bookings →
        </button>
      </div>

      {/* Stat cards */}
      <div className="row g-3 mb-4">
        {statCards.map(c => (
          <div key={c.label} className="col-6 col-md-3">
            <div className={`card border-0 shadow-sm text-center p-3 border-top border-${c.color} border-3`}>
              <div className={`fw-bold fs-3 text-${c.color}`}>{c.value}</div>
              <div className="text-muted small">{c.label}</div>
            </div>
          </div>
        ))}
      </div>

      {/* Revenue */}
      <div className="card border-0 shadow-sm mb-4">
        <div className="card-header bg-light fw-semibold d-flex justify-content-between">
          <span>Revenue (Confirmed + Checked In)</span>
          <span className="text-success fw-bold">{dash.totalRevenue.toLocaleString()}</span>
        </div>
      </div>

      {/* Flight occupancy table */}
      {stats.length > 0 && (
        <div className="card border-0 shadow-sm">
          <div className="card-header bg-light fw-semibold">Flight Occupancy</div>
          <div className="card-body p-0">
            <div className="table-responsive">
              <table className="table table-hover mb-0 small">
                <thead className="table-light">
                  <tr>
                    <th>Flight</th>
                    <th>Route</th>
                    <th>Departure</th>
                    <th>Booked</th>
                    <th>Total</th>
                    <th>Occupancy</th>
                  </tr>
                </thead>
                <tbody>
                  {stats.map(s => {
                    const barColor =
                      s.occupancyRate >= 80 ? 'danger'
                      : s.occupancyRate >= 50 ? 'warning'
                      : 'success';
                    return (
                      <tr key={s.flightId}>
                        <td className="fw-semibold">{s.flightNumber}</td>
                        <td>{s.departureCity} → {s.arrivalCity}</td>
                        <td className="text-nowrap">
                          {new Date(s.departureDateTime).toLocaleDateString('en-GB', {
                            day: '2-digit', month: 'short',
                          })}{' '}
                          {new Date(s.departureDateTime).toLocaleTimeString('en-GB', {
                            hour: '2-digit', minute: '2-digit',
                          })}
                        </td>
                        <td>{s.bookedSeats}</td>
                        <td>{s.totalSeats}</td>
                        <td>
                          <div className="d-flex align-items-center gap-2">
                            <div className="progress flex-grow-1" style={{ height: 8 }}>
                              <div
                                className={`progress-bar bg-${barColor}`}
                                style={{ width: `${s.occupancyRate}%` }}
                              />
                            </div>
                            <span className="text-muted" style={{ minWidth: 38 }}>
                              {s.occupancyRate.toFixed(0)}%
                            </span>
                          </div>
                        </td>
                      </tr>
                    );
                  })}
                </tbody>
              </table>
            </div>
          </div>
        </div>
      )}
    </>
  );
}
