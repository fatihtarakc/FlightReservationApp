export interface ApiResponse<T> {
  success: boolean;
  message?: string;
  data?: T;
}

// ── Auth models ───────────────────────────────────────────────────────────────
export interface TokenModel {
  accessToken: string;
  expiration: string;
}

export interface SignInRequest {
  usernameOrEmail: string;
  password: string;
}

export interface RegisterRequest {
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

export interface VerifyEmailRequest {
  email: string;
  code: string;
}

export interface ResetPasswordRequest {
  email: string;
  code: string;
  newPassword: string;
}

// ── Enums — integer values mirror App.Entity exactly ─────────────────────────
export enum SeatClass {
  Economy        = 1,
  PremiumEconomy = 2,
  Business       = 3,
  First          = 4,
}

export enum SeatColumn {
  A = 1, B = 2, C = 3, D = 4, E = 5, F = 6,
  G = 7, H = 8, J = 9, K = 10,
}

export enum BodyType {
  NarrowBody  = 1,
  WideBody    = 2,
  RegionalJet = 3,
  Turboprop   = 4,
}

export enum FlightStatus {
  Scheduled = 1,
  Boarding  = 2,
  Departed  = 3,
  InAir     = 4,
  Landed    = 5,
  Arrived   = 6,
  Delayed   = 7,
  Cancelled = 8,
  Diverted  = 9,
}

export enum BookingStatus {
  Pending   = 1,
  Confirmed = 2,
  Cancelled = 3,
  CheckedIn = 4,
  Boarded   = 5,
  Completed = 6,
  NoShow    = 7,
}

export enum UserStatus {
  Pending     = 1,
  Active      = 2,
  Suspended   = 3,
  Deactivated = 4,
}

export enum Currency {
  TRY = 1,
  USD = 2,
  EUR = 3,
  GBP = 4,
  AED = 5,
}

export enum NotificationChannel {
  None     = 0,
  Email    = 1,
  Sms      = 2,
  WhatsApp = 4,
  All      = 7,
}

// ── Flight models ─────────────────────────────────────────────────────────────
export interface FlightListItem {
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
  currency: Currency;
  flightStatus: FlightStatus;
  availableSeats: number;
}

export interface FlightDetails extends FlightListItem {
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

export interface FlightSearchParams {
  departureIata: string;
  arrivalIata: string;
  departureDate: string;
  passengers: number;
  seatClass: SeatClass;
}

// ── Seat models ───────────────────────────────────────────────────────────────
export interface SeatModel {
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

// ── Booking models ────────────────────────────────────────────────────────────
export interface BookingRequest {
  flightId: string;
  seatId: string;
}

export interface BookingListItem {
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

export interface BookingDetails extends BookingListItem {
  currency: Currency;
  checkInTime?: string;
  boardingPassNumber?: string;
  appUserId: string;
  passengerName: string;
  flightId: string;
  createdDate: string;
}

// ── Admin models ──────────────────────────────────────────────────────────────
export interface AdminDashboard {
  totalFlights: number;
  scheduledFlights: number;
  cancelledFlights: number;
  totalBookings: number;
  confirmedBookings: number;
  cancelledBookings: number;
  checkedInPassengers: number;
  totalRevenue: number;
  averageOccupancyRate: number;
}

export interface FlightPassengerStat {
  flightId: string;
  flightNumber: string;
  departureCity: string;
  arrivalCity: string;
  departureDateTime: string;
  totalSeats: number;
  bookedSeats: number;
  occupancyRate: number;
}

export interface AdminBookingItem {
  id: string;
  pnrNumber: string;
  passengerName: string;
  flightNumber: string;
  departureDateTime: string;
  departureCity: string;
  arrivalCity: string;
  seatNumber: string;
  seatClass: SeatClass;
  totalPrice: number;
  currency: Currency;
  bookingStatus: BookingStatus;
  createdDate: string;
}

// ── Display labels ────────────────────────────────────────────────────────────
export const SeatClassLabels: Record<SeatClass, string> = {
  [SeatClass.Economy]:        'Economy',
  [SeatClass.PremiumEconomy]: 'Premium Economy',
  [SeatClass.Business]:       'Business',
  [SeatClass.First]:          'First Class',
};

export const SeatColumnLabels: Record<SeatColumn, string> = {
  [SeatColumn.A]: 'A', [SeatColumn.B]: 'B', [SeatColumn.C]: 'C',
  [SeatColumn.D]: 'D', [SeatColumn.E]: 'E', [SeatColumn.F]: 'F',
  [SeatColumn.G]: 'G', [SeatColumn.H]: 'H', [SeatColumn.J]: 'J',
  [SeatColumn.K]: 'K',
};

export const FlightStatusLabels: Record<FlightStatus, string> = {
  [FlightStatus.Scheduled]: 'Scheduled',
  [FlightStatus.Boarding]:  'Boarding',
  [FlightStatus.Departed]:  'Departed',
  [FlightStatus.InAir]:     'In Air',
  [FlightStatus.Landed]:    'Landed',
  [FlightStatus.Arrived]:   'Arrived',
  [FlightStatus.Delayed]:   'Delayed',
  [FlightStatus.Cancelled]: 'Cancelled',
  [FlightStatus.Diverted]:  'Diverted',
};

export const BookingStatusLabels: Record<BookingStatus, string> = {
  [BookingStatus.Pending]:   'Pending',
  [BookingStatus.Confirmed]: 'Confirmed',
  [BookingStatus.Cancelled]: 'Cancelled',
  [BookingStatus.CheckedIn]: 'Checked In',
  [BookingStatus.Boarded]:   'Boarded',
  [BookingStatus.Completed]: 'Completed',
  [BookingStatus.NoShow]:    'No Show',
};

export const CurrencyLabels: Record<Currency, string> = {
  [Currency.TRY]: 'TRY',
  [Currency.USD]: 'USD',
  [Currency.EUR]: 'EUR',
  [Currency.GBP]: 'GBP',
  [Currency.AED]: 'AED',
};
