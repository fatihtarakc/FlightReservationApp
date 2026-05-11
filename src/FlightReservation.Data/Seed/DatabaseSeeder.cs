using FlightReservation.Core.Constants;
using FlightReservation.Core.Entities;
using FlightReservation.Core.Enums;
using FlightReservation.Data.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FlightReservation.Data.Seed;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var sp = scope.ServiceProvider;

        var db = sp.GetRequiredService<AppDbContext>();
        var userManager = sp.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = sp.GetRequiredService<RoleManager<IdentityRole>>();
        var logger = sp.GetRequiredService<ILogger<AppDbContext>>();

        try
        {
            await db.Database.MigrateAsync();
            logger.LogInformation("Migration applied.");

            await SeedRolesAsync(roleManager, logger);
            await SeedUsersAsync(userManager, logger);
            await SeedRoutesAsync(db, logger);
            await SeedAircraftsAsync(db, logger);
            await SeedFlightsAsync(db, logger);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Seed işlemi sırasında hata oluştu.");
            throw;
        }
    }

    // ── Roles ────────────────────────────────────────────────────────────────

    private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager, ILogger logger)
    {
        string[] roles = [AppRoles.Admin, AppRoles.Member];

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
                logger.LogInformation("Role created: {Role}", role);
            }
        }
    }

    // ── Users ────────────────────────────────────────────────────────────────

    private static async Task SeedUsersAsync(UserManager<ApplicationUser> userManager, ILogger logger)
    {
        var users = new[]
        {
            new
            {
                User = new ApplicationUser
                {
                    UserName = "admin@flightreservation.com",
                    Email = "admin@flightreservation.com",
                    EmailConfirmed = true,
                    FirstName = "Admin",
                    LastName = "User",
                    PreferredLanguage = "tr",
                    IsActive = true
                },
                Password = "Admin@123!",
                Role = AppRoles.Admin
            },
            new
            {
                User = new ApplicationUser
                {
                    UserName = "ali.yilmaz@example.com",
                    Email = "ali.yilmaz@example.com",
                    EmailConfirmed = true,
                    FirstName = "Ali",
                    LastName = "Yılmaz",
                    IdentityNumber = "12345678901",
                    BirthDate = new DateTime(1990, 5, 15),
                    PreferredLanguage = "tr",
                    IsActive = true
                },
                Password = "User@123!",
                Role = AppRoles.Member
            },
            new
            {
                User = new ApplicationUser
                {
                    UserName = "ayse.kaya@example.com",
                    Email = "ayse.kaya@example.com",
                    EmailConfirmed = true,
                    FirstName = "Ayşe",
                    LastName = "Kaya",
                    IdentityNumber = "98765432109",
                    BirthDate = new DateTime(1995, 8, 22),
                    PreferredLanguage = "en",
                    IsActive = true
                },
                Password = "User@123!",
                Role = AppRoles.Member
            },
            new
            {
                User = new ApplicationUser
                {
                    UserName = "john.doe@example.com",
                    Email = "john.doe@example.com",
                    EmailConfirmed = true,
                    FirstName = "John",
                    LastName = "Doe",
                    IdentityNumber = "11223344556",
                    BirthDate = new DateTime(1985, 3, 10),
                    PreferredLanguage = "en",
                    IsActive = true
                },
                Password = "User@123!",
                Role = AppRoles.Member
            }
        };

        foreach (var item in users)
        {
            if (await userManager.FindByEmailAsync(item.User.Email!) is null)
            {
                var result = await userManager.CreateAsync(item.User, item.Password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(item.User, item.Role);
                    logger.LogInformation("User created: {Email} [{Role}]", item.User.Email, item.Role);
                }
                else
                {
                    logger.LogWarning("User creation failed: {Email} - {Errors}",
                        item.User.Email, string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
        }
    }

    // ── Routes ───────────────────────────────────────────────────────────────

    private static async Task SeedRoutesAsync(AppDbContext db, ILogger logger)
    {
        if (await db.Routes.IgnoreQueryFilters().AnyAsync())
            return;

        var routes = new List<Core.Entities.Route>
        {
            new()
            {
                OriginCity = "İstanbul",
                OriginCode = "IST",
                DestinationCity = "Ankara",
                DestinationCode = "ESB",
                DistanceKm = 351,
                EstimatedDurationMinutes = 60,
                IsActive = true
            },
            new()
            {
                OriginCity = "İstanbul",
                OriginCode = "IST",
                DestinationCity = "İzmir",
                DestinationCode = "ADB",
                DistanceKm = 483,
                EstimatedDurationMinutes = 70,
                IsActive = true
            },
            new()
            {
                OriginCity = "İstanbul",
                OriginCode = "IST",
                DestinationCity = "Antalya",
                DestinationCode = "AYT",
                DistanceKm = 712,
                EstimatedDurationMinutes = 80,
                IsActive = true
            },
            new()
            {
                OriginCity = "Ankara",
                OriginCode = "ESB",
                DestinationCity = "İzmir",
                DestinationCode = "ADB",
                DistanceKm = 598,
                EstimatedDurationMinutes = 75,
                IsActive = true
            },
            new()
            {
                OriginCity = "Ankara",
                OriginCode = "ESB",
                DestinationCity = "Antalya",
                DestinationCode = "AYT",
                DistanceKm = 487,
                EstimatedDurationMinutes = 65,
                IsActive = true
            },
            new()
            {
                OriginCity = "İzmir",
                OriginCode = "ADB",
                DestinationCity = "Trabzon",
                DestinationCode = "TZX",
                DistanceKm = 1035,
                EstimatedDurationMinutes = 110,
                IsActive = true
            }
        };

        await db.Routes.AddRangeAsync(routes);
        await db.SaveChangesAsync();
        logger.LogInformation("Routes seeded: {Count}", routes.Count);
    }

    // ── Aircrafts + Seats ────────────────────────────────────────────────────

    private static async Task SeedAircraftsAsync(AppDbContext db, ILogger logger)
    {
        if (await db.Aircrafts.IgnoreQueryFilters().AnyAsync())
            return;

        var aircraftDefs = new[]
        {
            new { Model = "Boeing 737-800", Manufacturer = "Boeing",  Reg = "TC-JHM", TotalRows = 32, SeatsPerRow = 6, BusinessRows = 4 },
            new { Model = "Airbus A320",    Manufacturer = "Airbus",  Reg = "TC-JPL", TotalRows = 30, SeatsPerRow = 6, BusinessRows = 3 },
            new { Model = "Boeing 777-300", Manufacturer = "Boeing",  Reg = "TC-LKA", TotalRows = 42, SeatsPerRow = 9, BusinessRows = 6 },
            new { Model = "Airbus A321",    Manufacturer = "Airbus",  Reg = "TC-JRF", TotalRows = 35, SeatsPerRow = 6, BusinessRows = 4 },
        };

        foreach (var def in aircraftDefs)
        {
            var aircraft = new Aircraft
            {
                Model = def.Model,
                Manufacturer = def.Manufacturer,
                RegistrationNumber = def.Reg,
                TotalRows = def.TotalRows,
                SeatsPerRow = def.SeatsPerRow,
                BusinessRowCount = def.BusinessRows,
                IsActive = true
            };

            aircraft.Seats = GenerateSeats(def.TotalRows, def.SeatsPerRow, def.BusinessRows);

            await db.Aircrafts.AddAsync(aircraft);
            logger.LogInformation("Aircraft seeded: {Model} ({Reg}) — {Count} seats",
                def.Model, def.Reg, aircraft.Seats.Count);
        }

        await db.SaveChangesAsync();
    }

    private static List<Seat> GenerateSeats(int totalRows, int seatsPerRow, int businessRows)
    {
        var columns = seatsPerRow switch
        {
            6 => new[] { "A", "B", "C", "D", "E", "F" },
            9 => new[] { "A", "B", "C", "D", "E", "F", "G", "H", "J" },
            _ => Enumerable.Range(0, seatsPerRow).Select(i => ((char)('A' + i)).ToString()).ToArray()
        };

        var seats = new List<Seat>();

        for (int row = 1; row <= totalRows; row++)
        {
            var seatClass = row <= businessRows ? SeatClass.Business : SeatClass.Economy;

            foreach (var col in columns)
            {
                seats.Add(new Seat
                {
                    RowNumber = row,
                    ColumnLetter = col,
                    SeatClass = seatClass,
                    IsExitRow = row == businessRows + 1 || row == totalRows - 2
                });
            }
        }

        return seats;
    }

    // ── Flights ───────────────────────────────────────────────────────────────

    private static async Task SeedFlightsAsync(AppDbContext db, ILogger logger)
    {
        if (await db.Flights.IgnoreQueryFilters().AnyAsync())
            return;

        var routes = await db.Routes.IgnoreQueryFilters().ToListAsync();
        var aircrafts = await db.Aircrafts.IgnoreQueryFilters().ToListAsync();

        if (!routes.Any() || !aircrafts.Any())
        {
            logger.LogWarning("Seed: Route veya Aircraft bulunamadı, Flight seed atlandı.");
            return;
        }

        var today = DateTime.UtcNow.Date;
        var flights = new List<Flight>();
        int flightSeq = 1;

        // Her güzergah için 3-4 sefer (önümüzdeki 30 gün içinde)
        var schedule = new[]
        {
            // (routeIndex, aircraftIndex, günler_sonra[], saat_kalkış)
            (0, 0, new[] { 1, 4, 7, 10, 14 }, 7, 00),   // IST-ESB Boeing737
            (0, 1, new[] { 2, 5, 8, 11, 15 }, 14, 30),  // IST-ESB A320
            (1, 0, new[] { 1, 3, 6, 9, 12 }, 9, 15),    // IST-ADB Boeing737
            (1, 2, new[] { 2, 5, 8, 13, 16 }, 16, 45),  // IST-ADB Boeing777
            (2, 1, new[] { 1, 4, 7, 11, 17 }, 8, 00),   // IST-AYT A320
            (2, 3, new[] { 3, 6, 9, 12, 18 }, 18, 30),  // IST-AYT A321
            (3, 0, new[] { 2, 5, 8, 11, 14 }, 10, 20),  // ESB-ADB Boeing737
            (4, 1, new[] { 1, 4, 7, 10, 13 }, 13, 00),  // ESB-AYT A320
            (5, 2, new[] { 3, 6, 9, 12, 15 }, 11, 30),  // ADB-TZX Boeing777
        };

        foreach (var (routeIdx, aircraftIdx, days, departHour, departMinute) in schedule)
        {
            if (routeIdx >= routes.Count || aircraftIdx >= aircrafts.Count) continue;

            var route = routes[routeIdx];
            var aircraft = aircrafts[aircraftIdx];

            foreach (var dayOffset in days)
            {
                var departure = today.AddDays(dayOffset)
                    .AddHours(departHour)
                    .AddMinutes(departMinute);

                var arrival = departure.AddMinutes(route.EstimatedDurationMinutes);

                flights.Add(new Flight
                {
                    FlightNumber = $"TK{1000 + flightSeq++:D4}",
                    RouteId = route.Id,
                    AircraftId = aircraft.Id,
                    DepartureUtc = departure,
                    ArrivalUtc = arrival,
                    Status = FlightStatus.Scheduled,
                    Gate = $"G{(flightSeq % 20) + 1}",
                    Terminal = flightSeq % 3 == 0 ? "T2" : "T1"
                });
            }
        }

        // Geçmiş 2 sefer (Completed)
        if (routes.Any() && aircrafts.Any())
        {
            flights.Add(new Flight
            {
                FlightNumber = $"TK{1000 + flightSeq++:D4}",
                RouteId = routes[0].Id,
                AircraftId = aircrafts[0].Id,
                DepartureUtc = today.AddDays(-3).AddHours(10),
                ArrivalUtc = today.AddDays(-3).AddHours(11),
                Status = FlightStatus.Completed,
                Gate = "G5",
                Terminal = "T1"
            });

            flights.Add(new Flight
            {
                FlightNumber = $"TK{1000 + flightSeq++:D4}",
                RouteId = routes[1].Id,
                AircraftId = aircrafts[1].Id,
                DepartureUtc = today.AddDays(-1).AddHours(15),
                ArrivalUtc = today.AddDays(-1).AddHours(16).AddMinutes(10),
                Status = FlightStatus.Completed,
                Gate = "G12",
                Terminal = "T2"
            });

            // 1 iptal sefer
            flights.Add(new Flight
            {
                FlightNumber = $"TK{1000 + flightSeq++:D4}",
                RouteId = routes[2].Id,
                AircraftId = aircrafts[2].Id,
                DepartureUtc = today.AddDays(2).AddHours(9),
                ArrivalUtc = today.AddDays(2).AddHours(10).AddMinutes(20),
                Status = FlightStatus.Cancelled,
                Gate = "G8",
                Terminal = "T1"
            });
        }

        await db.Flights.AddRangeAsync(flights);
        await db.SaveChangesAsync();
        logger.LogInformation("Flights seeded: {Count}", flights.Count);
    }
}
