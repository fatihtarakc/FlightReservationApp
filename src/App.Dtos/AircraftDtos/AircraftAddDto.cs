namespace App.Dtos.AircraftDtos
{
    public class AircraftAddDto
    {
        public string TailNumber { get; set; } = null!;
        public int ManufactureYear { get; set; }
        public Guid AirlineId { get; set; }
        public Guid ModelId { get; set; }
    }
}
