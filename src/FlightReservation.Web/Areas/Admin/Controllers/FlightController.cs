using FlightReservation.Business.Interfaces;
using FlightReservation.Core.Entities;
using FlightReservation.Core.Enums;
using FlightReservation.Web.ViewModels.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FlightReservation.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class FlightController : Controller
{
    private readonly IFlightService _flightService;
    private readonly IRouteService _routeService;
    private readonly IAircraftService _aircraftService;

    public FlightController(IFlightService flightService, IRouteService routeService, IAircraftService aircraftService)
    {
        _flightService = flightService;
        _routeService = routeService;
        _aircraftService = aircraftService;
    }

    public async Task<IActionResult> Index(int page = 1)
    {
        var flights = await _flightService.GetAllAsync(page, 20);
        ViewBag.Page = page;
        return View(flights);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        return View(await BuildFormAsync(new FlightFormViewModel()));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(FlightFormViewModel model)
    {
        if (!ModelState.IsValid)
            return View(await BuildFormAsync(model));

        var flight = new Flight
        {
            FlightNumber = model.FlightNumber,
            RouteId = model.RouteId,
            AircraftId = model.AircraftId,
            DepartureUtc = model.DepartureUtc,
            ArrivalUtc = model.ArrivalUtc,
            Status = model.Status,
            Gate = model.Gate,
            Terminal = model.Terminal
        };

        await _flightService.CreateAsync(flight);
        TempData["Success"] = "Sefer oluşturuldu.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var flight = await _flightService.GetByIdWithDetailsAsync(id);
        if (flight == null) return NotFound();

        var vm = new FlightFormViewModel
        {
            Id = flight.Id,
            FlightNumber = flight.FlightNumber,
            RouteId = flight.RouteId,
            AircraftId = flight.AircraftId,
            DepartureUtc = flight.DepartureUtc,
            ArrivalUtc = flight.ArrivalUtc,
            Status = flight.Status,
            Gate = flight.Gate,
            Terminal = flight.Terminal
        };

        return View(await BuildFormAsync(vm));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(FlightFormViewModel model)
    {
        if (!ModelState.IsValid)
            return View(await BuildFormAsync(model));

        var flight = await _flightService.GetByIdWithDetailsAsync(model.Id);
        if (flight == null) return NotFound();

        flight.FlightNumber = model.FlightNumber;
        flight.RouteId = model.RouteId;
        flight.AircraftId = model.AircraftId;
        flight.DepartureUtc = model.DepartureUtc;
        flight.ArrivalUtc = model.ArrivalUtc;
        flight.Status = model.Status;
        flight.Gate = model.Gate;
        flight.Terminal = model.Terminal;

        await _flightService.UpdateAsync(flight);
        TempData["Success"] = "Sefer güncellendi.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _flightService.DeleteAsync(id);
        TempData["Success"] = "Sefer silindi.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(int id, FlightStatus status)
    {
        await _flightService.UpdateStatusAsync(id, status);
        TempData["Success"] = "Sefer durumu güncellendi.";
        return RedirectToAction(nameof(Index));
    }

    private async Task<FlightFormViewModel> BuildFormAsync(FlightFormViewModel vm)
    {
        var routes = await _routeService.GetAllActiveAsync();
        var aircrafts = await _aircraftService.GetAllActiveAsync();

        vm.Routes = routes.Select(r => new SelectListItem
        {
            Value = r.Id.ToString(),
            Text = $"{r.OriginCity} ({r.OriginCode}) → {r.DestinationCity} ({r.DestinationCode})",
            Selected = r.Id == vm.RouteId
        }).ToList();

        vm.Aircrafts = aircrafts.Select(a => new SelectListItem
        {
            Value = a.Id.ToString(),
            Text = $"{a.Model} ({a.RegistrationNumber})",
            Selected = a.Id == vm.AircraftId
        }).ToList();

        return vm;
    }
}
