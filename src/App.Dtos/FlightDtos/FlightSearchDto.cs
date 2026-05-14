namespace App.Dtos.FlightDtos
{
    public class FlightSearchDto
    {
        public string DepartureIata { get; set; } = null!;
        public string ArrivalIata { get; set; } = null!;
        public DateTime DepartureDate { get; set; }
        public int Passengers { get; set; } = 1;
        public SeatClass SeatClass { get; set; } = SeatClass.Economy;
    }
}
