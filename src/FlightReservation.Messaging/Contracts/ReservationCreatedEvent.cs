namespace FlightReservation.Messaging.Contracts;

public record ReservationCreatedEvent(
    int ReservationId,
    string PnrCode,
    string UserEmail,
    string UserFullName,
    string FlightNumber,
    string RouteInfo,
    string SeatNumber,
    DateTime DepartureUtc
);
