import { api } from './apiClient';
import type { ApiResponse, AdminDashboard, FlightPassengerStat, AdminBookingItem } from '../types';

export const adminApi = {
  getDashboard:            () => api.get<ApiResponse<AdminDashboard>>('admin/dashboard'),
  getFlightPassengerStats: () => api.get<ApiResponse<FlightPassengerStat[]>>('admin/flight-passenger-stats'),
  getAllBookings:           () => api.get<ApiResponse<AdminBookingItem[]>>('booking'),
};
