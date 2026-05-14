import { api } from './apiClient';
import type { ApiResponse, FlightDto, FlightListDto, FlightSearchDto, SeatDto } from '../types';

export const flightApi = {
  getAll: () =>
    api.get<ApiResponse<FlightListDto[]>>('flight'),

  search: (data: FlightSearchDto) => {
    const params = new URLSearchParams({
      departureIata: data.departureIata,
      arrivalIata:   data.arrivalIata,
      departureDate: data.departureDate,
      passengers:    String(data.passengers),
      seatClass:     String(data.seatClass),
    });
    return api.get<ApiResponse<FlightListDto[]>>(`flight/search?${params}`);
  },

  getById: (id: string) =>
    api.get<ApiResponse<FlightDto>>(`flight/${id}`),

  getAvailableSeats: (flightId: string) =>
    api.get<ApiResponse<SeatDto[]>>(`seat/available/${flightId}`),
};
