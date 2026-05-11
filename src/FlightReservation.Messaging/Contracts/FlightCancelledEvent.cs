namespace FlightReservation.Messaging.Contracts;

public record FlightCancelledEvent(
    int FlightId,
    string FlightNumber,
    string RouteInfo,
    DateTime DepartureUtc,
    IReadOnlyList<AffectedPassenger> AffectedPassengers
);

public record AffectedPassenger(
    string UserEmail,
    string UserFullName,
    string PnrCode
);
