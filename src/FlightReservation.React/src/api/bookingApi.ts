import { api } from './apiClient';
import type { ApiResponse, BookingRequest, BookingDetails, BookingListItem } from '../types';

export const bookingApi = {
  myBookings: ()                     => api.get<ApiResponse<BookingListItem[]>>('booking/my-bookings'),
  getById:    (id: string)           => api.get<ApiResponse<BookingDetails>>(`booking/${id}`),
  book:       (data: BookingRequest) => api.post<ApiResponse<BookingDetails>>('booking', data),
  cancel:     (id: string)           => api.post<ApiResponse<object>>(`booking/${id}/cancel`, {}),
};
