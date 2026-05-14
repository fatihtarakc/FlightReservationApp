import { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { authApi } from '../api/authApi';
import AlertMessage from '../components/AlertMessage';
import PhoneInput from '../components/PhoneInput';
import type { RegisterDto } from '../types';

const INITIAL: RegisterDto = {
  name: '', surname: '', username: '', email: '', password: '',
  phoneNumber: '', birthDate: '', preferredNotificationChannel: 1, nationality: '',
};

function passwordStrength(pw: string): { level: number; label: string; color: string } {
  let score = 0;
  if (pw.length >= 8) score++;
  if (/[A-Z]/.test(pw)) score++;
  if (/[0-9]/.test(pw)) score++;
  if (/[^A-Za-z0-9]/.test(pw)) score++;
  if (score <= 1) return { level: score, label: 'Weak', color: 'danger' };
  if (score === 2) return { level: score, label: 'Fair', color: 'warning' };
  if (score === 3) return { level: score, label: 'Good', color: 'info' };
  return { level: score, label: 'Strong', color: 'success' };
}

function validate(form: RegisterDto): Partial<Record<keyof RegisterDto, string>> {
  const errors: Partial<Record<keyof RegisterDto, string>> = {};
  if (!form.name.trim()) errors.name = 'First name is required.';
  if (!form.surname.trim()) errors.surname = 'Last name is required.';
  if (!form.username.trim()) errors.username = 'Username is required.';
  else if (form.username.length < 3) errors.username = 'Username must be at least 3 characters.';
  if (!form.email.trim()) errors.email = 'Email is required.';
  else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(form.email)) errors.email = 'Enter a valid email address.';
  if (!form.password) errors.password = 'Password is required.';
  else if (form.password.length < 8) errors.password = 'Password must be at least 8 characters.';
  if (!form.phoneNumber || form.phoneNumber.replace(/\D/g, '').length < 8)
    errors.phoneNumber = 'Enter a valid phone number.';
  if (!form.birthDate) errors.birthDate = 'Date of birth is required.';
  else {
    const age = (Date.now() - new Date(form.birthDate).getTime()) / (365.25 * 24 * 3600 * 1000);
    if (age < 12) errors.birthDate = 'You must be at least 12 years old.';
  }
  return errors;
}

const today = new Date().toISOString().split('T')[0];
const maxDate = new Date();
maxDate.setFullYear(maxDate.getFullYear() - 12);
const maxBirthDate = maxDate.toISOString().split('T')[0];

