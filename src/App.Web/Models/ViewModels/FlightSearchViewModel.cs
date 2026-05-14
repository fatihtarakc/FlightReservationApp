namespace App.Web.Models.ViewModels
{
    public class FlightSearchViewModel
    {
        public string DepartureIata { get; set; } = string.Empty;
        public string ArrivalIata { get; set; } = string.Empty;
        public DateTime DepartureDate { get; set; } = DateTime.Today.AddDays(1);
        public int Passengers { get; set; } = 1;
        public SeatClass SeatClass { get; set; } = SeatClass.Economy;
        public List<FlightListDto> Results { get; set; } = new();
    }
}
