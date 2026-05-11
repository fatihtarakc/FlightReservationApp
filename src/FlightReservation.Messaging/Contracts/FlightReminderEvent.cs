namespace FlightReservation.Messaging.Contracts;

public record FlightReminderEvent(
    string UserEmail,
    string UserFullName,
    string PnrCode,
    string FlightNumber,
    string RouteInfo,
    string SeatNumber,
    DateTime DepartureUtc
);
