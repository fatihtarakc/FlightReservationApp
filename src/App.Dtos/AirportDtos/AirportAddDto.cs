namespace App.Dtos.AirportDtos
{
    public class AirportAddDto
    {
        public string Name { get; set; } = null!;
        public string IataCode { get; set; } = null!;
        public string IcaoCode { get; set; } = null!;
        public string City { get; set; } = null!;
        public string Country { get; set; } = null!;
        public string TimeZone { get; set; } = null!;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}