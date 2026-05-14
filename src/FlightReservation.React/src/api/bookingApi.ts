import { api } from './apiClient';
import type { ApiResponse, BookingAddDto, BookingDto, BookingListDto } from '../types';

export const bookingApi = {
  myBookings: ()               => api.get<ApiResponse<BookingListDto[]>>('booking/my-bookings'),
  getById:    (id: string)     => api.get<ApiResponse<BookingDto>>(`booking/${id}`),
  book:       (data: BookingAddDto) => api.post<ApiResponse<BookingDto>>('booking', data),
  cancel:     (id: string)     => api.delete<ApiResponse<object>>(`booking/${id}/cancel`),
};
