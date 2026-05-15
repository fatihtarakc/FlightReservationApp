namespace App.Web.ViewModels.Airport
{
    public class AirportAddVM
    {
        public string Name { get; set; } = null!;
        public string IataCode { get; set; } = null!;
        public string IcaoCode { get; set; } = null!;
        public string City { get; set; } = null!;
        public string Country { get; set; } = null!;
        public string Timezone { get; set; } = null!;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
