import { api } from './apiClient';
import type { ApiResponse, FlightDetails, FlightListItem, FlightSearchParams, SeatModel } from '../types';

export const flightApi = {
  getAll: () =>
    api.get<ApiResponse<FlightListItem[]>>('flight'),

  search: (data: FlightSearchParams) => {
    const params = new URLSearchParams({
      departureIata: data.departureIata,
      arrivalIata:   data.arrivalIata,
      departureDate: data.departureDate,
      passengers:    String(data.passengers),
      seatClass:     String(data.seatClass),
    });
    return api.get<ApiResponse<FlightListItem[]>>(`flight/search?${params}`);
  },

  getById: (id: string) =>
    api.get<ApiResponse<FlightDetails>>(`flight/${id}`),

  getAllSeats: (flightId: string) =>
    api.get<ApiResponse<SeatModel[]>>(`seat/flight/${flightId}`),
};
