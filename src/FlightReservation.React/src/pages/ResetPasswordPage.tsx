import { useState } from 'react';
import { useNavigate, useSearchParams } from 'react-router-dom';
import { authApi } from '../api/authApi';
import AlertMessage from '../components/AlertMessage';

export default function ResetPasswordPage() {
  const [searchParams] = useSearchParams();
  const email = searchParams.get('email') ?? '';
  const navigate = useNavigate();
  const [form, setForm] = useState({ code: '', newPassword: '' });
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    setLoading(true);
    try {
      const res = await authApi.resetPassword({ email, ...form });
      if (!res.data.success) { setError(res.data.message ?? 'Reset failed.'); return; }
      navigate('/login', { state: { success: 'Password reset successfully. Please log in.' } });
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
              <div style={{ fontSize: '3rem' }}>🔒</div>
              <h2 className="fw-bold">Reset Password</h2>
              <p className="text-muted">Code sent to <strong>{email}</strong></p>
            </div>
            {error && <AlertMessage type="danger" message={error} onClose={() => setError('')} />}
            <form onSubmit={handleSubmit}>
              <div className="mb-3">
                <label className="form-label fw-semibold">Reset Code</label>
                <input
                  className="form-control form-control-lg text-center"
                  placeholder="000000"
                  maxLength={6}
                  style={{ letterSpacing: '0.5rem' }}
                  value={form.code}
                  onChange={e => setForm({ ...form, code: e.target.value })}
                  required
                />
              </div>
              <div className="mb-3">
                <label className="form-label fw-semibold">New Password</label>
                <input
                  type="password"
                  className="form-control"
                  placeholder="••••••••"
                  value={form.newPassword}
                  onChange={e => setForm({ ...form, newPassword: e.target.value })}
                  required
                />
              </div>
              <button type="submit" className="btn btn-primary w-100 btn-lg" disabled={loading}>
                {loading ? <span className="spinner-border spinner-border-sm me-2" /> : null}
                Reset Password
              </button>
            </form>
          </div>
        </div>
      </div>
    </div>
  );
}
