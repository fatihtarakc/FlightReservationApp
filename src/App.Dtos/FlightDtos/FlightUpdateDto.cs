namespace App.Dtos.FlightDtos
{
    public class FlightUpdateDto
    {
        public Guid Id { get; set; }
        public decimal BaseEconomyPrice { get; set; }
        public decimal BasePremiumEconomyPrice { get; set; }
        public decimal BaseBusinessPrice { get; set; }
        public decimal BaseFirstClassPrice { get; set; }
        public FlightStatus FlightStatus { get; set; }
        public string? Gate { get; set; }
        public string? Terminal { get; set; }
        public string? CancellationReason { get; set; }
    }
}
