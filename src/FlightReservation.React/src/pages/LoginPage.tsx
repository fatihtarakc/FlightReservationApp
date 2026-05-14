import { useState } from 'react';
import { Link, useNavigate, useLocation } from 'react-router-dom';
import { authApi } from '../api/authApi';
import { useAuth } from '../context/AuthContext';
import AlertMessage from '../components/AlertMessage';

export default function LoginPage() {
  const navigate = useNavigate();
  const location = useLocation();
  const { login } = useAuth();
  const from = (location.state as { from?: { pathname: string } })?.from?.pathname ?? '/';

  const [form, setForm] = useState({ email: '', password: '' });
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    setLoading(true);
    try {
      const res = await authApi.signIn(form);
      const body = res.data;
      if (!body.success || !body.data) {
        setError(body.message ?? 'Login failed.');
        return;
      }
      login(body.data.accessToken, body.data.expiration);
      navigate(from, { replace: true });
    } catch {
      setError('Network error. Please try again.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="row justify-content-center">
      <div className="col-md-5">
        <div className="card shadow border-0">
          <div className="card-body p-4">
            <div className="text-center mb-4">
              <div style={{ fontSize: '3rem' }}>👤</div>
              <h2 className="fw-bold">Sign In</h2>
              <p className="text-muted">Welcome back to FlightReservation</p>
            </div>
            {error && <AlertMessage type="danger" message={error} onClose={() => setError('')} />}
            <form onSubmit={handleSubmit}>
              <div className="mb-3">
                <label className="form-label fw-semibold">Email</label>
                <input
                  type="email"
                  className="form-control"
                  placeholder="your@email.com"
                  value={form.email}
                  onChange={e => setForm({ ...form, email: e.target.value })}
                  required
                  autoComplete="email"
                />
              </div>
              <div className="mb-3">
                <label className="form-label fw-semibold">Password</label>
                <input
                  type="password"
                  className="form-control"
                  placeholder="••••••••"
                  value={form.password}
                  onChange={e => setForm({ ...form, password: e.target.value })}
                  required
                  autoComplete="current-password"
                />
              </div>
              <div className="mb-3 text-end">
                <Link to="/forgot-password" className="text-muted small">Forgot password?</Link>
              </div>
              <button type="submit" className="btn btn-primary w-100 btn-lg" disabled={loading}>
                {loading ? <span className="spinner-border spinner-border-sm me-2" /> : null}
                Sign In
              </button>
            </form>
            <hr className="my-4" />
            <p className="text-center mb-0">
              Don't have an account? <Link to="/register" className="text-primary fw-semibold">Register</Link>
            </p>
          </div>
        </div>
      </div>
    </div>
  );
}
