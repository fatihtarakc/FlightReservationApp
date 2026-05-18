namespace App.Web.ViewModels.Admin
{
    public class AdminFlightListPageVM
    {
        public List<FlightVM> Flights { get; set; } = new();
        public string? Search { get; set; }
        public int? StatusFilter { get; set; }
        public string? DateFilter { get; set; }
    }

    public class AdminFlightFormPageVM
    {
        public FlightAddVM Form { get; set; } = new();
        public List<ScheduleSelectItemVM> Schedules { get; set; } = new();
        public List<AircraftVM> Aircraft { get; set; } = new();
        public Guid? EditId { get; set; }
    }

    public class AdminRouteListPageVM
    {
        public List<RouteVM> Routes { get; set; } = new();
    }

    public class AdminRouteFormPageVM
    {
        public RouteAddVM Form { get; set; } = new();
        public List<AirportVM> Airports { get; set; } = new();
        public Guid? EditId { get; set; }
    }

    public class AdminRouteEditPageVM
    {
        public RouteUpdateVM Form { get; set; } = new();
        public List<AirportVM> Airports { get; set; } = new();
        public Guid EditId { get; set; }
    }

    public class AdminAircraftListPageVM
    {
        public List<AircraftVM> Aircraft { get; set; } = new();
    }

    public class AdminAirportListPageVM
    {
        public List<AirportVM> Airports { get; set; } = new();
        public string? Search { get; set; }
    }

    public class AdminBookingListPageVM
    {
        public List<BookingVM> Bookings { get; set; } = new();
        public string? Search { get; set; }
        public int? StatusFilter { get; set; }
        public string? DateFilter { get; set; }
    }

    public class AdminNotificationListPageVM
    {
        public List<NotificationLogVM> Notifications { get; set; } = new();
        public string? Search { get; set; }
        public string? Channel { get; set; }
        public string? DateFilter { get; set; }
        public int EmailCount => Notifications.Count(x => x.Channel == "Email" && x.IsSuccess);
        public int SmsCount => Notifications.Count(x => x.Channel == "Sms" && x.IsSuccess);
        public int WhatsAppCount => Notifications.Count(x => x.Channel == "WhatsApp" && x.IsSuccess);
    }

    public class AdminLogListPageVM
    {
        public List<AppLogVM> Logs { get; set; } = new();
        public string? Search { get; set; }
        public string? Level { get; set; }
        public string? DateFilter { get; set; }
    }

    public class AdminHangfirePageVM
    {
        public HangfireStatsVM Stats { get; set; } = new();
    }

    public class AdminRouteDetailPageVM
    {
        public RouteVM Route { get; set; } = new();
        public AirportVM? Origin { get; set; }
        public AirportVM? Destination { get; set; }
    }

    public class AdminUserListPageVM
    {
        public List<AdminUserVM> Users { get; set; } = new();
        public string? Search { get; set; }
        public string? RoleFilter { get; set; }
        public bool? ActiveFilter { get; set; }
        public int TotalUsers => Users.Count;
        public int ActiveCount => Users.Count(u => u.IsActive);
        public int PassiveCount => Users.Count(u => !u.IsActive);
    }

    public class AdminScheduleListPageVM
    {
        public List<ScheduleVM> Schedules { get; set; } = new();
        public string? Search { get; set; }
    }

    public class AdminScheduleFormPageVM
    {
        public ScheduleAddVM Form { get; set; } = new();
        public List<RouteVM> Routes { get; set; } = new();
    }

    public class AdminScheduleEditPageVM
    {
        public ScheduleUpdateVM Form { get; set; } = new();
        public Guid EditId { get; set; }
        public string RouteName { get; set; } = string.Empty;
    }
}
