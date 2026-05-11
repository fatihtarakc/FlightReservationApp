using System.ComponentModel.DataAnnotations;

namespace FlightReservation.Web.ViewModels.Admin;

public class RouteFormViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Kalkış şehri gereklidir.")]
    [Display(Name = "Kalkış Şehri")]
    public string OriginCity { get; set; } = string.Empty;

    [Required(ErrorMessage = "Kalkış kodu gereklidir.")]
    [StringLength(3, MinimumLength = 3, ErrorMessage = "IATA kodu 3 harf olmalıdır.")]
    [Display(Name = "Kalkış Kodu (IATA)")]
    public string OriginCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "Varış şehri gereklidir.")]
    [Display(Name = "Varış Şehri")]
    public string DestinationCity { get; set; } = string.Empty;

    [Required(ErrorMessage = "Varış kodu gereklidir.")]
    [StringLength(3, MinimumLength = 3, ErrorMessage = "IATA kodu 3 harf olmalıdır.")]
    [Display(Name = "Varış Kodu (IATA)")]
    public string DestinationCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "Mesafe gereklidir.")]
    [Range(1, 20000, ErrorMessage = "Mesafe 1-20000 km arasında olmalıdır.")]
    [Display(Name = "Mesafe (km)")]
    public int DistanceKm { get; set; }

    [Required(ErrorMessage = "Tahmini süre gereklidir.")]
    [Range(10, 1440, ErrorMessage = "Süre 10-1440 dakika arasında olmalıdır.")]
    [Display(Name = "Tahmini Süre (dk)")]
    public int EstimatedDurationMinutes { get; set; }

    [Display(Name = "Aktif")]
    public bool IsActive { get; set; } = true;
}
