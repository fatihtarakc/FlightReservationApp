namespace FlightReservation.Infrastructure.Email;

public interface IEmailService
{
    Task SendReservationConfirmationAsync(string toEmail, string toName, string pnr, string flightInfo);
    Task SendReservationCancellationAsync(string toEmail, string toName, string pnr, string reason);
    Task SendFlightCancellationAsync(string toEmail, string toName, string flightNumber, string flightInfo);
    Task SendWelcomeEmailAsync(string toEmail, string toName, string confirmationLink);
    Task SendPasswordResetAsync(string toEmail, string toName, string resetLink);
    Task SendFlightReminderAsync(string toEmail, string toName, string pnr, string flightInfo, DateTime departureTime);
    Task SendEmailAsync(string toEmail, string toName, string subject, string htmlBody);
}
