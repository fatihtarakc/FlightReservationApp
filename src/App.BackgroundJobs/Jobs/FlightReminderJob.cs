namespace App.BackgroundJobs.Jobs
{
    public class FlightReminderJob
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<FlightReminderJob> _logger;

        public FlightReminderJob(
            IBookingRepository bookingRepository,
            IUnitOfWork unitOfWork,
            IPublishEndpoint publishEndpoint,
            ILogger<FlightReminderJob> logger)
        {
            _bookingRepository = bookingRepository;
            _unitOfWork = unitOfWork;
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        [AutomaticRetry(Attempts = 3, DelaysInSeconds = new[] { 30, 60, 120 })]
        public async Task Send7DayRemindersAsync()
        {
            _logger.LogInformation("Starting 7-day flight reminder job at {Time}", DateTime.UtcNow);
            await SendRemindersAsync(168, is7DayReminder: true);
        }

        [AutomaticRetry(Attempts = 3, DelaysInSeconds = new[] { 30, 60, 120 })]
        public async Task Send24HourRemindersAsync()
        {
            _logger.LogInformation("Starting 24-hour flight reminder job at {Time}", DateTime.UtcNow);
            await SendRemindersAsync(24, is7DayReminder: false);
        }

        private async Task SendRemindersAsync(int hoursBeforeDeparture, bool is7DayReminder)
        {
            var bookings = await _bookingRepository.GetPendingRemindersAsync(hoursBeforeDeparture);
            var bookingList = bookings.ToList();

            _logger.LogInformation("Found {Count} bookings pending {ReminderType} reminder",
                bookingList.Count, is7DayReminder ? "7-day" : "24-hour");

            foreach (var booking in bookingList)
            {
                try
                {
                    var reminderEvent = new FlightReminderEvent
                    {
                        BookingId = booking.Id,
                        PnrNumber = booking.PnrNumber,
                        PassengerName = $"{booking.AppUser.Name} {booking.AppUser.Surname}",
                        Email = booking.AppUser.Email!,
                        PhoneNumber = booking.AppUser.PhoneNumber!,
                        FlightNumber = booking.Flight.Number,
                        DepartureDateTime = booking.Flight.DepartureDateTime,
                        DepartureAirport = booking.Flight.Schedule.Route.DepartureAirport.IataCode,
                        ArrivalAirport = booking.Flight.Schedule.Route.ArrivalAirport.IataCode,
                        PreferredChannel = booking.AppUser.PreferredNotificationChannel,
                        Is7DayReminder = is7DayReminder
                    };

                    await _publishEndpoint.Publish(reminderEvent);

                    if (is7DayReminder)
                        booking.IsReminderSent7Days = true;
                    else
                        booking.IsReminderSent24Hours = true;

                    await _bookingRepository.UpdateAsync(booking);

                    _logger.LogInformation("Published reminder event for BookingId: {BookingId} PNR: {Pnr}",
                        booking.Id, booking.PnrNumber);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send {ReminderType} reminder for BookingId: {BookingId}",
                        is7DayReminder ? "7-day" : "24-hour", booking.Id);
                }
            }

            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Completed {ReminderType} reminder job. Processed: {Count}",
                is7DayReminder ? "7-day" : "24-hour", bookingList.Count);
        }
    }
}
