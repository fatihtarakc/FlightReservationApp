namespace App.Dtos.AircraftDtos
{
    public class AircraftUpdateDto
    {
        public Guid Id { get; set; }
        public string TailNumber { get; set; } = null!;
        public int ManufactureYear { get; set; }
        public AircraftStatus AircraftStatus { get; set; }
        public Guid AirlineId { get; set; }
        public Guid ModelId { get; set; }
    }
}