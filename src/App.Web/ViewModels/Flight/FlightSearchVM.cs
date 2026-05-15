using App.Web.Enums;
namespace App.Web.ViewModels.Flight
{
    public class FlightSearchVM
    {
        public string DepartureIata { get; set; } = null!;
        public string ArrivalIata { get; set; } = null!;
        public DateTime DepartureDate { get; set; } = DateTime.Today;
        public int Passengers { get; set; } = 1;
        public SeatClass SeatClass { get; set; } = SeatClass.Economy;
    }
}
