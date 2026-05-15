namespace App.Web.ViewModels.Airport
{
    public class AirportVM
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string IataCode { get; set; } = null!;
        public string IcaoCode { get; set; } = null!;
        public string City { get; set; } = null!;
        public string Country { get; set; } = null!;
        public string Timezone { get; set; } = null!;
    }
}
