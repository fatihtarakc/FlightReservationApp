using FlightReservation.Business.Interfaces;
using FlightReservation.Core.Entities;
using FlightReservation.Web.ViewModels.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlightReservation.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class AircraftController : Controller
{
    private readonly IAircraftService _aircraftService;

    public AircraftController(IAircraftService aircraftService)
    {
        _aircraftService = aircraftService;
    }

    public async Task<IActionResult> Index()
    {
        var aircrafts = await _aircraftService.GetAllActiveAsync();
        return View(aircrafts);
    }

    [HttpGet]
    public IActionResult Create() => View(new AircraftFormViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AircraftFormViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var aircraft = new Aircraft
        {
            Model = model.Model,
            Manufacturer = model.Manufacturer,
            RegistrationNumber = model.RegistrationNumber,
            TotalRows = model.TotalRows,
            SeatsPerRow = model.SeatsPerRow,
            BusinessRowCount = model.BusinessRowCount,
            IsActive = model.IsActive
        };

        await _aircraftService.CreateAsync(aircraft);
        TempData["Success"] = "Uçak eklendi.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var aircraft = await _aircraftService.GetByIdWithSeatsAsync(id);
        if (aircraft == null) return NotFound();

        var vm = new AircraftFormViewModel
        {
            Id = aircraft.Id,
            Model = aircraft.Model,
            Manufacturer = aircraft.Manufacturer,
            RegistrationNumber = aircraft.RegistrationNumber,
            TotalRows = aircraft.TotalRows,
            SeatsPerRow = aircraft.SeatsPerRow,
            BusinessRowCount = aircraft.BusinessRowCount,
            IsActive = aircraft.IsActive
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(AircraftFormViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var aircraft = await _aircraftService.GetByIdWithSeatsAsync(model.Id);
        if (aircraft == null) return NotFound();

        aircraft.Model = model.Model;
        aircraft.Manufacturer = model.Manufacturer;
        aircraft.RegistrationNumber = model.RegistrationNumber;
        aircraft.IsActive = model.IsActive;

        await _aircraftService.UpdateAsync(aircraft);
        TempData["Success"] = "Uçak güncellendi.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _aircraftService.DeleteAsync(id);
        TempData["Success"] = "Uçak silindi.";
        return RedirectToAction(nameof(Index));
    }
}
