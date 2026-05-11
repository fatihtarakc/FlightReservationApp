using FlightReservation.Business.Interfaces;
using FlightReservation.Core.Entities;
using FlightReservation.Core.Enums;
using FlightReservation.Web.ViewModels.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FlightReservation.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class DashboardController : Controller
{
    private readonly IFlightService _flightService;
    private readonly IReservationService _reservationService;
    private readonly IRouteService _routeService;
    private readonly IAircraftService _aircraftService;
    private readonly UserManager<ApplicationUser> _userManager;

    public DashboardController(
        IFlightService flightService,
        IReservationService reservationService,
        IRouteService routeService,
        IAircraftService aircraftService,
        UserManager<ApplicationUser> userManager)
    {
        _flightService = flightService;
        _reservationService = reservationService;
        _routeService = routeService;
        _aircraftService = aircraftService;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var allFlights = (await _flightService.GetAllAsync(1, 1000)).ToList();
        var allReservations = (await _reservationService.GetAllAsync(1, 1000)).ToList();
        var allRoutes = (await _routeService.GetAllActiveAsync()).ToList();
        var allAircrafts = (await _aircraftService.GetAllActiveAsync()).ToList();
        var allUsers = _userManager.Users.ToList();

        var vm = new DashboardViewModel
        {
            TotalFlights = allFlights.Count,
            ScheduledFlights = allFlights.Count(f => f.Status == FlightStatus.Scheduled),
            TotalReservations = allReservations.Count,
            ActiveReservations = allReservations.Count(r => r.Status == ReservationStatus.Active),
            TotalUsers = allUsers.Count,
            TotalRoutes = allRoutes.Count,
            TotalAircrafts = allAircrafts.Count,
            RecentReservations = allReservations.Take(10).Select(r => new RecentReservationItem
            {
                PnrCode = r.PnrCode,
                PassengerName = r.PassengerFullName,
                FlightNumber = r.Flight?.FlightNumber ?? "-",
                Route = r.Flight != null ? $"{r.Flight.Route?.OriginCode} → {r.Flight.Route?.DestinationCode}" : "-",
                ReservedAt = r.ReservedAt,
                Status = r.Status.ToString()
            }).ToList(),
            UpcomingFlights = allFlights
                .Where(f => f.DepartureUtc > DateTime.UtcNow && f.Status == FlightStatus.Scheduled)
                .OrderBy(f => f.DepartureUtc)
                .Take(10)
                .Select(f => new UpcomingFlightItem
                {
                    Id = f.Id,
                    FlightNumber = f.FlightNumber,
                    Route = $"{f.Route?.OriginCode} → {f.Route?.DestinationCode}",
                    DepartureUtc = f.DepartureUtc,
                    ReservationCount = f.Reservations?.Count(r => r.Status == ReservationStatus.Active) ?? 0,
                    TotalSeats = f.Aircraft?.TotalCapacity ?? 0,
                    Status = f.Status.ToString()
                }).ToList()
        };

        return View(vm);
    }
}
