export interface ApiResponse<T> {
  success: boolean;
  message?: string;
  data?: T;
}

export interface TokenDto {
  accessToken: string;
  expiration: string;
}

export interface SignInDto {
  email: string;
  password: string;
}

export interface RegisterDto {
  name: string;
  surname: string;
  username: string;
  email: string;
  password: string;
  phoneNumber: string;
  birthDate: string;
  preferredNotificationChannel: number;
  nationality?: string;
}

export interface VerifyEmailDto {
  email: string;
  code: string;
}

export interface ResetPasswordDto {
  email: string;
  code: string;
  newPassword: string;
}

export enum SeatClass {
  Economy = 0,
  PremiumEconomy = 1,
  Business = 2,
  FirstClass = 3,
}

export enum FlightStatus {
  Scheduled = 0,
  Delayed = 1,
  Boarding = 2,
  Departed = 3,
  Arrived = 4,
  Cancelled = 5,
}

export enum BookingStatus {
  Confirmed = 0,
  Cancelled = 1,
  CheckedIn = 2,
  NoShow = 3,
}

export enum SeatColumn {
  A = 0,
  B = 1,
  C = 2,
  D = 3,
  E = 4,
  F = 5,
}

export interface FlightListDto {
  id: string;
  number: string;
  departureDateTime: string;
  arrivalDateTime: string;
  airlineName: string;
  departureAirportIata: string;
  departureCity: string;
  arrivalAirportIata: string;
  arrivalCity: string;
  baseEconomyPrice: number;
  currency: string;
  flightStatus: FlightStatus;
  availableSeats: number;
}

export interface FlightDto extends FlightListDto {
  duration: string;
  basePremiumEconomyPrice: number;
  baseBusinessPrice: number;
  baseFirstClassPrice: number;
  airlineIata: string;
  aircraftTailNumber: string;
  modelName: string;
  gate?: string;
  terminal?: string;
  availableEconomySeats: number;
  availableBusinessSeats: number;
  availableFirstClassSeats: number;
}

export interface FlightSearchDto {
  departureIata: string;
  arrivalIata: string;
  departureDate: string;
  passengers: number;
  seatClass: SeatClass;
}

export interface SeatDto {
  id: string;
  row: number;
  column: SeatColumn;
  seatClass: SeatClass;
  isWindowSeat: boolean;
  isAisleSeat: boolean;
  hasExtraLegRoom: boolean;
  aircraftId: string;
  isAvailable: boolean;
}

export interface BookingAddDto {
  flightId: string;
  seatId: string;
}

export interface BookingListDto {
  id: string;
  pnrNumber: string;
  flightNumber: string;
  departureDateTime: string;
  departureCity: string;
  arrivalCity: string;
  seatNumber: string;
  seatClass: SeatClass;
  totalPrice: number;
  bookingStatus: BookingStatus;
}

export interface BookingDto extends BookingListDto {
  currency: string;
  checkInTime?: string;
  boardingPassNumber?: string;
  appUserId: string;
  passengerName: string;
  flightId: string;
  createdDate: string;
  arrivalCity: string;
}

export const SeatClassLabels: Record<SeatClass, string> = {
  [SeatClass.Economy]: 'Economy',
  [SeatClass.PremiumEconomy]: 'Premium Economy',
  [SeatClass.Business]: 'Business',
  [SeatClass.FirstClass]: 'First Class',
};

export const SeatColumnLabels: Record<SeatColumn, string> = {
  [SeatColumn.A]: 'A',
  [SeatColumn.B]: 'B',
  [SeatColumn.C]: 'C',
  [SeatColumn.D]: 'D',
  [SeatColumn.E]: 'E',
  [SeatColumn.F]: 'F',
};

export const FlightStatusLabels: Record<FlightStatus, string> = {
  [FlightStatus.Scheduled]: 'Scheduled',
  [FlightStatus.Delayed]: 'Delayed',
  [FlightStatus.Boarding]: 'Boarding',
  [FlightStatus.Departed]: 'Departed',
  [FlightStatus.Arrived]: 'Arrived',
  [FlightStatus.Cancelled]: 'Cancelled',
};

export const BookingStatusLabels: Record<BookingStatus, string> = {
  [BookingStatus.Confirmed]: 'Confirmed',
  [BookingStatus.Cancelled]: 'Cancelled',
  [BookingStatus.CheckedIn]: 'Checked In',
  [BookingStatus.NoShow]: 'No Show',
};
