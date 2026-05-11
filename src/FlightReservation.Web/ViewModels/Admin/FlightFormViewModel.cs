using System.ComponentModel.DataAnnotations;
using FlightReservation.Core.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FlightReservation.Web.ViewModels.Admin;

public class FlightFormViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Sefer numarası gereklidir.")]
    [Display(Name = "Sefer No")]
    public string FlightNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Güzergah seçiniz.")]
    [Display(Name = "Güzergah")]
    public int RouteId { get; set; }

    [Required(ErrorMessage = "Uçak seçiniz.")]
    [Display(Name = "Uçak")]
    public int AircraftId { get; set; }

    [Required(ErrorMessage = "Kalkış zamanı gereklidir.")]
    [Display(Name = "Kalkış (UTC)")]
    public DateTime DepartureUtc { get; set; } = DateTime.UtcNow.AddDays(1);

    [Required(ErrorMessage = "Varış zamanı gereklidir.")]
    [Display(Name = "Varış (UTC)")]
    public DateTime ArrivalUtc { get; set; } = DateTime.UtcNow.AddDays(1).AddHours(1);

    [Display(Name = "Durum")]
    public FlightStatus Status { get; set; } = FlightStatus.Scheduled;

    [Display(Name = "Kapı")]
    public string? Gate { get; set; }

    [Display(Name = "Terminal")]
    public string? Terminal { get; set; }

    public List<SelectListItem> Routes { get; set; } = [];
    public List<SelectListItem> Aircrafts { get; set; } = [];
}
