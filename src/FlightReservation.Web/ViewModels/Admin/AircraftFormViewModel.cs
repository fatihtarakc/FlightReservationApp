using System.ComponentModel.DataAnnotations;

namespace FlightReservation.Web.ViewModels.Admin;

public class AircraftFormViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Model adı gereklidir.")]
    [Display(Name = "Model")]
    public string Model { get; set; } = string.Empty;

    [Required(ErrorMessage = "Üretici adı gereklidir.")]
    [Display(Name = "Üretici")]
    public string Manufacturer { get; set; } = string.Empty;

    [Required(ErrorMessage = "Tescil numarası gereklidir.")]
    [Display(Name = "Tescil No")]
    public string RegistrationNumber { get; set; } = string.Empty;

    [Required]
    [Range(1, 100, ErrorMessage = "Sıra sayısı 1-100 arasında olmalıdır.")]
    [Display(Name = "Toplam Sıra")]
    public int TotalRows { get; set; } = 30;

    [Required]
    [Range(3, 10, ErrorMessage = "Sıra başı koltuk 3-10 arasında olmalıdır.")]
    [Display(Name = "Sıra Başı Koltuk")]
    public int SeatsPerRow { get; set; } = 6;

    [Required]
    [Range(0, 20, ErrorMessage = "Business sıra sayısı 0-20 arasında olmalıdır.")]
    [Display(Name = "Business Sıra Sayısı")]
    public int BusinessRowCount { get; set; } = 3;

    [Display(Name = "Aktif")]
    public bool IsActive { get; set; } = true;
}
