import apiClient from './apiClient';
import type { ApiResponse, FlightDto, FlightListDto, FlightSearchDto, SeatDto } from '../types';

export const flightApi = {
  search: (data: FlightSearchDto) =>
    apiClient.post<ApiResponse<FlightListDto[]>>('flights/search', data),

  getById: (id: string) =>
    apiClient.get<ApiResponse<FlightDto>>(`flights/${id}`),

  getAvailableSeats: (flightId: string) =>
    apiClient.get<ApiResponse<SeatDto[]>>(`seats/available/${flightId}`),
};
