namespace App.Dtos.AirlineDtos
{
    public class AirlineAddDto
    {
        public string Name { get; set; } = null!;
        public string IataCode { get; set; } = null!;
        public string IcaoCode { get; set; } = null!;
        public string Country { get; set; } = null!;
        public string? LogoUrl { get; set; }
        public string? Website { get; set; }
    }
}