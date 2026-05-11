namespace FlightReservation.Infrastructure.Email;

public static class EmailTemplates
{
    private static string Wrap(string title, string body) =>
        $$"""
        <!DOCTYPE html>
        <html lang="tr">
        <head><meta charset="UTF-8"><style>
          body { font-family: Arial, sans-serif; background:#f4f4f4; margin:0; padding:0; }
          .container { max-width:600px; margin:30px auto; background:#fff; border-radius:8px; overflow:hidden; box-shadow:0 2px 8px rgba(0,0,0,.1); }
          .header { background:#1a3c6e; color:#fff; padding:24px 32px; }
          .header h1 { margin:0; font-size:22px; }
          .content { padding:32px; color:#333; line-height:1.6; }
          .pnr-box { background:#f0f7ff; border:2px solid #1a3c6e; border-radius:6px; padding:16px; text-align:center; margin:20px 0; font-size:28px; font-weight:bold; letter-spacing:6px; color:#1a3c6e; }
          .btn { display:inline-block; background:#1a3c6e; color:#fff; padding:12px 28px; border-radius:6px; text-decoration:none; font-weight:bold; margin-top:16px; }
          .footer { background:#f4f4f4; padding:16px 32px; font-size:12px; color:#888; text-align:center; }
        </style></head>
        <body>
          <div class="container">
            <div class="header"><h1>&#x2708; Flight Reservation &mdash; {{title}}</h1></div>
            <div class="content">{{body}}</div>
            <div class="footer">&copy; 2026 Flight Reservation System. T&uuml;m haklar&#x131; sakl&#x131;d&#x131;r.</div>
          </div>
        </body></html>
        """;

    public static string ReservationConfirmation(string name, string pnr, string flightInfo) =>
        Wrap("Rezervasyon Onayı",
            $"<p>Sayın <strong>{name}</strong>,</p>" +
            "<p>Rezervasyonunuz başarıyla oluşturulmuştur.</p>" +
            $"<div class=\"pnr-box\">{pnr}</div>" +
            $"<p><strong>Uçuş Bilgileri:</strong></p><p>{flightInfo}</p>" +
            "<p>PNR kodunuzu havalimanında görevlilere gösteriniz. İyi yolculuklar dileriz!</p>");

    public static string ReservationCancellation(string name, string pnr, string reason) =>
        Wrap("Rezervasyon İptali",
            $"<p>Sayın <strong>{name}</strong>,</p>" +
            $"<p><strong>{pnr}</strong> PNR kodlu rezervasyonunuz iptal edilmiştir.</p>" +
            $"<p><strong>İptal Nedeni:</strong> {reason}</p>" +
            "<p>Farklı bir uçuş için rezervasyon oluşturmak isterseniz sitemizi ziyaret edebilirsiniz.</p>");

    public static string FlightCancellation(string name, string flightNumber, string flightInfo) =>
        Wrap("Sefer İptali",
            $"<p>Sayın <strong>{name}</strong>,</p>" +
            $"<p><strong>{flightNumber}</strong> sefer numaralı uçuşunuz iptal edilmiştir.</p>" +
            $"<p>{flightInfo}</p>" +
            "<p>Mağduriyetiniz için özür dileriz. Destek için bizimle iletişime geçebilirsiniz.</p>");

    public static string Welcome(string name, string confirmationLink) =>
        Wrap("Hoş Geldiniz",
            $"<p>Sayın <strong>{name}</strong>,</p>" +
            "<p>Flight Reservation'a hoş geldiniz! Hesabınızı etkinleştirmek için aşağıdaki butona tıklayın.</p>" +
            $"<a class=\"btn\" href=\"{confirmationLink}\">E-postamı Doğrula</a>" +
            "<p style=\"margin-top:16px;font-size:13px;color:#888;\">Bu link 24 saat geçerlidir.</p>");

    public static string PasswordReset(string name, string resetLink) =>
        Wrap("Şifre Sıfırlama",
            $"<p>Sayın <strong>{name}</strong>,</p>" +
            "<p>Şifre sıfırlama talebiniz alındı. Yeni şifrenizi oluşturmak için butona tıklayın.</p>" +
            $"<a class=\"btn\" href=\"{resetLink}\">Şifremi Sıfırla</a>" +
            "<p style=\"margin-top:16px;font-size:13px;color:#888;\">Bu link 2 saat geçerlidir. Talepte bulunmadıysanız bu e-postayı dikkate almayın.</p>");

    public static string FlightReminder(string name, string pnr, string flightInfo, DateTime departureTime) =>
        Wrap("Uçuş Hatırlatması",
            $"<p>Sayın <strong>{name}</strong>,</p>" +
            $"<p>Yarın saat <strong>{departureTime:HH:mm}</strong>'de uçuşunuz var.</p>" +
            $"<div class=\"pnr-box\">{pnr}</div>" +
            $"<p>{flightInfo}</p>" +
            "<p>Havalimanına en az 2 saat erken gelmenizi öneririz. İyi yolculuklar!</p>");
}
