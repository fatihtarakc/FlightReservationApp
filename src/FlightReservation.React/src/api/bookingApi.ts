import apiClient from './apiClient';
import type { ApiResponse, BookingAddDto, BookingDto, BookingListDto } from '../types';

export const bookingApi = {
  myBookings: () =>
    apiClient.get<ApiResponse<BookingListDto[]>>('bookings/my-bookings'),

  getById: (id: string) =>
    apiClient.get<ApiResponse<BookingDto>>(`bookings/${id}`),

  book: (data: BookingAddDto) =>
    apiClient.post<ApiResponse<BookingDto>>('bookings', data),

  cancel: (id: string) =>
    apiClient.delete<ApiResponse<object>>(`bookings/${id}/cancel`),
};
