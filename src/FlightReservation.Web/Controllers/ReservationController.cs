using FlightReservation.Business.Interfaces;
using FlightReservation.Core.Entities;
using FlightReservation.Infrastructure.Email;
using FlightReservation.Web.ViewModels.Reservation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FlightReservation.Web.Controllers;

[Authorize]
public class ReservationController : Controller
{
    private readonly IReservationService _reservationService;
    private readonly IFlightService _flightService;
    private readonly IEmailService _emailService;
    private readonly UserManager<ApplicationUser> _userManager;

    public ReservationController(
        IReservationService reservationService,
        IFlightService flightService,
        UserManager<ApplicationUser> userManager, IEmailService emailService)
    {
        _reservationService = reservationService;
        _flightService = flightService;
        _userManager = userManager;
        _emailService = emailService;
    }

    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Challenge();

        var reservations = await _reservationService.GetUserReservationsAsync(user.Id);
        var vms = reservations.Select(r => new ReservationViewModel
        {
            Id = r.Id,
            PnrCode = r.PnrCode,
            FlightNumber = r.Flight.FlightNumber,
            Route = $"{r.Flight.Route.OriginCode} → {r.Flight.Route.DestinationCode}",
            DepartureUtc = r.Flight.DepartureUtc,
            ArrivalUtc = r.Flight.ArrivalUtc,
            SeatNumber = r.Seat.SeatNumber,
            SeatClass = r.Seat.SeatClass,
            PassengerFullName = r.PassengerFullName,
            PassengerIdentityNumber = r.PassengerIdentityNumber,
            Status = r.Status,
            ReservedAt = r.ReservedAt,
            CancelReason = r.CancelReason,
            CancelledAt = r.CancelledAt,
            FlightStatus = r.Flight.Status,
            Gate = r.Flight.Gate,
            Terminal = r.Flight.Terminal
        }).ToList();

        return View(vms);
    }

    [HttpGet]
    public async Task<IActionResult> Create(int flightId, int seatId)
    {
        var flight = await _flightService.GetByIdWithDetailsAsync(flightId);
        if (flight == null) return NotFound();

        var isAvailable = await _flightService.IsSeatAvailableAsync(flightId, seatId);
        if (!isAvailable)
        {
            TempData["Error"] = "Seçtiğiniz koltuk artık müsait değil.";
            return RedirectToAction("Details", "Flight", new { id = flightId });
        }

        var seat = flight.Aircraft.Seats.FirstOrDefault(s => s.Id == seatId);
        if (seat == null) return NotFound();

        var user = await _userManager.GetUserAsync(User);

        var vm = new CreateReservationViewModel
        {
            FlightId = flightId,
            SeatId = seatId,
            FlightNumber = flight.FlightNumber,
            Route = $"{flight.Route.OriginCity} ({flight.Route.OriginCode}) → {flight.Route.DestinationCity} ({flight.Route.DestinationCode})",
            DepartureUtc = flight.DepartureUtc,
            SeatNumber = seat.SeatNumber,
            SeatClass = seat.SeatClass,
            PassengerFirstName = user?.FirstName ?? string.Empty,
            PassengerLastName = user?.LastName ?? string.Empty,
            PassengerIdentityNumber = user?.IdentityNumber ?? string.Empty
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateReservationViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Challenge();

        var isAvailable = await _flightService.IsSeatAvailableAsync(model.FlightId, model.SeatId);
        if (!isAvailable)
        {
            ModelState.AddModelError(string.Empty, "Seçtiğiniz koltuk artık müsait değil. Lütfen başka bir koltuk seçin.");
            return View(model);
        }

        try
        {
            var reservation = new Reservation
            {
                FlightId = model.FlightId,
                SeatId = model.SeatId,
                UserId = user.Id,
                PassengerFirstName = model.PassengerFirstName,
                PassengerLastName = model.PassengerLastName,
                PassengerIdentityNumber = model.PassengerIdentityNumber
            };

            var created = await _reservationService.CreateAsync(reservation);
            TempData["Success"] = $"Rezervasyonunuz oluşturuldu. PNR Kodunuz: {created.PnrCode}";
            await _emailService.SendEmailAsync(user?.Email ?? string.Empty, user.FullName, "Rezervasyon Yapıldı", $"Rezervasyonunuz PNR No: {reservation.PnrCode}");
            return RedirectToAction(nameof(Details), new { id = created.Id });
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(model);
        }
    }

    public async Task<IActionResult> Details(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Challenge();

        var reservation = await _reservationService.GetByIdAsync(id);
        if (reservation == null) return NotFound();

        var belongs = await _reservationService.BelongsToUserAsync(id, user.Id);
        if (!belongs && !User.IsInRole("Admin"))
            return Forbid();

        var vm = new ReservationViewModel
        {
            Id = reservation.Id,
            PnrCode = reservation.PnrCode,
            FlightNumber = reservation.Flight.FlightNumber,
            Route = $"{reservation.Flight.Route.OriginCity} ({reservation.Flight.Route.OriginCode}) → {reservation.Flight.Route.DestinationCity} ({reservation.Flight.Route.DestinationCode})",
            DepartureUtc = reservation.Flight.DepartureUtc,
            ArrivalUtc = reservation.Flight.ArrivalUtc,
            SeatNumber = reservation.Seat.SeatNumber,
            SeatClass = reservation.Seat.SeatClass,
            PassengerFullName = reservation.PassengerFullName,
            PassengerIdentityNumber = reservation.PassengerIdentityNumber,
            Status = reservation.Status,
            ReservedAt = reservation.ReservedAt,
            CancelReason = reservation.CancelReason,
            CancelledAt = reservation.CancelledAt,
            FlightStatus = reservation.Flight.Status,
            Gate = reservation.Flight.Gate,
            Terminal = reservation.Flight.Terminal
        };

        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> Cancel(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Challenge();

        var reservation = await _reservationService.GetByIdAsync(id);
        if (reservation == null) return NotFound();

        var belongs = await _reservationService.BelongsToUserAsync(id, user.Id);
        if (!belongs) return Forbid();

        if (reservation.Status != Core.Enums.ReservationStatus.Active)
        {
            TempData["Error"] = "Bu rezervasyon iptal edilemez.";
            return RedirectToAction(nameof(Index));
        }

        var vm = new CancelReservationViewModel
        {
            Id = reservation.Id,
            PnrCode = reservation.PnrCode,
            FlightNumber = reservation.Flight.FlightNumber,
            Route = $"{reservation.Flight.Route.OriginCode} → {reservation.Flight.Route.DestinationCode}",
            DepartureUtc = reservation.Flight.DepartureUtc
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(CancelReservationViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Challenge();

        var belongs = await _reservationService.BelongsToUserAsync(model.Id, user.Id);
        if (!belongs) return Forbid();

        await _reservationService.CancelAsync(model.Id, model.Reason, user.Id);
        await _emailService.SendEmailAsync(user.Email, user.FullName, "Rezervasyon İptal Edildi", $"Rezervasyonunuz PNR No: {model.PnrCode} iptal edildi. İptal nedeni: {model.Reason}");
        TempData["Success"] = "Rezervasyonunuz iptal edildi.";
        return RedirectToAction(nameof(Index));
    }
}
