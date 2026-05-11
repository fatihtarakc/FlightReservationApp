using FlightReservation.Business.Interfaces;
using FlightReservation.Core.Entities;
using FlightReservation.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace FlightReservation.Business.Services;

public class RouteService : IRouteService
{
    private readonly AppDbContext _db;

    public RouteService(AppDbContext db) => _db = db;

    public async Task<IEnumerable<Route>> GetAllActiveAsync() =>
        await _db.Routes.Where(r => r.IsActive).OrderBy(r => r.OriginCity).ToListAsync();

    public async Task<Route?> GetByIdAsync(int id) =>
        await _db.Routes.FindAsync(id);

    public async Task<Route> CreateAsync(Route route)
    {
        _db.Routes.Add(route);
        await _db.SaveChangesAsync();
        return route;
    }

    public async Task<Route> UpdateAsync(Route route)
    {
        _db.Routes.Update(route);
        await _db.SaveChangesAsync();
        return route;
    }

    public async Task DeleteAsync(int id)
    {
        var route = await _db.Routes.FindAsync(id)
            ?? throw new KeyNotFoundException($"Route {id} bulunamadı.");
        route.IsDeleted = true;
        await _db.SaveChangesAsync();
    }

    public async Task<IEnumerable<string>> GetCitiesAsync() =>
        await _db.Routes
            .Where(r => r.IsActive)
            .SelectMany(r => new[] { r.OriginCity, r.DestinationCity })
            .Distinct()
            .OrderBy(c => c)
            .ToListAsync();
}
