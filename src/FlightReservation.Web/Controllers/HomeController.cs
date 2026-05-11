using FlightReservation.Business.Interfaces;
using FlightReservation.Web.ViewModels.Flight;
using Microsoft.AspNetCore.Mvc;

namespace FlightReservation.Web.Controllers;

public class HomeController : Controller
{
    private readonly IRouteService _routeService;

    public HomeController(IRouteService routeService)
    {
        _routeService = routeService;
    }

    public async Task<IActionResult> Index()
    {
        var routes = await _routeService.GetAllActiveAsync();
        var vm = new FlightSearchViewModel
        {
            Date = DateTime.Today.AddDays(1),
            Routes = routes.Select(r => new RouteOption
            {
                OriginCode = r.OriginCode,
                OriginCity = r.OriginCity,
                DestinationCode = r.DestinationCode,
                DestinationCity = r.DestinationCity
            }).ToList()
        };
        return View(vm);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View();
    }
}
