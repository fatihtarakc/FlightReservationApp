using App.Web.Controllers;

namespace App.Web.Areas.Passenger.Controllers
{
    [Area("Passenger")]
    [Authorize]
    public class HomeController : BaseController
    {
        private readonly IBookingService _bookingService;
        private readonly IAccountService _accountService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HomeController(IBookingService bookingService, IAccountService accountService,
            IHttpContextAccessor httpContextAccessor)
        {
            _bookingService = bookingService;
            _accountService = accountService;
            _httpContextAccessor = httpContextAccessor;
        }

        private string? Token => TokenHelper.GetToken(_httpContextAccessor);

        public async Task<IActionResult> Index()
        {
            var token = Token;
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Account", new { area = "" });

            var result = await _bookingService.GetMyBookingsAsync(token);
            var bookings = result.IsSuccess ? result.Data ?? new() : new List<BookingVM>();

            var vm = new PassengerDashboardVM
            {
                UserName = TokenHelper.GetUserName(_httpContextAccessor) ?? "Yolcu",
                ActiveBookings = bookings.Where(b =>
                    b.Status == BookingStatus.Confirmed ||
                    b.Status == BookingStatus.Pending ||
                    b.Status == BookingStatus.CheckedIn ||
                    b.Status == BookingStatus.Boarded).ToList(),
                PastBookings = bookings.Where(b =>
                    b.Status == BookingStatus.Completed ||
                    b.Status == BookingStatus.Cancelled ||
                    b.Status == BookingStatus.NoShow).ToList()
            };
            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Settings()
        {
            var token = Token;
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Account", new { area = "" });

            var profileResult = await _accountService.GetProfileAsync(token);
            var profile = profileResult.IsSuccess && profileResult.Data != null ? profileResult.Data : new PassengerProfileVM
            {
                UserName = TokenHelper.GetUserName(_httpContextAccessor) ?? string.Empty
            };

            var vm = new PassengerSettingsPageVM
            {
                Profile = profile,
                UpdateForm = new PassengerProfileUpdateVM
                {
                    Name = profile.Name,
                    Surname = profile.Surname,
                    Email = profile.Email,
                    PhoneNumber = profile.PhoneNumber,
                    NotificationPreference = profile.NotificationPreference
                }
            };
            return View(vm);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(PassengerSettingsPageVM model)
        {
            var token = Token;
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Account", new { area = "" });

            var result = await _accountService.UpdateProfileAsync(model.UpdateForm, token);
            if (!result.IsSuccess)
                NotifyErrorLocalized(result.Message);
            else
                NotifySuccessLocalized(result.Message);
            return RedirectToAction(nameof(Settings));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(PassengerSettingsPageVM model)
        {
            var token = Token;
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Account", new { area = "" });

            var result = await _accountService.ChangePasswordAsync(model.PasswordForm, token);
            if (!result.IsSuccess)
                NotifyErrorLocalized(result.Message);
            else
                NotifySuccessLocalized(result.Message);
            return RedirectToAction(nameof(Settings));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateNotification(IFormCollection form)
        {
            var token = Token;
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Account", new { area = "" });

            var channels = form["channels"].Select(int.Parse).Aggregate(0, (a, b) => a | b);
            var result = await _accountService.UpdateNotificationPreferenceAsync((NotificationChannel)channels, token);
            if (!result.IsSuccess)
                NotifyErrorLocalized(result.Message);
            else
                NotifySuccessLocalized(result.Message);
            return RedirectToAction(nameof(Settings));
        }
    }
}
