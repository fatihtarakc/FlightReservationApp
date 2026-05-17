using App.Web.Enums;

namespace App.Web.ViewModels.Aircraft
{
    public class AircraftUpdateVM
    {
        public string         TailNumber      { get; set; }
        public int            ManufactureYear { get; set; }
        public AircraftStatus AircraftStatus  { get; set; }
        public Guid           AirlineId       { get; set; }
        public Guid           ModelId         { get; set; }
    }
}
