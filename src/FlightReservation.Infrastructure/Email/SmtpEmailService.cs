using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using static MassTransit.Monitoring.Performance.BuiltInCounters;

namespace FlightReservation.Infrastructure.Email;

public class SmtpSettings
{
    public string From { get; set; }
    public string EmailFrom { get; set; }
    public string SmtpServer { get; set; }
    public int Port { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
}

public class SmtpEmailService : IEmailService
{
    private readonly SmtpSettings _settings;

    public SmtpEmailService(IOptions<SmtpSettings> options) => _settings = options.Value;

    public async Task SendEmailAsync(string toEmail, string toName, string subject, string htmlBody)
    {
        MimeMessage mimeMessage = new();
        mimeMessage.From.Add(new MailboxAddress(_settings.From, _settings.EmailFrom));
        mimeMessage.To.Add(new MailboxAddress(toName, toEmail));
        mimeMessage.Subject = subject;

        var bodyBuilder = new BodyBuilder { HtmlBody = htmlBody };
        mimeMessage.Body = bodyBuilder.ToMessageBody();

        using SmtpClient smtpClient = new();
        await smtpClient.ConnectAsync(_settings.SmtpServer, _settings.Port, SecureSocketOptions.StartTls);
        await smtpClient.AuthenticateAsync(_settings.Username, _settings.Password);
        await smtpClient.SendAsync(mimeMessage);
        await smtpClient.DisconnectAsync(true);
    }

    public Task SendReservationConfirmationAsync(string toEmail, string toName, string pnr, string flightInfo) =>
        SendEmailAsync(toEmail, toName,
            $"Rezervasyon Onayı - PNR: {pnr}",
            EmailTemplates.ReservationConfirmation(toName, pnr, flightInfo));

    public Task SendReservationCancellationAsync(string toEmail, string toName, string pnr, string reason) =>
        SendEmailAsync(toEmail, toName,
            $"Rezervasyon İptali - PNR: {pnr}",
            EmailTemplates.ReservationCancellation(toName, pnr, reason));

    public Task SendFlightCancellationAsync(string toEmail, string toName, string flightNumber, string flightInfo) =>
        SendEmailAsync(toEmail, toName,
            $"Sefer İptali - {flightNumber}",
            EmailTemplates.FlightCancellation(toName, flightNumber, flightInfo));

    public Task SendWelcomeEmailAsync(string toEmail, string toName, string confirmationLink) =>
        SendEmailAsync(toEmail, toName,
            "Hoş Geldiniz! E-posta Adresinizi Doğrulayın",
            EmailTemplates.Welcome(toName, confirmationLink));

    public Task SendPasswordResetAsync(string toEmail, string toName, string resetLink) =>
        SendEmailAsync(toEmail, toName,
            "Şifre Sıfırlama Talebi",
            EmailTemplates.PasswordReset(toName, resetLink));

    public Task SendFlightReminderAsync(string toEmail, string toName, string pnr, string flightInfo, DateTime departureTime) =>
        SendEmailAsync(toEmail, toName,
            $"Uçuş Hatırlatması - {departureTime:dd MMM HH:mm}",
            EmailTemplates.FlightReminder(toName, pnr, flightInfo, departureTime));
}
