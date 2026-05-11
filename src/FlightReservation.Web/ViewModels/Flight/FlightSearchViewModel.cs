using System.ComponentModel.DataAnnotations;

namespace FlightReservation.Web.ViewModels.Flight;

public class FlightSearchViewModel
{
    [Required(ErrorMessage = "Kalkış noktası seçiniz.")]
    [Display(Name = "Kalkış")]
    public string Origin { get; set; } = string.Empty;

    [Required(ErrorMessage = "Varış noktası seçiniz.")]
    [Display(Name = "Varış")]
    public string Destination { get; set; } = string.Empty;

    [Required(ErrorMessage = "Tarih seçiniz.")]
    [DataType(DataType.Date)]
    [Display(Name = "Tarih")]
    public DateTime Date { get; set; } = DateTime.Today.AddDays(1);

    public List<RouteOption> Routes { get; set; } = [];
}

public class RouteOption
{
    public string OriginCode { get; set; } = string.Empty;
    public string OriginCity { get; set; } = string.Empty;
    public string DestinationCode { get; set; } = string.Empty;
    public string DestinationCity { get; set; } = string.Empty;
    public string Display => $"{OriginCity} ({OriginCode}) → {DestinationCity} ({DestinationCode})";
}
