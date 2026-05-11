namespace FlightReservation.Messaging.Contracts;

public record ReservationCancelledEvent(
    int ReservationId,
    string PnrCode,
    string UserEmail,
    string UserFullName,
    string FlightInfo,
    string CancelReason
);
