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

  const [form, setForm] = useState({ usernameOrEmail: '', password: '' });
  const [showPw, setShowPw] = useState(false);
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    if (!form.usernameOrEmail.trim()) { setError('Username or email is required.'); return; }
    if (!form.password) { setError('Password is required.'); return; }

    setLoading(true);
    try {
      const res = await authApi.signIn(form);
      const body = res.data;
      if (!body.success || !body.data) {
        setError(body.message ?? 'Sign in failed. Please check your credentials.');
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
    <div className="row justify-content-center mt-4">
      <div className="col-sm-10 col-md-6 col-lg-5">
        <div className="card shadow-lg border-0 rounded-4">
          <div className="card-body p-4 p-md-5">

            <div className="text-center mb-4">
              <div className="display-6 mb-2">✈</div>
              <h2 className="fw-bold mb-1">Welcome Back</h2>
              <p className="text-muted mb-0">Sign in to FlightReservation</p>
            </div>

            {error && <AlertMessage type="danger" message={error} onClose={() => setError('')} />}

            <form onSubmit={handleSubmit} noValidate>
              <div className="mb-3">
                <label className="form-label fw-semibold">Username or Email</label>
                <div className="input-group">
                  <span className="input-group-text bg-light">
                    <i className="bi bi-person text-muted"></i>
                  </span>
                  <input
                    type="text"
                    className="form-control"
                    placeholder="username or email@example.com"
                    value={form.usernameOrEmail}
                    onChange={e => setForm({ ...form, usernameOrEmail: e.target.value })}
                    autoComplete="username"
                    required
                  />
                </div>
              </div>

              <div className="mb-3">
                <label className="form-label fw-semibold">Password</label>
                <div className="input-group">
                  <span className="input-group-text bg-light">
                    <i className="bi bi-lock text-muted"></i>
                  </span>
                  <input
                    type={showPw ? 'text' : 'password'}
                    className="form-control"
                    placeholder="••••••••"
                    value={form.password}
                    onChange={e => setForm({ ...form, password: e.target.value })}
                    autoComplete="current-password"
                    required
                  />
                  <button
                    type="button"
                    className="btn btn-outline-secondary"
                    onClick={() => setShowPw(p => !p)}
                    tabIndex={-1}
                  >
                    <i className={`bi bi-eye${showPw ? '-slash' : ''}`}></i>
                  </button>
                </div>
              </div>

              <div className="d-flex justify-content-end mb-3">
                <Link to="/forgot-password" className="text-muted small text-decoration-none">
                  Forgot password?
                </Link>
              </div>

              <button
                type="submit"
                className="btn btn-primary w-100 btn-lg fw-semibold"
                disabled={loading}
              >
                {loading
                  ? <><span className="spinner-border spinner-border-sm me-2" />Signing in...</>
                  : 'Sign In'}
              </button>
            </form>

            <hr className="my-4" />
            <p className="text-center mb-0 text-muted">
              Don't have an account?{' '}
              <Link to="/register" className="text-primary fw-semibold text-decoration-none">Register</Link>
            </p>
          </div>
        </div>
      </div>
    </div>
  );
}
