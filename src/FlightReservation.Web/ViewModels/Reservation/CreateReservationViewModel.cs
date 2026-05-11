using System.ComponentModel.DataAnnotations;
using FlightReservation.Core.Enums;

namespace FlightReservation.Web.ViewModels.Reservation;

public class CreateReservationViewModel
{
    public int FlightId { get; set; }
    public int SeatId { get; set; }

    public string FlightNumber { get; set; } = string.Empty;
    public string Route { get; set; } = string.Empty;
    public DateTime DepartureUtc { get; set; }
    public string SeatNumber { get; set; } = string.Empty;
    public SeatClass SeatClass { get; set; }

    [Required(ErrorMessage = "Ad gereklidir.")]
    [Display(Name = "Yolcu Adı")]
    public string PassengerFirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Soyad gereklidir.")]
    [Display(Name = "Yolcu Soyadı")]
    public string PassengerLastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "T.C. Kimlik No gereklidir.")]
    [StringLength(11, MinimumLength = 11, ErrorMessage = "T.C. Kimlik No 11 haneli olmalıdır.")]
    [Display(Name = "T.C. Kimlik No")]
    public string PassengerIdentityNumber { get; set; } = string.Empty;
}