export default function RegisterPage() {
  const navigate = useNavigate();
  const [form, setForm] = useState<RegisterDto>(INITIAL);
  const [errors, setErrors] = useState<Partial<Record<keyof RegisterDto, string>>>({});
  const [serverError, setServerError] = useState('');
  const [loading, setLoading] = useState(false);
  const [showPw, setShowPw] = useState(false);

  const set = (field: keyof RegisterDto) =>
    (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
      const value = field === 'preferredNotificationChannel' ? Number(e.target.value) : e.target.value;
      setForm(f => ({ ...f, [field]: value }));
      if (errors[field]) setErrors(prev => ({ ...prev, [field]: undefined }));
    };

  const pwStrength = passwordStrength(form.password);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    const validationErrors = validate(form);
    if (Object.keys(validationErrors).length > 0) {
      setErrors(validationErrors);
      return;
    }
    setServerError('');
    setLoading(true);
    try {
      const res = await authApi.register(form);
      if (!res.data.success) {
        setServerError(res.data.message ?? 'Registration failed.');
        return;
      }
      navigate(`/verify-email?email=${encodeURIComponent(form.email)}`);
    } catch {
      setServerError('Network error. Please try again.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="row justify-content-center mt-4 mb-5">
      <div className="col-sm-11 col-md-9 col-lg-8">
        <div className="card shadow-lg border-0 rounded-4">
          <div className="card-body p-4 p-md-5">

            <div className="text-center mb-4">
              <div className="display-6 mb-2">✈</div>
              <h2 className="fw-bold mb-1">Create Account</h2>
              <p className="text-muted mb-0">Join FlightReservation today</p>
            </div>

            {serverError && <AlertMessage type="danger" message={serverError} onClose={() => setServerError('')} />}

            <form onSubmit={handleSubmit} noValidate>
              <div className="row g-3">

                {/* Name */}
                <div className="col-sm-6">
                  <label className="form-label fw-semibold">First Name <span className="text-danger">*</span></label>
                  <input
                    className={`form-control ${errors.name ? 'is-invalid' : ''}`}
                    placeholder="John"
                    value={form.name}
                    onChange={set('name')}
                    autoComplete="given-name"
                  />
                  {errors.name && <div className="invalid-feedback">{errors.name}</div>}
                </div>

                {/* Surname */}
                <div className="col-sm-6">
                  <label className="form-label fw-semibold">Last Name <span className="text-danger">*</span></label>
                  <input
                    className={`form-control ${errors.surname ? 'is-invalid' : ''}`}
                    placeholder="Doe"
                    value={form.surname}
                    onChange={set('surname')}
                    autoComplete="family-name"
                  />
                  {errors.surname && <div className="invalid-feedback">{errors.surname}</div>}
                </div>

                {/* Username */}
                <div className="col-sm-6">
                  <label className="form-label fw-semibold">Username <span className="text-danger">*</span></label>
                  <div className="input-group">
                    <span className="input-group-text bg-light">@</span>
                    <input
                      className={`form-control ${errors.username ? 'is-invalid' : ''}`}
                      placeholder="johndoe"
                      value={form.username}
                      onChange={set('username')}
                      autoComplete="username"
                      minLength={3}
                    />
                    {errors.username && <div className="invalid-feedback">{errors.username}</div>}
                  </div>
                </div>

                {/* Email */}
                <div className="col-sm-6">
                  <label className="form-label fw-semibold">Email <span className="text-danger">*</span></label>
                  <div className="input-group">
                    <span className="input-group-text bg-light">
                      <i className="bi bi-envelope text-muted"></i>
                    </span>
                    <input
                      type="email"
                      className={`form-control ${errors.email ? 'is-invalid' : ''}`}
                      placeholder="john@example.com"
                      value={form.email}
                      onChange={set('email')}
                      autoComplete="email"
                    />
                    {errors.email && <div className="invalid-feedback">{errors.email}</div>}
                  </div>
                </div>

                {/* Password */}
                <div className="col-sm-6">
                  <label className="form-label fw-semibold">Password <span className="text-danger">*</span></label>
                  <div className="input-group">
                    <span className="input-group-text bg-light">
                      <i className="bi bi-lock text-muted"></i>
                    </span>
                    <input
                      type={showPw ? 'text' : 'password'}
                      className={`form-control ${errors.password ? 'is-invalid' : ''}`}
                      placeholder="Min. 8 characters"
                      value={form.password}
                      onChange={set('password')}
                      autoComplete="new-password"
                    />
                    <button
                      type="button"
                      className="btn btn-outline-secondary"
                      onClick={() => setShowPw(p => !p)}
                      tabIndex={-1}
                    >
                      <i className={`bi bi-eye${showPw ? '-slash' : ''}`}></i>
                    </button>
                    {errors.password && <div className="invalid-feedback">{errors.password}</div>}
                  </div>
                  {form.password.length > 0 && (
                    <div className="mt-1">
                      <div className="progress" style={{ height: '4px' }}>
                        <div
                          className={`progress-bar bg-${pwStrength.color}`}
                          style={{ width: `${(pwStrength.level / 4) * 100}%` }}
                        />
                      </div>
                      <small className={`text-${pwStrength.color}`}>{pwStrength.label} password</small>
                    </div>
                  )}
                </div>

                {/* Phone */}
                <div className="col-sm-6">
                  <label className="form-label fw-semibold">Phone Number <span className="text-danger">*</span></label>
                  <PhoneInput
                    value={form.phoneNumber}
                    onChange={val => {
                      setForm(f => ({ ...f, phoneNumber: val }));
                      if (errors.phoneNumber) setErrors(prev => ({ ...prev, phoneNumber: undefined }));
                    }}
                    error={errors.phoneNumber}
                    required
                  />
                </div>

                {/* Birth Date */}
                <div className="col-sm-6">
                  <label className="form-label fw-semibold">Date of Birth <span className="text-danger">*</span></label>
                  <input
                    type="date"
                    className={`form-control ${errors.birthDate ? 'is-invalid' : ''}`}
                    value={form.birthDate}
                    onChange={set('birthDate')}
                    max={maxBirthDate}
                  />
                  {errors.birthDate && <div className="invalid-feedback">{errors.birthDate}</div>}
                </div>

                {/* Nationality */}
                <div className="col-sm-6">
                  <label className="form-label fw-semibold">Nationality <span className="text-muted small">(optional)</span></label>
                  <input
                    className="form-control"
                    placeholder="e.g. Turkish"
                    value={form.nationality}
                    onChange={set('nationality')}
                  />
                </div>

                {/* Notification */}
                <div className="col-12">
                  <label className="form-label fw-semibold">Notification Preference</label>
                  <select
                    className="form-select"
                    value={form.preferredNotificationChannel}
                    onChange={set('preferredNotificationChannel')}
                  >
                    <option value={1}>📧 Email</option>
                    <option value={2}>📱 SMS</option>
                    <option value={4}>💬 WhatsApp</option>
                    <option value={7}>🔔 All (Email + SMS + WhatsApp)</option>
                  </select>
                </div>

                <div className="col-12 mt-2">
                  <button
                    type="submit"
                    className="btn btn-primary w-100 btn-lg fw-semibold"
                    disabled={loading}
                  >
                    {loading
                      ? <><span className="spinner-border spinner-border-sm me-2" />Creating account...</>
                      : 'Create Account'}
                  </button>
                </div>

              </div>
            </form>

            <hr className="my-4" />
            <p className="text-center mb-0 text-muted">
              Already have an account?{' '}
              <Link to="/login" className="text-primary fw-semibold text-decoration-none">Sign In</Link>
            </p>
          </div>
        </div>
      </div>
    </div>
  );
}
