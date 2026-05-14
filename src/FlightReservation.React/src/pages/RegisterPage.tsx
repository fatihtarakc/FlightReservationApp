import { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { authApi } from '../api/authApi';
import AlertMessage from '../components/AlertMessage';
import type { RegisterDto } from '../types';

const INITIAL: RegisterDto = {
  name: '', surname: '', username: '', email: '', password: '',
  phoneNumber: '', birthDate: '', preferredNotificationChannel: 1, nationality: '',
};

export default function RegisterPage() {
  const navigate = useNavigate();
  const [form, setForm] = useState<RegisterDto>(INITIAL);
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);

  const set = (field: keyof RegisterDto) => (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) =>
    setForm(f => ({ ...f, [field]: field === 'preferredNotificationChannel' ? Number(e.target.value) : e.target.value }));

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    setLoading(true);
    try {
      const res = await authApi.register(form);
      if (!res.data.success) { setError(res.data.message ?? 'Registration failed.'); return; }
      navigate(`/verify-email?email=${encodeURIComponent(form.email)}`);
    } catch {
      setError('Network error. Please try again.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="row justify-content-center">
      <div className="col-md-7">
        <div className="card shadow border-0">
          <div className="card-body p-4">
            <div className="text-center mb-4">
              <div style={{ fontSize: '3rem' }}>👤</div>
              <h2 className="fw-bold">Create Account</h2>
              <p className="text-muted">Join FlightReservation today</p>
            </div>
            {error && <AlertMessage type="danger" message={error} onClose={() => setError('')} />}
            <form onSubmit={handleSubmit}>
              <div className="row g-3">
                <div className="col-md-6">
                  <label className="form-label fw-semibold">First Name</label>
                  <input className="form-control" placeholder="John" value={form.name} onChange={set('name')} required />
                </div>
                <div className="col-md-6">
                  <label className="form-label fw-semibold">Last Name</label>
                  <input className="form-control" placeholder="Doe" value={form.surname} onChange={set('surname')} required />
                </div>
                <div className="col-md-6">
                  <label className="form-label fw-semibold">Username</label>
                  <input className="form-control" placeholder="johndoe" value={form.username} onChange={set('username')} required />
                </div>
                <div className="col-md-6">
                  <label className="form-label fw-semibold">Email</label>
                  <input type="email" className="form-control" placeholder="john@example.com" value={form.email} onChange={set('email')} required />
                </div>
                <div className="col-md-6">
                  <label className="form-label fw-semibold">Password</label>
                  <input type="password" className="form-control" placeholder="••••••••" value={form.password} onChange={set('password')} required />
                </div>
                <div className="col-md-6">
                  <label className="form-label fw-semibold">Phone Number</label>
                  <input className="form-control" placeholder="+905551234567" value={form.phoneNumber} onChange={set('phoneNumber')} required />
                </div>
                <div className="col-md-6">
                  <label className="form-label fw-semibold">Date of Birth</label>
                  <input type="date" className="form-control" value={form.birthDate} onChange={set('birthDate')} required />
                </div>
                <div className="col-md-6">
                  <label className="form-label fw-semibold">Nationality <span className="text-muted">(optional)</span></label>
                  <input className="form-control" placeholder="Turkish" value={form.nationality} onChange={set('nationality')} />
                </div>
                <div className="col-12">
                  <label className="form-label fw-semibold">Notification Preference</label>
                  <select className="form-select" value={form.preferredNotificationChannel} onChange={set('preferredNotificationChannel')}>
                    <option value={1}>Email</option>
                    <option value={2}>SMS</option>
                    <option value={4}>WhatsApp</option>
                    <option value={7}>All (Email + SMS + WhatsApp)</option>
                  </select>
                </div>
                <div className="col-12">
                  <button type="submit" className="btn btn-primary w-100 btn-lg" disabled={loading}>
                    {loading ? <span className="spinner-border spinner-border-sm me-2" /> : null}
                    Create Account
                  </button>
                </div>
              </div>
            </form>
            <hr className="my-4" />
            <p className="text-center mb-0">
              Already have an account? <Link to="/login" className="text-primary fw-semibold">Sign In</Link>
            </p>
          </div>
        </div>
      </div>
    </div>
  );
}
