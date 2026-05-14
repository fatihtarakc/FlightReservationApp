import { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { authApi } from '../api/authApi';
import AlertMessage from '../components/AlertMessage';

export default function ForgotPasswordPage() {
  const navigate = useNavigate();
  const [email, setEmail] = useState('');
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    setLoading(true);
    try {
      await authApi.sendVerificationCode(email);
      navigate(`/reset-password?email=${encodeURIComponent(email)}`);
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
              <div style={{ fontSize: '3rem' }}>🔑</div>
              <h2 className="fw-bold">Forgot Password</h2>
              <p className="text-muted">Enter your email to receive a reset code</p>
            </div>
            {error && <AlertMessage type="danger" message={error} onClose={() => setError('')} />}
            <form onSubmit={handleSubmit}>
              <div className="mb-3">
                <label className="form-label fw-semibold">Email</label>
                <input
                  type="email"
                  className="form-control"
                  placeholder="your@email.com"
                  value={email}
                  onChange={e => setEmail(e.target.value)}
                  required
                />
              </div>
              <button type="submit" className="btn btn-warning w-100 btn-lg" disabled={loading}>
                {loading ? <span className="spinner-border spinner-border-sm me-2" /> : null}
                Send Reset Code
              </button>
            </form>
            <p className="text-center mt-3 mb-0">
              <Link to="/login" className="text-muted">← Back to Login</Link>
            </p>
          </div>
        </div>
      </div>
    </div>
  );
}
