namespace FlightReservation.Messaging.Contracts;

public record UserRegisteredEvent(
    string UserId,
    string Email,
    string FullName,
    string ConfirmationToken,
    string ConfirmationLink
);
