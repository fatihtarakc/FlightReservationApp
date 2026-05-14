import { useState } from 'react';
import { useNavigate, useSearchParams } from 'react-router-dom';
import { authApi } from '../api/authApi';
import AlertMessage from '../components/AlertMessage';

export default function VerifyEmailPage() {
  const [searchParams] = useSearchParams();
  const email = searchParams.get('email') ?? '';
  const navigate = useNavigate();
  const [code, setCode] = useState('');
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    setLoading(true);
    try {
      const res = await authApi.verifyEmail({ email, code });
      if (!res.data.success) { setError(res.data.message ?? 'Verification failed.'); return; }
      navigate('/login', { state: { success: 'Email verified. You can now log in.' } });
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
          <div className="card-body p-4 text-center">
            <div style={{ fontSize: '3rem' }}>📧</div>
            <h2 className="fw-bold mt-2">Verify Your Email</h2>
            <p className="text-muted">
              A verification code has been sent to <strong>{email}</strong>.
            </p>
            {error && <AlertMessage type="danger" message={error} onClose={() => setError('')} />}
            <form onSubmit={handleSubmit}>
              <div className="mb-3">
                <input
                  className="form-control form-control-lg text-center"
                  placeholder="000000"
                  maxLength={6}
                  style={{ letterSpacing: '0.5rem', fontSize: '1.5rem' }}
                  value={code}
                  onChange={e => setCode(e.target.value)}
                  required
                />
              </div>
              <button type="submit" className="btn btn-primary w-100 btn-lg" disabled={loading}>
                {loading ? <span className="spinner-border spinner-border-sm me-2" /> : null}
                Verify
              </button>
            </form>
          </div>
        </div>
      </div>
    </div>
  );
}
