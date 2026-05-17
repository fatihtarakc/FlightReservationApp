namespace App.Dtos.ModelDtos
{
    public class ModelUpdateDto
    {
        public string Name { get; set; } = null!;
        public BodyType BodyType { get; set; }
        public int MaxPassengerCapacity { get; set; }
        public int EconomySeats { get; set; }
        public int PremiumEconomySeats { get; set; }
        public int BusinessSeats { get; set; }
        public int FirstClassSeats { get; set; }
        public decimal MaxRangeKm { get; set; }
    }
}