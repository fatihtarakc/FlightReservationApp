namespace App.Dtos.FlightDtos
{
    public class FlightAddDto
    {
        public string Number { get; set; } = null!;
        public DateTime DepartureDateTime { get; set; }
        public DateTime ArrivalDateTime { get; set; }
        public decimal BaseEconomyPrice { get; set; }
        public decimal BasePremiumEconomyPrice { get; set; }
        public decimal BaseBusinessPrice { get; set; }
        public decimal BaseFirstClassPrice { get; set; }
        public Currency Currency { get; set; }
        public string? Gate { get; set; }
        public string? Terminal { get; set; }
        public Guid AircraftId { get; set; }
        public Guid AirlineId { get; set; }
        public Guid ScheduleId { get; set; }
    }
}
