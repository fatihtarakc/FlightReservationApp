using FlightReservation.Business.Interfaces;
using FlightReservation.Core.Enums;
using FlightReservation.Web.ViewModels.Flight;
using Microsoft.AspNetCore.Mvc;

namespace FlightReservation.Web.Controllers;

public class FlightController : Controller
{
    private readonly IFlightService _flightService;
    private readonly IRouteService _routeService;

    public FlightController(IFlightService flightService, IRouteService routeService)
    {
        _flightService = flightService;
        _routeService = routeService;
    }

    [HttpGet]
    public async Task<IActionResult> Search(string? origin, string? destination, DateTime? date)
    {
        var routes = await _routeService.GetAllActiveAsync();
        var routeOptions = routes.Select(r => new RouteOption
        {
            OriginCode = r.OriginCode,
            OriginCity = r.OriginCity,
            DestinationCode = r.DestinationCode,
            DestinationCity = r.DestinationCity
        }).ToList();

        if (string.IsNullOrEmpty(origin) || string.IsNullOrEmpty(destination) || !date.HasValue)
        {
            return View(new FlightSearchResultsViewModel
            {
                Search = new FlightSearchViewModel { Routes = routeOptions, Date = DateTime.Today.AddDays(1) }
            });
        }

        var searchDate = date.Value.Date;
        var flights = await _flightService.SearchAsync(origin, destination, searchDate);

        var results = flights.Select(f => new FlightResultViewModel
        {
            Id = f.Id,
            FlightNumber = f.FlightNumber,
            OriginCity = f.Route.OriginCity,
            OriginCode = f.Route.OriginCode,
            DestinationCity = f.Route.DestinationCity,
            DestinationCode = f.Route.DestinationCode,
            DepartureUtc = f.DepartureUtc,
            ArrivalUtc = f.ArrivalUtc,
            Duration = f.ArrivalUtc - f.DepartureUtc,
            AircraftModel = f.Aircraft.Model,
            TotalSeats = f.Aircraft.TotalCapacity,
            AvailableSeats = f.Aircraft.TotalCapacity - f.Reservations.Count(r => r.Status == ReservationStatus.Active),
            Status = f.Status,
            Gate = f.Gate,
            Terminal = f.Terminal,
            SearchOrigin = origin,
            SearchDestination = destination,
            SearchDate = searchDate
        }).ToList();

        var vm = new FlightSearchResultsViewModel
        {
            Search = new FlightSearchViewModel
            {
                Origin = origin,
                Destination = destination,
                Date = searchDate,
                Routes = routeOptions
            },
            Flights = results
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Search(FlightSearchViewModel model)
    {
        if (!ModelState.IsValid)
            return RedirectToAction(nameof(Search));

        return RedirectToAction(nameof(Search), new
        {
            origin = model.Origin,
            destination = model.Destination,
            date = model.Date.ToString("yyyy-MM-dd")
        });
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id, string? origin, string? destination, string? date)
    {
        var flight = await _flightService.GetByIdWithDetailsAsync(id);
        if (flight == null)
            return NotFound();

        var availableSeats = await _flightService.GetAvailableSeatsAsync(id);
        var availableSeatIds = availableSeats.Select(s => s.Id).ToHashSet();

        var vm = new FlightDetailsViewModel
        {
            Id = flight.Id,
            FlightNumber = flight.FlightNumber,
            OriginCity = flight.Route.OriginCity,
            OriginCode = flight.Route.OriginCode,
            DestinationCity = flight.Route.DestinationCity,
            DestinationCode = flight.Route.DestinationCode,
            DepartureUtc = flight.DepartureUtc,
            ArrivalUtc = flight.ArrivalUtc,
            Duration = flight.ArrivalUtc - flight.DepartureUtc,
            AircraftModel = flight.Aircraft.Model,
            TotalRows = flight.Aircraft.TotalRows,
            SeatsPerRow = flight.Aircraft.SeatsPerRow,
            BusinessRowCount = flight.Aircraft.BusinessRowCount,
            Status = flight.Status,
            Gate = flight.Gate,
            Terminal = flight.Terminal,
            Seats = flight.Aircraft.Seats.OrderBy(s => s.RowNumber).ThenBy(s => s.ColumnLetter).Select(s => new SeatViewModel
            {
                Id = s.Id,
                RowNumber = s.RowNumber,
                ColumnLetter = s.ColumnLetter,
                SeatClass = s.SeatClass,
                IsExitRow = s.IsExitRow,
                IsOccupied = !availableSeatIds.Contains(s.Id)
            }).ToList()
        };

        vm.AvailableEconomy = vm.Seats.Count(s => !s.IsOccupied && s.SeatClass == SeatClass.Economy);
        vm.AvailableBusiness = vm.Seats.Count(s => !s.IsOccupied && s.SeatClass == SeatClass.Business);

        ViewBag.ReturnOrigin = origin;
        ViewBag.ReturnDestination = destination;
        ViewBag.ReturnDate = date;

        return View(vm);
    }
}
