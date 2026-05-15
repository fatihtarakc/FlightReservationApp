import { BrowserRouter, Routes, Route } from 'react-router-dom';
import { AuthProvider } from './context/AuthContext';
import Navbar from './components/Navbar';
import ProtectedRoute from './components/ProtectedRoute';
import AdminRoute from './components/AdminRoute';

import HomePage from './pages/HomePage';
import LoginPage from './pages/LoginPage';
import RegisterPage from './pages/RegisterPage';
import VerifyEmailPage from './pages/VerifyEmailPage';
import ForgotPasswordPage from './pages/ForgotPasswordPage';
import ResetPasswordPage from './pages/ResetPasswordPage';
import FlightSearchPage from './pages/FlightSearchPage';
import FlightDetailsPage from './pages/FlightDetailsPage';
import BookPage from './pages/BookPage';
import MyBookingsPage from './pages/MyBookingsPage';
import BookingDetailsPage from './pages/BookingDetailsPage';
import AdminDashboardPage from './pages/AdminDashboardPage';
import AdminBookingsPage from './pages/AdminBookingsPage';

export default function App() {
  return (
    <AuthProvider>
      <BrowserRouter>
        <Navbar />
        <main className="py-4">
          <div className="container">
            <Routes>
              <Route path="/" element={<HomePage />} />
              <Route path="/login" element={<LoginPage />} />
              <Route path="/register" element={<RegisterPage />} />
              <Route path="/verify-email" element={<VerifyEmailPage />} />
              <Route path="/forgot-password" element={<ForgotPasswordPage />} />
              <Route path="/reset-password" element={<ResetPasswordPage />} />
              <Route path="/flights/search" element={<FlightSearchPage />} />
              <Route path="/flights/:id" element={<FlightDetailsPage />} />
              <Route
                path="/flights/:flightId/book"
                element={<ProtectedRoute><BookPage /></ProtectedRoute>}
              />
              <Route
                path="/bookings"
                element={<ProtectedRoute><MyBookingsPage /></ProtectedRoute>}
              />
              <Route
                path="/bookings/:id"
                element={<ProtectedRoute><BookingDetailsPage /></ProtectedRoute>}
              />
              <Route
                path="/admin"
                element={<AdminRoute><AdminDashboardPage /></AdminRoute>}
              />
              <Route
                path="/admin/bookings"
                element={<AdminRoute><AdminBookingsPage /></AdminRoute>}
              />
            </Routes>
          </div>
        </main>
        <footer className="bg-dark text-light py-4 mt-5">
          <div className="container text-center">
            <p className="mb-0 text-muted">
              &copy; {new Date().getFullYear()} FlightReservation. All rights reserved.
            </p>
          </div>
        </footer>
      </BrowserRouter>
    </AuthProvider>
  );
}
