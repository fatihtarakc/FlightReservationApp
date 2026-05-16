using System.ComponentModel.DataAnnotations;
namespace App.Web.ViewModels.Passenger
{
    public class PassengerDashboardVM
    {
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public List<BookingVM> ActiveBookings { get; set; } = new();
        public List<BookingVM> PastBookings { get; set; } = new();
        public int TotalBookings => ActiveBookings.Count + PastBookings.Count;
        public int UpcomingCount => ActiveBookings.Count(b => b.Status == BookingStatus.Confirmed || b.Status == BookingStatus.CheckedIn);
    }

    public class PassengerProfileVM
    {
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("preferredNotificationChannel")]
        public NotificationChannel NotificationPreference { get; set; }
    }

    public class PassengerProfileUpdateVM
    {
        [Required] public string Name { get; set; } = string.Empty;
        [Required] public string Surname { get; set; } = string.Empty;
        [Required, EmailAddress] public string Email { get; set; } = string.Empty;
        [Required] public string PhoneNumber { get; set; } = string.Empty;
        public NotificationChannel NotificationPreference { get; set; }
    }

    public class ChangePasswordVM
    {
        [Required] public string CurrentPassword { get; set; } = string.Empty;
        [Required, MinLength(8)] public string NewPassword { get; set; } = string.Empty;
        [Required, Compare(nameof(NewPassword))] public string ConfirmPassword { get; set; } = string.Empty;
    }

    public class PassengerSettingsPageVM
    {
        public PassengerProfileVM Profile { get; set; } = new();
        public PassengerProfileUpdateVM UpdateForm { get; set; } = new();
        public ChangePasswordVM PasswordForm { get; set; } = new();
    }

    public class BookingCreatePageVM
    {
        public BookingAddVM Form { get; set; } = new();
        public FlightVM Flight { get; set; } = new();
        public SeatVM Seat { get; set; } = new();
        public decimal TotalPrice { get; set; }
    }

    public class BookingDetailPageVM
    {
        public BookingVM Booking { get; set; } = new();
        public bool CanCancel { get; set; }
        public bool CanCheckIn { get; set; }
    }
}
