namespace FlightReservation.Infrastructure.Cache;

public static class CacheKeys
{
    public const string ActiveRoutes = "routes:active";
    public static string FlightSearch(string origin, string dest, string date) => $"flights:search:{origin}:{dest}:{date}";
    public static string SeatMap(int flightId) => $"seatmap:{flightId}";
    public static string SeatHold(int flightId, int seatId) => $"seat:hold:{flightId}:{seatId}";
    public const string DashboardStats = "dashboard:stats";
    public static string FlightDetail(int flightId) => $"flight:detail:{flightId}";
}
