namespace App.Web.ViewModels.Aircraft
{
    public class AircraftAddVM
    {
        public string TailNumber      { get; set; }
        public int    ManufactureYear { get; set; } = DateTime.UtcNow.Year;
        public Guid   AirlineId       { get; set; }
        public Guid   ModelId         { get; set; }
    }
}
