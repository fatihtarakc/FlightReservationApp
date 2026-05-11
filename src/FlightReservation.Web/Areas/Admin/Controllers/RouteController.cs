using FlightReservation.Business.Interfaces;
using FlightReservation.Web.ViewModels.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlightReservation.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class RouteController : Controller
{
    private readonly IRouteService _routeService;

    public RouteController(IRouteService routeService)
    {
        _routeService = routeService;
    }

    public async Task<IActionResult> Index()
    {
        var routes = await _routeService.GetAllActiveAsync();
        return View(routes);
    }

    [HttpGet]
    public IActionResult Create() => View(new RouteFormViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(RouteFormViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var route = new Core.Entities.Route
        {
            OriginCity = model.OriginCity,
            OriginCode = model.OriginCode.ToUpper(),
            DestinationCity = model.DestinationCity,
            DestinationCode = model.DestinationCode.ToUpper(),
            DistanceKm = model.DistanceKm,
            EstimatedDurationMinutes = model.EstimatedDurationMinutes,
            IsActive = model.IsActive
        };

        await _routeService.CreateAsync(route);
        TempData["Success"] = "Güzergah oluşturuldu.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var route = await _routeService.GetByIdAsync(id);
        if (route == null) return NotFound();

        var vm = new RouteFormViewModel
        {
            Id = route.Id,
            OriginCity = route.OriginCity,
            OriginCode = route.OriginCode,
            DestinationCity = route.DestinationCity,
            DestinationCode = route.DestinationCode,
            DistanceKm = route.DistanceKm,
            EstimatedDurationMinutes = route.EstimatedDurationMinutes,
            IsActive = route.IsActive
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(RouteFormViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var route = await _routeService.GetByIdAsync(model.Id);
        if (route == null) return NotFound();

        route.OriginCity = model.OriginCity;
        route.OriginCode = model.OriginCode.ToUpper();
        route.DestinationCity = model.DestinationCity;
        route.DestinationCode = model.DestinationCode.ToUpper();
        route.DistanceKm = model.DistanceKm;
        route.EstimatedDurationMinutes = model.EstimatedDurationMinutes;
        route.IsActive = model.IsActive;

        await _routeService.UpdateAsync(route);
        TempData["Success"] = "Güzergah güncellendi.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _routeService.DeleteAsync(id);
        TempData["Success"] = "Güzergah silindi.";
        return RedirectToAction(nameof(Index));
    }
}
