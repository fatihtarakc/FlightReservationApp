namespace App.DataAccess.Concrete.SeedDatas
{
    public static class SeedDataService
    {
        private const string AdminEmail       = "admin@flightreservation.com";
        private const string AdminUsername    = "admin";
        private const string AdminPassword    = "Admin2026+-!?";
        private const string TestAdminEmail   = "testadmin@flightreservation.com";
        private const string TestAdminUsername = "testadmin";
        private const string TestAdminPassword = "TestAdmin2026+-!?";
        private const string TestUserEmail    = "testuser@flightreservation.com";
        private const string TestUserUsername = "testuser";
        private const string TestUserPassword = "TestUser2026+-!?";

        public static async Task SeedAsync(
            FlightReservationDbContext db,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            await SeedRolesAsync(roleManager);
            await SeedAdminAsync(db, userManager);
            await SeedTestAdminAsync(db, userManager);
            await SeedTestAppUserAsync(db, userManager);
            await SeedManufacturersAsync(db);
            await SeedModelsAsync(db);
            await SeedAirlinesAsync(db);
            await SeedAirportsAsync(db);
            await SeedRoutesAsync(db);
            await SeedSchedulesAsync(db);
            await SeedAircraftsAsync(db);
            await SeedSeatsAsync(db);
            await SeedFlightsAsync(db);
        }

        // ── Roles & Users ────────────────────────────────────────────────────────

        private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            foreach (var role in new[] { "Admin", "AppUser" })
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
        }

        private static async Task SeedAdminAsync(FlightReservationDbContext db, UserManager<IdentityUser> userManager)
        {
            if (await userManager.FindByEmailAsync(AdminEmail) != null) return;
            var identity = new IdentityUser { Email = AdminEmail, UserName = AdminUsername, NormalizedEmail = AdminEmail.ToUpperInvariant(), NormalizedUserName = AdminUsername.ToUpperInvariant(), EmailConfirmed = true };
            await userManager.CreateAsync(identity, AdminPassword);
            await userManager.AddToRoleAsync(identity, "Admin");
            if (!await db.Admins.AnyAsync(a => a.Email == AdminEmail))
            { db.Admins.Add(new Admin { Name = "Super", Surname = "Admin", Email = AdminEmail, IdentityId = identity.Id, CreatedBy = "system" }); await db.SaveChangesAsync(); }
        }

        private static async Task SeedTestAdminAsync(FlightReservationDbContext db, UserManager<IdentityUser> userManager)
        {
            if (await userManager.FindByEmailAsync(TestAdminEmail) != null) return;
            var identity = new IdentityUser { Email = TestAdminEmail, UserName = TestAdminUsername, NormalizedEmail = TestAdminEmail.ToUpperInvariant(), NormalizedUserName = TestAdminUsername.ToUpperInvariant(), EmailConfirmed = true };
            await userManager.CreateAsync(identity, TestAdminPassword);
            await userManager.AddToRoleAsync(identity, "Admin");
            if (!await db.Admins.AnyAsync(a => a.Email == TestAdminEmail))
            { db.Admins.Add(new Admin { Name = "Test", Surname = "Admin", Email = TestAdminEmail, IdentityId = identity.Id, IsSuperAdmin = false, CreatedBy = "system" }); await db.SaveChangesAsync(); }
        }

        private static async Task SeedTestAppUserAsync(FlightReservationDbContext db, UserManager<IdentityUser> userManager)
        {
            if (await userManager.FindByEmailAsync(TestUserEmail) != null) return;
            var identity = new IdentityUser { Email = TestUserEmail, UserName = TestUserUsername, NormalizedEmail = TestUserEmail.ToUpperInvariant(), NormalizedUserName = TestUserUsername.ToUpperInvariant(), EmailConfirmed = true };
            await userManager.CreateAsync(identity, TestUserPassword);
            await userManager.AddToRoleAsync(identity, "AppUser");
            if (!await db.AppUsers.AnyAsync(u => u.Email == TestUserEmail))
            {
                db.AppUsers.Add(new AppUser { Name = "Test", Surname = "User", Email = TestUserEmail, PhoneNumber = "+905001234567", BirthDate = new DateTime(1996, 6, 15, 0, 0, 0, DateTimeKind.Utc), UserStatus = UserStatus.Active, PreferredNotificationChannel = NotificationChannel.Email, Nationality = "TR", IdentityId = identity.Id, CreatedBy = "system" });
                await db.SaveChangesAsync();
            }
        }

        // ── Manufacturers ─────────────────────────────────────────────────────────

        private static async Task SeedManufacturersAsync(FlightReservationDbContext db)
        {
            if (await db.Manufacturers.AnyAsync()) return;
            db.Manufacturers.AddRange(
                new Manufacturer { Id = MfgIds.Boeing,     Name = "Boeing",     Country = "United States", CreatedBy = "system" },
                new Manufacturer { Id = MfgIds.Airbus,     Name = "Airbus",     Country = "France",        CreatedBy = "system" },
                new Manufacturer { Id = MfgIds.Bombardier, Name = "Bombardier", Country = "Canada",        CreatedBy = "system" }
            );
            await db.SaveChangesAsync();
        }

        // ── Aircraft Models ───────────────────────────────────────────────────────

        private static async Task SeedModelsAsync(FlightReservationDbContext db)
        {
            if (await db.Models.AnyAsync()) return;
            db.Models.AddRange(
                new Model { Id = ModelIds.B737800,  ManufacturerId = MfgIds.Boeing,  Name = "737-800",   BodyType = BodyType.NarrowBody, MaxPassengerCapacity = 189, EconomySeats = 162, PremiumEconomySeats = 0,  BusinessSeats = 12,  FirstClassSeats = 0, MaxRangeKm = 5765,  CreatedBy = "system" },
                new Model { Id = ModelIds.B777300ER,ManufacturerId = MfgIds.Boeing,  Name = "777-300ER", BodyType = BodyType.WideBody,   MaxPassengerCapacity = 396, EconomySeats = 304, PremiumEconomySeats = 40, BusinessSeats = 42,  FirstClassSeats = 8, MaxRangeKm = 13650, CreatedBy = "system" },
                new Model { Id = ModelIds.B7879,    ManufacturerId = MfgIds.Boeing,  Name = "787-9",     BodyType = BodyType.WideBody,   MaxPassengerCapacity = 296, EconomySeats = 222, PremiumEconomySeats = 28, BusinessSeats = 42,  FirstClassSeats = 0, MaxRangeKm = 14140, CreatedBy = "system" },
                new Model { Id = ModelIds.A320Neo,  ManufacturerId = MfgIds.Airbus,  Name = "A320neo",   BodyType = BodyType.NarrowBody, MaxPassengerCapacity = 194, EconomySeats = 150, PremiumEconomySeats = 0,  BusinessSeats = 12,  FirstClassSeats = 0, MaxRangeKm = 6300,  CreatedBy = "system" },
                new Model { Id = ModelIds.A350900,  ManufacturerId = MfgIds.Airbus,  Name = "A350-900",  BodyType = BodyType.WideBody,   MaxPassengerCapacity = 369, EconomySeats = 253, PremiumEconomySeats = 48, BusinessSeats = 40,  FirstClassSeats = 8, MaxRangeKm = 15000, CreatedBy = "system" },
                new Model { Id = ModelIds.A380800,  ManufacturerId = MfgIds.Airbus,  Name = "A380-800",  BodyType = BodyType.WideBody,   MaxPassengerCapacity = 555, EconomySeats = 399, PremiumEconomySeats = 76, BusinessSeats = 76,  FirstClassSeats = 14,MaxRangeKm = 15200, CreatedBy = "system" }
            );
            await db.SaveChangesAsync();
        }

        // ── Airlines ──────────────────────────────────────────────────────────────

        private static async Task SeedAirlinesAsync(FlightReservationDbContext db)
        {
            if (await db.Airlines.AnyAsync()) return;
            db.Airlines.AddRange(
                new Airline { Id = AirlineIds.THY, Name = "Turkish Airlines",  IataCode = "TK", IcaoCode = "THY", Country = "Turkey",               Website = "https://www.turkishairlines.com", CreatedBy = "system" },
                new Airline { Id = AirlineIds.EK,  Name = "Emirates",          IataCode = "EK", IcaoCode = "UAE", Country = "United Arab Emirates",   Website = "https://www.emirates.com",        CreatedBy = "system" },
                new Airline { Id = AirlineIds.PC,  Name = "Pegasus Airlines",  IataCode = "PC", IcaoCode = "PGT", Country = "Turkey",               Website = "https://www.flypgs.com",          CreatedBy = "system" },
                new Airline { Id = AirlineIds.LH,  Name = "Lufthansa",         IataCode = "LH", IcaoCode = "DLH", Country = "Germany",              Website = "https://www.lufthansa.com",       CreatedBy = "system" },
                new Airline { Id = AirlineIds.BA,  Name = "British Airways",   IataCode = "BA", IcaoCode = "BAW", Country = "United Kingdom",        Website = "https://www.britishairways.com",  CreatedBy = "system" },
                new Airline { Id = AirlineIds.AF,  Name = "Air France",        IataCode = "AF", IcaoCode = "AFR", Country = "France",               Website = "https://www.airfrance.com",       CreatedBy = "system" }
            );
            await db.SaveChangesAsync();
        }

        // ── Airports ──────────────────────────────────────────────────────────────

        private static async Task SeedAirportsAsync(FlightReservationDbContext db)
        {
            if (await db.Airports.AnyAsync()) return;
            db.Airports.AddRange(
                new Airport { Id = AptIds.IST, Name = "Istanbul Airport",           IataCode = "IST", IcaoCode = "LTFM", City = "Istanbul",      Country = "Turkey",               TimeZone = "Europe/Istanbul",   CreatedBy = "system" },
                new Airport { Id = AptIds.SAW, Name = "Sabiha Gokcen Airport",      IataCode = "SAW", IcaoCode = "LTFJ", City = "Istanbul",      Country = "Turkey",               TimeZone = "Europe/Istanbul",   CreatedBy = "system" },
                new Airport { Id = AptIds.ESB, Name = "Esenboga Airport",           IataCode = "ESB", IcaoCode = "LTAC", City = "Ankara",        Country = "Turkey",               TimeZone = "Europe/Istanbul",   CreatedBy = "system" },
                new Airport { Id = AptIds.ADB, Name = "Adnan Menderes Airport",     IataCode = "ADB", IcaoCode = "LTBJ", City = "Izmir",         Country = "Turkey",               TimeZone = "Europe/Istanbul",   CreatedBy = "system" },
                new Airport { Id = AptIds.AYT, Name = "Antalya Airport",            IataCode = "AYT", IcaoCode = "LTAI", City = "Antalya",       Country = "Turkey",               TimeZone = "Europe/Istanbul",   CreatedBy = "system" },
                new Airport { Id = AptIds.LHR, Name = "Heathrow Airport",           IataCode = "LHR", IcaoCode = "EGLL", City = "London",        Country = "United Kingdom",        TimeZone = "Europe/London",     CreatedBy = "system" },
                new Airport { Id = AptIds.DXB, Name = "Dubai International Airport",IataCode = "DXB", IcaoCode = "OMDB", City = "Dubai",         Country = "United Arab Emirates",  TimeZone = "Asia/Dubai",        CreatedBy = "system" },
                new Airport { Id = AptIds.JFK, Name = "John F. Kennedy Airport",    IataCode = "JFK", IcaoCode = "KJFK", City = "New York",      Country = "United States",         TimeZone = "America/New_York",  CreatedBy = "system" },
                new Airport { Id = AptIds.FRA, Name = "Frankfurt Airport",          IataCode = "FRA", IcaoCode = "EDDF", City = "Frankfurt",     Country = "Germany",              TimeZone = "Europe/Berlin",     CreatedBy = "system" },
                new Airport { Id = AptIds.CDG, Name = "Charles de Gaulle Airport",  IataCode = "CDG", IcaoCode = "LFPG", City = "Paris",         Country = "France",               TimeZone = "Europe/Paris",      CreatedBy = "system" },
                new Airport { Id = AptIds.AMS, Name = "Amsterdam Schiphol Airport", IataCode = "AMS", IcaoCode = "EHAM", City = "Amsterdam",     Country = "Netherlands",          TimeZone = "Europe/Amsterdam",  CreatedBy = "system" },
                new Airport { Id = AptIds.MUC, Name = "Munich Airport",             IataCode = "MUC", IcaoCode = "EDDM", City = "Munich",        Country = "Germany",              TimeZone = "Europe/Berlin",     CreatedBy = "system" },
                new Airport { Id = AptIds.MAD, Name = "Adolfo Suarez Airport",      IataCode = "MAD", IcaoCode = "LEMD", City = "Madrid",        Country = "Spain",                TimeZone = "Europe/Madrid",     CreatedBy = "system" },
                new Airport { Id = AptIds.FCO, Name = "Fiumicino Airport",          IataCode = "FCO", IcaoCode = "LIRF", City = "Rome",          Country = "Italy",                TimeZone = "Europe/Rome",       CreatedBy = "system" },
                new Airport { Id = AptIds.SIN, Name = "Changi Airport",             IataCode = "SIN", IcaoCode = "WSSS", City = "Singapore",     Country = "Singapore",            TimeZone = "Asia/Singapore",    CreatedBy = "system" },
                new Airport { Id = AptIds.DOH, Name = "Hamad International Airport",IataCode = "DOH", IcaoCode = "OTHH", City = "Doha",          Country = "Qatar",                TimeZone = "Asia/Qatar",        CreatedBy = "system" },
                new Airport { Id = AptIds.LAX, Name = "Los Angeles Airport",        IataCode = "LAX", IcaoCode = "KLAX", City = "Los Angeles",   Country = "United States",         TimeZone = "America/Los_Angeles",CreatedBy = "system" },
                new Airport { Id = AptIds.ORD, Name = "O'Hare International Airport",IataCode="ORD",  IcaoCode = "KORD", City = "Chicago",       Country = "United States",         TimeZone = "America/Chicago",   CreatedBy = "system" },
                new Airport { Id = AptIds.BKK, Name = "Suvarnabhumi Airport",       IataCode = "BKK", IcaoCode = "VTBS", City = "Bangkok",       Country = "Thailand",             TimeZone = "Asia/Bangkok",      CreatedBy = "system" }
            );
            await db.SaveChangesAsync();
        }

        // ── Routes ────────────────────────────────────────────────────────────────

        private static async Task SeedRoutesAsync(FlightReservationDbContext db)
        {
            if (await db.Routes.AnyAsync()) return;
            db.Routes.AddRange(
                // Turkish domestic
                new Route { Id = RouteIds.IST_ESB, DepartureAirportId = AptIds.IST, ArrivalAirportId = AptIds.ESB, DistanceKm =  352, EstimatedDuration = TimeSpan.FromMinutes(70),  CreatedBy = "system" },
                new Route { Id = RouteIds.IST_ADB, DepartureAirportId = AptIds.IST, ArrivalAirportId = AptIds.ADB, DistanceKm =  445, EstimatedDuration = TimeSpan.FromMinutes(80),  CreatedBy = "system" },
                new Route { Id = RouteIds.IST_AYT, DepartureAirportId = AptIds.IST, ArrivalAirportId = AptIds.AYT, DistanceKm =  555, EstimatedDuration = TimeSpan.FromMinutes(90),  CreatedBy = "system" },
                // IST international
                new Route { Id = RouteIds.IST_LHR, DepartureAirportId = AptIds.IST, ArrivalAirportId = AptIds.LHR, DistanceKm = 2514, EstimatedDuration = TimeSpan.FromHours(4),    CreatedBy = "system" },
                new Route { Id = RouteIds.IST_DXB, DepartureAirportId = AptIds.IST, ArrivalAirportId = AptIds.DXB, DistanceKm = 2573, EstimatedDuration = TimeSpan.FromHours(4.5),  CreatedBy = "system" },
                new Route { Id = RouteIds.IST_FRA, DepartureAirportId = AptIds.IST, ArrivalAirportId = AptIds.FRA, DistanceKm = 1869, EstimatedDuration = TimeSpan.FromHours(3.5),  CreatedBy = "system" },
                new Route { Id = RouteIds.IST_CDG, DepartureAirportId = AptIds.IST, ArrivalAirportId = AptIds.CDG, DistanceKm = 2244, EstimatedDuration = TimeSpan.FromHours(3.75), CreatedBy = "system" },
                new Route { Id = RouteIds.IST_AMS, DepartureAirportId = AptIds.IST, ArrivalAirportId = AptIds.AMS, DistanceKm = 2194, EstimatedDuration = TimeSpan.FromHours(3.5),  CreatedBy = "system" },
                new Route { Id = RouteIds.IST_FCO, DepartureAirportId = AptIds.IST, ArrivalAirportId = AptIds.FCO, DistanceKm = 1949, EstimatedDuration = TimeSpan.FromHours(3.25), CreatedBy = "system" },
                new Route { Id = RouteIds.IST_MAD, DepartureAirportId = AptIds.IST, ArrivalAirportId = AptIds.MAD, DistanceKm = 2855, EstimatedDuration = TimeSpan.FromHours(4.5),  CreatedBy = "system" },
                new Route { Id = RouteIds.IST_JFK, DepartureAirportId = AptIds.IST, ArrivalAirportId = AptIds.JFK, DistanceKm = 9377, EstimatedDuration = TimeSpan.FromHours(10.5), CreatedBy = "system" },
                new Route { Id = RouteIds.IST_LAX, DepartureAirportId = AptIds.IST, ArrivalAirportId = AptIds.LAX, DistanceKm =12020, EstimatedDuration = TimeSpan.FromHours(14),   CreatedBy = "system" },
                // Return routes IST
                new Route { Id = RouteIds.LHR_IST, DepartureAirportId = AptIds.LHR, ArrivalAirportId = AptIds.IST, DistanceKm = 2514, EstimatedDuration = TimeSpan.FromHours(4),    CreatedBy = "system" },
                new Route { Id = RouteIds.DXB_IST, DepartureAirportId = AptIds.DXB, ArrivalAirportId = AptIds.IST, DistanceKm = 2573, EstimatedDuration = TimeSpan.FromHours(4.5),  CreatedBy = "system" },
                new Route { Id = RouteIds.FRA_IST, DepartureAirportId = AptIds.FRA, ArrivalAirportId = AptIds.IST, DistanceKm = 1869, EstimatedDuration = TimeSpan.FromHours(3.5),  CreatedBy = "system" },
                new Route { Id = RouteIds.CDG_IST, DepartureAirportId = AptIds.CDG, ArrivalAirportId = AptIds.IST, DistanceKm = 2244, EstimatedDuration = TimeSpan.FromHours(3.75), CreatedBy = "system" },
                new Route { Id = RouteIds.AMS_IST, DepartureAirportId = AptIds.AMS, ArrivalAirportId = AptIds.IST, DistanceKm = 2194, EstimatedDuration = TimeSpan.FromHours(3.5),  CreatedBy = "system" },
                // Emirates DXB
                new Route { Id = RouteIds.DXB_LHR, DepartureAirportId = AptIds.DXB, ArrivalAirportId = AptIds.LHR, DistanceKm = 5482, EstimatedDuration = TimeSpan.FromHours(7.5),  CreatedBy = "system" },
                new Route { Id = RouteIds.DXB_SIN, DepartureAirportId = AptIds.DXB, ArrivalAirportId = AptIds.SIN, DistanceKm = 5841, EstimatedDuration = TimeSpan.FromHours(7.5),  CreatedBy = "system" },
                new Route { Id = RouteIds.DXB_JFK, DepartureAirportId = AptIds.DXB, ArrivalAirportId = AptIds.JFK, DistanceKm =11022, EstimatedDuration = TimeSpan.FromHours(14),   CreatedBy = "system" },
                new Route { Id = RouteIds.DXB_BKK, DepartureAirportId = AptIds.DXB, ArrivalAirportId = AptIds.BKK, DistanceKm = 4908, EstimatedDuration = TimeSpan.FromHours(6.5),  CreatedBy = "system" },
                new Route { Id = RouteIds.LHR_DXB, DepartureAirportId = AptIds.LHR, ArrivalAirportId = AptIds.DXB, DistanceKm = 5482, EstimatedDuration = TimeSpan.FromHours(7),    CreatedBy = "system" },
                // FRA / CDG hub
                new Route { Id = RouteIds.FRA_JFK, DepartureAirportId = AptIds.FRA, ArrivalAirportId = AptIds.JFK, DistanceKm = 6195, EstimatedDuration = TimeSpan.FromHours(9),    CreatedBy = "system" },
                new Route { Id = RouteIds.LHR_JFK, DepartureAirportId = AptIds.LHR, ArrivalAirportId = AptIds.JFK, DistanceKm = 5571, EstimatedDuration = TimeSpan.FromHours(7.5),  CreatedBy = "system" }
            );
            await db.SaveChangesAsync();
        }

        // ── Schedules ─────────────────────────────────────────────────────────────

        private static async Task SeedSchedulesAsync(FlightReservationDbContext db)
        {
            if (await db.Schedules.AnyAsync()) return;
            var v = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            db.Schedules.AddRange(
                // Turkish domestic (Daily)
                new Schedule { Id = SchIds.TK7_IST_ESB,   RouteId = RouteIds.IST_ESB, Code = "TK7-IST-ESB",   ValidFrom = v, DaysOfWeek = DaysOfWeek.Daily, DepartureTime = TimeSpan.FromHours(7),    TimeZone = "Europe/Istanbul", CreatedBy = "system" },
                new Schedule { Id = SchIds.TK9_IST_ESB,   RouteId = RouteIds.IST_ESB, Code = "TK9-IST-ESB",   ValidFrom = v, DaysOfWeek = DaysOfWeek.Daily, DepartureTime = TimeSpan.FromHours(13),   TimeZone = "Europe/Istanbul", CreatedBy = "system" },
                new Schedule { Id = SchIds.TK11_IST_ESB,  RouteId = RouteIds.IST_ESB, Code = "TK11-IST-ESB",  ValidFrom = v, DaysOfWeek = DaysOfWeek.Daily, DepartureTime = TimeSpan.FromHours(18),   TimeZone = "Europe/Istanbul", CreatedBy = "system" },
                new Schedule { Id = SchIds.TK1793_IST_ADB,RouteId = RouteIds.IST_ADB, Code = "TK1793-IST-ADB",ValidFrom = v, DaysOfWeek = DaysOfWeek.Daily, DepartureTime = TimeSpan.FromHours(7),    TimeZone = "Europe/Istanbul", CreatedBy = "system" },
                new Schedule { Id = SchIds.TK1795_IST_ADB,RouteId = RouteIds.IST_ADB, Code = "TK1795-IST-ADB",ValidFrom = v, DaysOfWeek = DaysOfWeek.Daily, DepartureTime = TimeSpan.FromHours(12.5), TimeZone = "Europe/Istanbul", CreatedBy = "system" },
                new Schedule { Id = SchIds.TK1797_IST_ADB,RouteId = RouteIds.IST_ADB, Code = "TK1797-IST-ADB",ValidFrom = v, DaysOfWeek = DaysOfWeek.Daily, DepartureTime = TimeSpan.FromHours(18.5), TimeZone = "Europe/Istanbul", CreatedBy = "system" },
                new Schedule { Id = SchIds.TK2120_IST_AYT,RouteId = RouteIds.IST_AYT, Code = "TK2120-IST-AYT",ValidFrom = v, DaysOfWeek = DaysOfWeek.Daily, DepartureTime = TimeSpan.FromHours(8),    TimeZone = "Europe/Istanbul", CreatedBy = "system" },
                new Schedule { Id = SchIds.TK2122_IST_AYT,RouteId = RouteIds.IST_AYT, Code = "TK2122-IST-AYT",ValidFrom = v, DaysOfWeek = DaysOfWeek.Daily, DepartureTime = TimeSpan.FromHours(14),   TimeZone = "Europe/Istanbul", CreatedBy = "system" },
                new Schedule { Id = SchIds.TK2124_IST_AYT,RouteId = RouteIds.IST_AYT, Code = "TK2124-IST-AYT",ValidFrom = v, DaysOfWeek = DaysOfWeek.Daily, DepartureTime = TimeSpan.FromHours(20),   TimeZone = "Europe/Istanbul", CreatedBy = "system" },
                new Schedule { Id = SchIds.PC3150_IST_AYT,RouteId = RouteIds.IST_AYT, Code = "PC3150-IST-AYT",ValidFrom = v, DaysOfWeek = DaysOfWeek.Daily, DepartureTime = TimeSpan.FromHours(9),    TimeZone = "Europe/Istanbul", CreatedBy = "system" },
                new Schedule { Id = SchIds.PC3152_IST_AYT,RouteId = RouteIds.IST_AYT, Code = "PC3152-IST-AYT",ValidFrom = v, DaysOfWeek = DaysOfWeek.Daily, DepartureTime = TimeSpan.FromHours(17),   TimeZone = "Europe/Istanbul", CreatedBy = "system" },
                // THY IST international
                new Schedule { Id = SchIds.TK1_IST_LHR,  RouteId = RouteIds.IST_LHR, Code = "TK1-IST-LHR",   ValidFrom = v, DaysOfWeek = DaysOfWeek.Daily, DepartureTime = TimeSpan.FromHours(8),    TimeZone = "Europe/Istanbul", CreatedBy = "system" },
                new Schedule { Id = SchIds.TK3_IST_LHR,  RouteId = RouteIds.IST_LHR, Code = "TK3-IST-LHR",   ValidFrom = v, DaysOfWeek = DaysOfWeek.Daily, DepartureTime = TimeSpan.FromHours(16),   TimeZone = "Europe/Istanbul", CreatedBy = "system" },
                new Schedule { Id = SchIds.TK11_IST_DXB, RouteId = RouteIds.IST_DXB, Code = "TK11-IST-DXB",  ValidFrom = v, DaysOfWeek = DaysOfWeek.Daily, DepartureTime = TimeSpan.FromHours(10),   TimeZone = "Europe/Istanbul", CreatedBy = "system" },
                new Schedule { Id = SchIds.TK13_IST_DXB, RouteId = RouteIds.IST_DXB, Code = "TK13-IST-DXB",  ValidFrom = v, DaysOfWeek = DaysOfWeek.Daily, DepartureTime = TimeSpan.FromHours(18),   TimeZone = "Europe/Istanbul", CreatedBy = "system" },
                new Schedule { Id = SchIds.TK751_IST_FRA, RouteId = RouteIds.IST_FRA, Code = "TK751-IST-FRA", ValidFrom = v, DaysOfWeek = DaysOfWeek.Daily, DepartureTime = TimeSpan.FromHours(9),    TimeZone = "Europe/Istanbul", CreatedBy = "system" },
                new Schedule { Id = SchIds.TK1841_IST_CDG,RouteId = RouteIds.IST_CDG, Code = "TK1841-IST-CDG",ValidFrom = v, DaysOfWeek = DaysOfWeek.Daily, DepartureTime = TimeSpan.FromHours(8),    TimeZone = "Europe/Istanbul", CreatedBy = "system" },
                new Schedule { Id = SchIds.TK2010_IST_AMS,RouteId = RouteIds.IST_AMS, Code = "TK2010-IST-AMS",ValidFrom = v, DaysOfWeek = DaysOfWeek.Daily, DepartureTime = TimeSpan.FromHours(10),   TimeZone = "Europe/Istanbul", CreatedBy = "system" },
                new Schedule { Id = SchIds.TK1881_IST_FCO,RouteId = RouteIds.IST_FCO, Code = "TK1881-IST-FCO",ValidFrom = v, DaysOfWeek = DaysOfWeek.Daily, DepartureTime = TimeSpan.FromHours(9.5),  TimeZone = "Europe/Istanbul", CreatedBy = "system" },
                new Schedule { Id = SchIds.TK1851_IST_MAD,RouteId = RouteIds.IST_MAD, Code = "TK1851-IST-MAD",ValidFrom = v, DaysOfWeek = DaysOfWeek.Daily, DepartureTime = TimeSpan.FromHours(10),   TimeZone = "Europe/Istanbul", CreatedBy = "system" },
                new Schedule { Id = SchIds.TK3_IST_JFK,  RouteId = RouteIds.IST_JFK, Code = "TK3-IST-JFK",   ValidFrom = v, DaysOfWeek = DaysOfWeek.Daily, DepartureTime = TimeSpan.FromHours(23.5), TimeZone = "Europe/Istanbul", CreatedBy = "system" },
                new Schedule { Id = SchIds.TK203_IST_LAX,RouteId = RouteIds.IST_LAX, Code = "TK203-IST-LAX", ValidFrom = v, DaysOfWeek = DaysOfWeek.Daily, DepartureTime = TimeSpan.FromHours(22),   TimeZone = "Europe/Istanbul", CreatedBy = "system" },
                // Return to IST
                new Schedule { Id = SchIds.TK2_LHR_IST,  RouteId = RouteIds.LHR_IST, Code = "TK2-LHR-IST",   ValidFrom = v, DaysOfWeek = DaysOfWeek.Daily, DepartureTime = TimeSpan.FromHours(11),   TimeZone = "Europe/London",   CreatedBy = "system" },
                new Schedule { Id = SchIds.TK12_DXB_IST, RouteId = RouteIds.DXB_IST, Code = "TK12-DXB-IST",  ValidFrom = v, DaysOfWeek = DaysOfWeek.Daily, DepartureTime = TimeSpan.FromHours(8),    TimeZone = "Asia/Dubai",      CreatedBy = "system" },
                new Schedule { Id = SchIds.LH1290_FRA_IST,RouteId = RouteIds.FRA_IST, Code = "LH1290-FRA-IST",ValidFrom = v, DaysOfWeek = DaysOfWeek.Daily, DepartureTime = TimeSpan.FromHours(10),   TimeZone = "Europe/Berlin",   CreatedBy = "system" },
                new Schedule { Id = SchIds.AF1471_CDG_IST,RouteId = RouteIds.CDG_IST, Code = "AF1471-CDG-IST",ValidFrom = v, DaysOfWeek = DaysOfWeek.Daily, DepartureTime = TimeSpan.FromHours(8),    TimeZone = "Europe/Paris",    CreatedBy = "system" },
                new Schedule { Id = SchIds.BA686_LHR_IST, RouteId = RouteIds.LHR_IST, Code = "BA686-LHR-IST", ValidFrom = v, DaysOfWeek = DaysOfWeek.Daily, DepartureTime = TimeSpan.FromHours(14),   TimeZone = "Europe/London",   CreatedBy = "system" },
                new Schedule { Id = SchIds.TK752_AMS_IST, RouteId = RouteIds.AMS_IST, Code = "TK752-AMS-IST", ValidFrom = v, DaysOfWeek = DaysOfWeek.Daily, DepartureTime = TimeSpan.FromHours(14),   TimeZone = "Europe/Amsterdam",CreatedBy = "system" },
                // Emirates DXB routes
                new Schedule { Id = SchIds.EK1_DXB_LHR,  RouteId = RouteIds.DXB_LHR, Code = "EK1-DXB-LHR",   ValidFrom = v, DaysOfWeek = DaysOfWeek.Daily, DepartureTime = TimeSpan.FromHours(8),    TimeZone = "Asia/Dubai",      CreatedBy = "system" },
                new Schedule { Id = SchIds.EK3_DXB_LHR,  RouteId = RouteIds.DXB_LHR, Code = "EK3-DXB-LHR",   ValidFrom = v, DaysOfWeek = DaysOfWeek.Daily, DepartureTime = TimeSpan.FromHours(14.5), TimeZone = "Asia/Dubai",      CreatedBy = "system" },
                new Schedule { Id = SchIds.EK352_DXB_SIN,RouteId = RouteIds.DXB_SIN, Code = "EK352-DXB-SIN", ValidFrom = v, DaysOfWeek = DaysOfWeek.Daily, DepartureTime = TimeSpan.FromHours(2.5),  TimeZone = "Asia/Dubai",      CreatedBy = "system" },
                new Schedule { Id = SchIds.EK354_DXB_SIN,RouteId = RouteIds.DXB_SIN, Code = "EK354-DXB-SIN", ValidFrom = v, DaysOfWeek = DaysOfWeek.Daily, DepartureTime = TimeSpan.FromHours(14),   TimeZone = "Asia/Dubai",      CreatedBy = "system" },
                new Schedule { Id = SchIds.EK201_DXB_JFK,RouteId = RouteIds.DXB_JFK, Code = "EK201-DXB-JFK", ValidFrom = v, DaysOfWeek = DaysOfWeek.Daily, DepartureTime = TimeSpan.FromHours(2),    TimeZone = "Asia/Dubai",      CreatedBy = "system" },
                new Schedule { Id = SchIds.EK2_LHR_DXB,  RouteId = RouteIds.LHR_DXB, Code = "EK2-LHR-DXB",   ValidFrom = v, DaysOfWeek = DaysOfWeek.Daily, DepartureTime = TimeSpan.FromHours(9),    TimeZone = "Europe/London",   CreatedBy = "system" },
                new Schedule { Id = SchIds.EK374_DXB_BKK,RouteId = RouteIds.DXB_BKK, Code = "EK374-DXB-BKK", ValidFrom = v, DaysOfWeek = DaysOfWeek.Daily, DepartureTime = TimeSpan.FromHours(3.5),  TimeZone = "Asia/Dubai",      CreatedBy = "system" },
                // Lufthansa / BA cross-routes
                new Schedule { Id = SchIds.LH400_FRA_JFK,RouteId = RouteIds.FRA_JFK, Code = "LH400-FRA-JFK",  ValidFrom = v, DaysOfWeek = DaysOfWeek.Daily, DepartureTime = TimeSpan.FromHours(10.5), TimeZone = "Europe/Berlin",   CreatedBy = "system" },
                new Schedule { Id = SchIds.BA175_LHR_JFK,RouteId = RouteIds.LHR_JFK, Code = "BA175-LHR-JFK",  ValidFrom = v, DaysOfWeek = DaysOfWeek.Daily, DepartureTime = TimeSpan.FromHours(11),   TimeZone = "Europe/London",   CreatedBy = "system" },
                new Schedule { Id = SchIds.BA177_LHR_JFK,RouteId = RouteIds.LHR_JFK, Code = "BA177-LHR-JFK",  ValidFrom = v, DaysOfWeek = DaysOfWeek.Daily, DepartureTime = TimeSpan.FromHours(15),   TimeZone = "Europe/London",   CreatedBy = "system" }
            );
            await db.SaveChangesAsync();
        }

        // ── Aircraft ──────────────────────────────────────────────────────────────

        private static async Task SeedAircraftsAsync(FlightReservationDbContext db)
        {
            if (await db.Aircrafts.AnyAsync()) return;
            db.Aircrafts.AddRange(
                // Turkish Airlines
                new Aircraft { Id = AcIds.TCLNA,  TailNumber = "TC-LNA", ManufactureYear = 2018, AircraftStatus = AircraftStatus.Active, AirlineId = AirlineIds.THY, ModelId = ModelIds.B777300ER, CreatedBy = "system" },
                new Aircraft { Id = AcIds.TCKUE,  TailNumber = "TC-KUE", ManufactureYear = 2020, AircraftStatus = AircraftStatus.Active, AirlineId = AirlineIds.THY, ModelId = ModelIds.A320Neo,   CreatedBy = "system" },
                new Aircraft { Id = AcIds.TCJJK,  TailNumber = "TC-JJK", ManufactureYear = 2019, AircraftStatus = AircraftStatus.Active, AirlineId = AirlineIds.THY, ModelId = ModelIds.A350900,   CreatedBy = "system" },
                new Aircraft { Id = AcIds.TCLJT,  TailNumber = "TC-LJT", ManufactureYear = 2021, AircraftStatus = AircraftStatus.Active, AirlineId = AirlineIds.THY, ModelId = ModelIds.B7879,     CreatedBy = "system" },
                new Aircraft { Id = AcIds.TCJHM,  TailNumber = "TC-JHM", ManufactureYear = 2022, AircraftStatus = AircraftStatus.Active, AirlineId = AirlineIds.THY, ModelId = ModelIds.A320Neo,   CreatedBy = "system" },
                // Emirates
                new Aircraft { Id = AcIds.A6EDB,  TailNumber = "A6-EDB", ManufactureYear = 2016, AircraftStatus = AircraftStatus.Active, AirlineId = AirlineIds.EK,  ModelId = ModelIds.A350900,   CreatedBy = "system" },
                new Aircraft { Id = AcIds.A6EWC,  TailNumber = "A6-EWC", ManufactureYear = 2017, AircraftStatus = AircraftStatus.Active, AirlineId = AirlineIds.EK,  ModelId = ModelIds.A380800,   CreatedBy = "system" },
                new Aircraft { Id = AcIds.A6EDM,  TailNumber = "A6-EDM", ManufactureYear = 2015, AircraftStatus = AircraftStatus.Active, AirlineId = AirlineIds.EK,  ModelId = ModelIds.B777300ER, CreatedBy = "system" },
                new Aircraft { Id = AcIds.A6ENB,  TailNumber = "A6-ENB", ManufactureYear = 2019, AircraftStatus = AircraftStatus.Active, AirlineId = AirlineIds.EK,  ModelId = ModelIds.B777300ER, CreatedBy = "system" },
                // Pegasus
                new Aircraft { Id = AcIds.YBBSA,  TailNumber = "YB-BSA", ManufactureYear = 2019, AircraftStatus = AircraftStatus.Active, AirlineId = AirlineIds.PC,  ModelId = ModelIds.A320Neo,   CreatedBy = "system" },
                new Aircraft { Id = AcIds.YBBSB,  TailNumber = "YB-BSB", ManufactureYear = 2020, AircraftStatus = AircraftStatus.Active, AirlineId = AirlineIds.PC,  ModelId = ModelIds.A320Neo,   CreatedBy = "system" },
                new Aircraft { Id = AcIds.YBBSC,  TailNumber = "YB-BSC", ManufactureYear = 2018, AircraftStatus = AircraftStatus.Active, AirlineId = AirlineIds.PC,  ModelId = ModelIds.B737800,   CreatedBy = "system" },
                // Lufthansa
                new Aircraft { Id = AcIds.DAIXA,  TailNumber = "D-AIXA", ManufactureYear = 2020, AircraftStatus = AircraftStatus.Active, AirlineId = AirlineIds.LH,  ModelId = ModelIds.A350900,   CreatedBy = "system" },
                new Aircraft { Id = AcIds.DAIXB,  TailNumber = "D-AIXB", ManufactureYear = 2021, AircraftStatus = AircraftStatus.Active, AirlineId = AirlineIds.LH,  ModelId = ModelIds.A320Neo,   CreatedBy = "system" },
                new Aircraft { Id = AcIds.DAIXC,  TailNumber = "D-AIXC", ManufactureYear = 2019, AircraftStatus = AircraftStatus.Active, AirlineId = AirlineIds.LH,  ModelId = ModelIds.B777300ER, CreatedBy = "system" },
                // British Airways
                new Aircraft { Id = AcIds.GXWBA,  TailNumber = "G-XWBA", ManufactureYear = 2020, AircraftStatus = AircraftStatus.Active, AirlineId = AirlineIds.BA,  ModelId = ModelIds.B7879,     CreatedBy = "system" },
                new Aircraft { Id = AcIds.GEUXA,  TailNumber = "G-EUXA", ManufactureYear = 2019, AircraftStatus = AircraftStatus.Active, AirlineId = AirlineIds.BA,  ModelId = ModelIds.A320Neo,   CreatedBy = "system" },
                new Aircraft { Id = AcIds.GXWBB,  TailNumber = "G-XWBB", ManufactureYear = 2018, AircraftStatus = AircraftStatus.Active, AirlineId = AirlineIds.BA,  ModelId = ModelIds.B777300ER, CreatedBy = "system" },
                // Air France
                new Aircraft { Id = AcIds.FGZNX,  TailNumber = "F-GZNX", ManufactureYear = 2017, AircraftStatus = AircraftStatus.Active, AirlineId = AirlineIds.AF,  ModelId = ModelIds.B777300ER, CreatedBy = "system" },
                new Aircraft { Id = AcIds.FHPJH,  TailNumber = "F-HPJH", ManufactureYear = 2020, AircraftStatus = AircraftStatus.Active, AirlineId = AirlineIds.AF,  ModelId = ModelIds.A350900,   CreatedBy = "system" },
                new Aircraft { Id = AcIds.FGKXA,  TailNumber = "F-GKXA", ManufactureYear = 2021, AircraftStatus = AircraftStatus.Active, AirlineId = AirlineIds.AF,  ModelId = ModelIds.A320Neo,   CreatedBy = "system" }
            );
            await db.SaveChangesAsync();
        }

        // ── Seats ─────────────────────────────────────────────────────────────────

        private static async Task SeedSeatsAsync(FlightReservationDbContext db)
        {
            if (await db.Seats.AnyAsync()) return;
            var seats = new List<Seat>();

            // Turkish Airlines — B777-300ER (WideBody): 8F + 24B + 30PE + 220E = 282
            GenerateSeats(seats, AcIds.TCLNA, true,  firstRows:2, businessRows:4, premiumRows:3, economyRows:22);
            // Turkish Airlines — A320neo (NarrowBody): 12B + 120E = 132
            GenerateSeats(seats, AcIds.TCKUE, false, firstRows:0, businessRows:3, premiumRows:0, economyRows:20);
            // Turkish Airlines — A350-900 (WideBody): 8F + 24B + 30PE + 200E = 262
            GenerateSeats(seats, AcIds.TCJJK, true,  firstRows:2, businessRows:4, premiumRows:3, economyRows:20);
            // Turkish Airlines — B787-9 (WideBody): 18B + 20PE + 180E = 218
            GenerateSeats(seats, AcIds.TCLJT, true,  firstRows:0, businessRows:3, premiumRows:2, economyRows:18);
            // Turkish Airlines — A320neo #2 (NarrowBody): 12B + 120E = 132
            GenerateSeats(seats, AcIds.TCJHM, false, firstRows:0, businessRows:3, premiumRows:0, economyRows:20);

            // Emirates — A350-900 (WideBody): 8F + 24B + 30PE + 200E = 262
            GenerateSeats(seats, AcIds.A6EDB, true,  firstRows:2, businessRows:4, premiumRows:3, economyRows:20);
            // Emirates — A380-800 (WideBody): 14F + 36B + 40PE + 240E = 330
            GenerateSeats(seats, AcIds.A6EWC, true,  firstRows:2, businessRows:6, premiumRows:4, economyRows:24);
            // Emirates — B777-300ER (WideBody): 8F + 24B + 30PE + 220E = 282
            GenerateSeats(seats, AcIds.A6EDM, true,  firstRows:2, businessRows:4, premiumRows:3, economyRows:22);
            // Emirates — B777-300ER #2 (WideBody): 8F + 24B + 30PE + 220E = 282
            GenerateSeats(seats, AcIds.A6ENB, true,  firstRows:2, businessRows:4, premiumRows:3, economyRows:22);

            // Pegasus — A320neo (NarrowBody): 12B + 120E = 132
            GenerateSeats(seats, AcIds.YBBSA, false, firstRows:0, businessRows:3, premiumRows:0, economyRows:20);
            GenerateSeats(seats, AcIds.YBBSB, false, firstRows:0, businessRows:3, premiumRows:0, economyRows:20);
            // Pegasus — B737-800 (NarrowBody): 12B + 120E = 132
            GenerateSeats(seats, AcIds.YBBSC, false, firstRows:0, businessRows:3, premiumRows:0, economyRows:20);

            // Lufthansa — A350-900 (WideBody): 8F + 24B + 30PE + 200E = 262
            GenerateSeats(seats, AcIds.DAIXA, true,  firstRows:2, businessRows:4, premiumRows:3, economyRows:20);
            // Lufthansa — A320neo (NarrowBody): 12B + 120E = 132
            GenerateSeats(seats, AcIds.DAIXB, false, firstRows:0, businessRows:3, premiumRows:0, economyRows:20);
            // Lufthansa — B777-300ER (WideBody): 8F + 24B + 30PE + 220E = 282
            GenerateSeats(seats, AcIds.DAIXC, true,  firstRows:2, businessRows:4, premiumRows:3, economyRows:22);

            // British Airways — B787-9 (WideBody): 18B + 20PE + 180E = 218
            GenerateSeats(seats, AcIds.GXWBA, true,  firstRows:0, businessRows:3, premiumRows:2, economyRows:18);
            // British Airways — A320neo (NarrowBody): 12B + 120E = 132
            GenerateSeats(seats, AcIds.GEUXA, false, firstRows:0, businessRows:3, premiumRows:0, economyRows:20);
            // British Airways — B777-300ER (WideBody): 8F + 24B + 30PE + 220E = 282
            GenerateSeats(seats, AcIds.GXWBB, true,  firstRows:2, businessRows:4, premiumRows:3, economyRows:22);

            // Air France — B777-300ER (WideBody): 8F + 24B + 30PE + 220E = 282
            GenerateSeats(seats, AcIds.FGZNX, true,  firstRows:2, businessRows:4, premiumRows:3, economyRows:22);
            // Air France — A350-900 (WideBody): 8F + 24B + 30PE + 200E = 262
            GenerateSeats(seats, AcIds.FHPJH, true,  firstRows:2, businessRows:4, premiumRows:3, economyRows:20);
            // Air France — A320neo (NarrowBody): 12B + 120E = 132
            GenerateSeats(seats, AcIds.FGKXA, false, firstRows:0, businessRows:3, premiumRows:0, economyRows:20);

            db.Seats.AddRange(seats);
            await db.SaveChangesAsync();
        }

        private static void GenerateSeats(List<Seat> seats, Guid aircraftId, bool isWideBody,
            int firstRows, int businessRows, int premiumRows, int economyRows)
        {
            // Column layouts
            var firstCols   = new[] { SeatColumn.A, SeatColumn.B, SeatColumn.C, SeatColumn.D };
            var bizColsWide = new[] { SeatColumn.A, SeatColumn.B, SeatColumn.C, SeatColumn.D, SeatColumn.E, SeatColumn.F };
            var bizColsNarrow = new[] { SeatColumn.A, SeatColumn.B, SeatColumn.C, SeatColumn.D };
            var wideBodyCols = new[] { SeatColumn.A, SeatColumn.B, SeatColumn.C, SeatColumn.D, SeatColumn.E, SeatColumn.F, SeatColumn.G, SeatColumn.H, SeatColumn.J, SeatColumn.K };
            var narrowBodyCols = new[] { SeatColumn.A, SeatColumn.B, SeatColumn.C, SeatColumn.D, SeatColumn.E, SeatColumn.F };

            // WideBody 3+4+3 layout (A,B,C | D,E,F,G | H,J,K): aisle cols = C,D,G,H; window = A,K
            // NarrowBody 3+3 layout (A,B,C | D,E,F): aisle cols = C,D; window = A,F
            bool IsWindow(SeatColumn col) => isWideBody ? (col == SeatColumn.A || col == SeatColumn.K) : (col == SeatColumn.A || col == SeatColumn.F);
            bool IsAisle(SeatColumn col) => isWideBody
                ? (col == SeatColumn.C || col == SeatColumn.D || col == SeatColumn.G || col == SeatColumn.H)
                : (col == SeatColumn.C || col == SeatColumn.D);

            int row = 1;
            void AddRows(int count, SeatClass cls, SeatColumn[] cols)
            {
                for (int r = row; r < row + count; r++)
                    foreach (var col in cols)
                        seats.Add(new Seat
                        {
                            AircraftId = aircraftId, Row = r, Column = col, SeatClass = cls,
                            IsWindowSeat = IsWindow(col), IsAisleSeat = IsAisle(col),
                            HasExtraLegRoom = (r == row),
                            CreatedBy = "system"
                        });
                row += count;
            }

            if (firstRows   > 0) AddRows(firstRows,   SeatClass.First,          firstCols);
            if (businessRows> 0) AddRows(businessRows, SeatClass.Business,       isWideBody ? bizColsWide : bizColsNarrow);
            if (premiumRows > 0) AddRows(premiumRows,  SeatClass.PremiumEconomy, wideBodyCols);
            if (economyRows > 0) AddRows(economyRows,  SeatClass.Economy,        isWideBody ? wideBodyCols : narrowBodyCols);
        }

        // ── Flights ───────────────────────────────────────────────────────────────

        private static async Task SeedFlightsAsync(FlightReservationDbContext db)
        {
            if (await db.Flights.AnyAsync()) return;
            var today  = DateTime.UtcNow.Date;
            var flights = new List<Flight>();

            void AddFlights(string number, int days, double depHour, double durationH,
                decimal eco, decimal peco, decimal biz, decimal first, Currency currency,
                Guid aircraftId, Guid airlineId, Guid scheduleId)
            {
                for (int i = 1; i <= days; i++)
                {
                    var dep = today.AddDays(i).AddHours(depHour);
                    flights.Add(new Flight
                    {
                        Number = number, DepartureDateTime = dep,
                        ArrivalDateTime = dep.AddHours(durationH),
                        Duration = TimeSpan.FromHours(durationH),
                        BaseEconomyPrice = eco, BasePremiumEconomyPrice = peco,
                        BaseBusinessPrice = biz, BaseFirstClassPrice = first,
                        Currency = currency, FlightStatus = FlightStatus.Scheduled,
                        AircraftId = aircraftId, AirlineId = airlineId,
                        ScheduleId = scheduleId, CreatedBy = "system"
                    });
                }
            }

            // ── Turkish Airlines domestic ──────────────────────────────────────
            AddFlights("TK7",   21, 7,    1.17, 2300, 3500,  7000,  7000, Currency.TRY, AcIds.TCKUE, AirlineIds.THY, SchIds.TK7_IST_ESB);
            AddFlights("TK9",   21, 13,   1.17, 2300, 3500,  7000,  7000, Currency.TRY, AcIds.TCJHM, AirlineIds.THY, SchIds.TK9_IST_ESB);
            AddFlights("TK11",  21, 18,   1.17, 2300, 3500,  7000,  7000, Currency.TRY, AcIds.TCKUE, AirlineIds.THY, SchIds.TK11_IST_ESB);
            AddFlights("TK1793",21, 7,    1.33, 2500, 3800,  8000,  8000, Currency.TRY, AcIds.TCKUE, AirlineIds.THY, SchIds.TK1793_IST_ADB);
            AddFlights("TK1795",21, 12.5, 1.33, 2500, 3800,  8000,  8000, Currency.TRY, AcIds.TCJHM, AirlineIds.THY, SchIds.TK1795_IST_ADB);
            AddFlights("TK1797",21, 18.5, 1.33, 2500, 3800,  8000,  8000, Currency.TRY, AcIds.TCKUE, AirlineIds.THY, SchIds.TK1797_IST_ADB);
            AddFlights("TK2120",21, 8,    1.5,  2800, 4200,  9000,  9000, Currency.TRY, AcIds.TCJHM, AirlineIds.THY, SchIds.TK2120_IST_AYT);
            AddFlights("TK2122",21, 14,   1.5,  2800, 4200,  9000,  9000, Currency.TRY, AcIds.TCKUE, AirlineIds.THY, SchIds.TK2122_IST_AYT);
            AddFlights("TK2124",21, 20,   1.5,  2800, 4200,  9000,  9000, Currency.TRY, AcIds.TCJHM, AirlineIds.THY, SchIds.TK2124_IST_AYT);

            // ── Pegasus domestic ──────────────────────────────────────────────
            AddFlights("PC3150",21, 9,    1.5,  1299, 1299,  4500,  4500, Currency.TRY, AcIds.YBBSA, AirlineIds.PC, SchIds.PC3150_IST_AYT);
            AddFlights("PC3152",21, 17,   1.5,  1299, 1299,  4500,  4500, Currency.TRY, AcIds.YBBSB, AirlineIds.PC, SchIds.PC3152_IST_AYT);

            // ── Turkish Airlines IST→Europe ───────────────────────────────────
            AddFlights("TK1",  21, 8,    4,    450, 750,  1800, 3500, Currency.USD, AcIds.TCLNA, AirlineIds.THY, SchIds.TK1_IST_LHR);
            AddFlights("TK3",  21, 16,   4,    450, 750,  1800, 3500, Currency.USD, AcIds.TCJJK, AirlineIds.THY, SchIds.TK3_IST_LHR);
            AddFlights("TK11", 21, 10,   4.5,  380, 650,  1600, 3200, Currency.USD, AcIds.TCLNA, AirlineIds.THY, SchIds.TK11_IST_DXB);
            AddFlights("TK13", 21, 18,   4.5,  380, 650,  1600, 3200, Currency.USD, AcIds.TCJJK, AirlineIds.THY, SchIds.TK13_IST_DXB);
            AddFlights("TK751",21, 9,    3.5,  320, 540,  1400, 2800, Currency.EUR, AcIds.TCKUE, AirlineIds.THY, SchIds.TK751_IST_FRA);
            AddFlights("TK1841",21,8,    3.75, 310, 520,  1350, 2700, Currency.EUR, AcIds.TCJHM, AirlineIds.THY, SchIds.TK1841_IST_CDG);
            AddFlights("TK2010",21,10,   3.5,  300, 510,  1350, 2700, Currency.EUR, AcIds.TCKUE, AirlineIds.THY, SchIds.TK2010_IST_AMS);
            AddFlights("TK1881",21,9.5,  3.25, 290, 490,  1300, 2600, Currency.EUR, AcIds.TCJHM, AirlineIds.THY, SchIds.TK1881_IST_FCO);
            AddFlights("TK1851",21,10,   4.5,  380, 640,  1600, 3200, Currency.EUR, AcIds.TCKUE, AirlineIds.THY, SchIds.TK1851_IST_MAD);
            AddFlights("TK3",  21, 23.5, 10.5, 650, 1100, 2800, 5500, Currency.USD, AcIds.TCLJT, AirlineIds.THY, SchIds.TK3_IST_JFK);
            AddFlights("TK203",21, 22,   14,   720, 1250, 3500, 7000, Currency.USD, AcIds.TCLJT, AirlineIds.THY, SchIds.TK203_IST_LAX);

            // ── Return flights to IST ─────────────────────────────────────────
            AddFlights("TK2",  21, 11,   4,    420, 720,  1750, 3400, Currency.USD, AcIds.TCLNA, AirlineIds.THY, SchIds.TK2_LHR_IST);
            AddFlights("TK12", 21, 8,    4.5,  360, 620,  1550, 3100, Currency.USD, AcIds.TCJJK, AirlineIds.THY, SchIds.TK12_DXB_IST);

            // ── Lufthansa ─────────────────────────────────────────────────────
            AddFlights("LH1290",21,10,   3.5,  310, 520,  1400, 2800, Currency.EUR, AcIds.DAIXA, AirlineIds.LH, SchIds.LH1290_FRA_IST);
            AddFlights("LH400", 21,10.5, 9,    580, 980,  2600, 5200, Currency.EUR, AcIds.DAIXC, AirlineIds.LH, SchIds.LH400_FRA_JFK);

            // ── British Airways ───────────────────────────────────────────────
            AddFlights("BA686", 21,14,   4,    440, 740,  1750, 3500, Currency.GBP, AcIds.GXWBA, AirlineIds.BA, SchIds.BA686_LHR_IST);
            AddFlights("BA175", 21,11,   7.5,  560, 950,  2500, 5000, Currency.GBP, AcIds.GXWBB, AirlineIds.BA, SchIds.BA175_LHR_JFK);
            AddFlights("BA177", 21,15,   7.5,  560, 950,  2500, 5000, Currency.GBP, AcIds.GXWBA, AirlineIds.BA, SchIds.BA177_LHR_JFK);

            // ── Air France ────────────────────────────────────────────────────
            AddFlights("AF1471",21,8,    3.75, 300, 510,  1350, 2700, Currency.EUR, AcIds.FGZNX, AirlineIds.AF, SchIds.AF1471_CDG_IST);

            // ── Emirates ──────────────────────────────────────────────────────
            AddFlights("EK1",   21, 8,    7.5,  620, 1050, 2800, 5600, Currency.USD, AcIds.A6EWC, AirlineIds.EK, SchIds.EK1_DXB_LHR);
            AddFlights("EK3",   21, 14.5, 7.5,  620, 1050, 2800, 5600, Currency.USD, AcIds.A6EDM, AirlineIds.EK, SchIds.EK3_DXB_LHR);
            AddFlights("EK352", 21, 2.5,  7.5,  550, 940,  2500, 5000, Currency.USD, AcIds.A6EWC, AirlineIds.EK, SchIds.EK352_DXB_SIN);
            AddFlights("EK354", 21, 14,   7.5,  550, 940,  2500, 5000, Currency.USD, AcIds.A6ENB, AirlineIds.EK, SchIds.EK354_DXB_SIN);
            AddFlights("EK201", 21, 2,    14,   780, 1350, 3600, 7200, Currency.USD, AcIds.A6EWC, AirlineIds.EK, SchIds.EK201_DXB_JFK);
            AddFlights("EK2",   21, 9,    7,    600, 1020, 2700, 5400, Currency.USD, AcIds.A6EDB, AirlineIds.EK, SchIds.EK2_LHR_DXB);
            AddFlights("EK374", 21, 3.5,  6.5,  490, 840,  2200, 4400, Currency.USD, AcIds.A6ENB, AirlineIds.EK, SchIds.EK374_DXB_BKK);

            // ── Turkish Airlines AMS return ───────────────────────────────────
            AddFlights("TK752", 21, 14,   3.5,  300, 510,  1350, 2700, Currency.EUR, AcIds.TCKUE, AirlineIds.THY, SchIds.TK752_AMS_IST);

            db.Flights.AddRange(flights);
            await db.SaveChangesAsync();
        }

        // ── ID Constants ──────────────────────────────────────────────────────────

        private static class MfgIds
        {
            public static readonly Guid Boeing     = new("10000000-0000-0000-0000-000000000001");
            public static readonly Guid Airbus     = new("10000000-0000-0000-0000-000000000002");
            public static readonly Guid Bombardier = new("10000000-0000-0000-0000-000000000003");
        }

        private static class ModelIds
        {
            public static readonly Guid B737800   = new("20000000-0000-0000-0000-000000000001");
            public static readonly Guid B777300ER = new("20000000-0000-0000-0000-000000000002");
            public static readonly Guid B7879     = new("20000000-0000-0000-0000-000000000003");
            public static readonly Guid A320Neo   = new("20000000-0000-0000-0000-000000000004");
            public static readonly Guid A350900   = new("20000000-0000-0000-0000-000000000005");
            public static readonly Guid A380800   = new("20000000-0000-0000-0000-000000000006");
        }

        private static class AirlineIds
        {
            public static readonly Guid THY = new("30000000-0000-0000-0000-000000000001");
            public static readonly Guid EK  = new("30000000-0000-0000-0000-000000000002");
            public static readonly Guid PC  = new("30000000-0000-0000-0000-000000000003");
            public static readonly Guid LH  = new("30000000-0000-0000-0000-000000000004");
            public static readonly Guid BA  = new("30000000-0000-0000-0000-000000000005");
            public static readonly Guid AF  = new("30000000-0000-0000-0000-000000000006");
        }

        private static class AptIds
        {
            public static readonly Guid IST = new("40000000-0000-0000-0000-000000000001");
            public static readonly Guid SAW = new("40000000-0000-0000-0000-000000000002");
            public static readonly Guid ESB = new("40000000-0000-0000-0000-000000000003");
            public static readonly Guid ADB = new("40000000-0000-0000-0000-000000000004");
            public static readonly Guid AYT = new("40000000-0000-0000-0000-000000000005");
            public static readonly Guid LHR = new("40000000-0000-0000-0000-000000000006");
            public static readonly Guid DXB = new("40000000-0000-0000-0000-000000000007");
            public static readonly Guid JFK = new("40000000-0000-0000-0000-000000000008");
            public static readonly Guid FRA = new("40000000-0000-0000-0000-000000000009");
            public static readonly Guid CDG = new("40000000-0000-0000-0000-000000000010");
            public static readonly Guid AMS = new("40000000-0000-0000-0000-000000000011");
            public static readonly Guid MUC = new("40000000-0000-0000-0000-000000000012");
            public static readonly Guid MAD = new("40000000-0000-0000-0000-000000000013");
            public static readonly Guid FCO = new("40000000-0000-0000-0000-000000000014");
            public static readonly Guid SIN = new("40000000-0000-0000-0000-000000000015");
            public static readonly Guid DOH = new("40000000-0000-0000-0000-000000000016");
            public static readonly Guid LAX = new("40000000-0000-0000-0000-000000000017");
            public static readonly Guid ORD = new("40000000-0000-0000-0000-000000000018");
            public static readonly Guid BKK = new("40000000-0000-0000-0000-000000000019");
        }

        private static class RouteIds
        {
            public static readonly Guid IST_ESB = new("50000000-0000-0000-0000-000000000001");
            public static readonly Guid IST_ADB = new("50000000-0000-0000-0000-000000000002");
            public static readonly Guid IST_AYT = new("50000000-0000-0000-0000-000000000003");
            public static readonly Guid IST_LHR = new("50000000-0000-0000-0000-000000000004");
            public static readonly Guid IST_DXB = new("50000000-0000-0000-0000-000000000005");
            public static readonly Guid IST_FRA = new("50000000-0000-0000-0000-000000000006");
            public static readonly Guid IST_CDG = new("50000000-0000-0000-0000-000000000007");
            public static readonly Guid IST_AMS = new("50000000-0000-0000-0000-000000000008");
            public static readonly Guid IST_FCO = new("50000000-0000-0000-0000-000000000009");
            public static readonly Guid IST_MAD = new("50000000-0000-0000-0000-000000000010");
            public static readonly Guid IST_JFK = new("50000000-0000-0000-0000-000000000011");
            public static readonly Guid IST_LAX = new("50000000-0000-0000-0000-000000000012");
            public static readonly Guid LHR_IST = new("50000000-0000-0000-0000-000000000013");
            public static readonly Guid DXB_IST = new("50000000-0000-0000-0000-000000000014");
            public static readonly Guid FRA_IST = new("50000000-0000-0000-0000-000000000015");
            public static readonly Guid CDG_IST = new("50000000-0000-0000-0000-000000000016");
            public static readonly Guid AMS_IST = new("50000000-0000-0000-0000-000000000017");
            public static readonly Guid DXB_LHR = new("50000000-0000-0000-0000-000000000018");
            public static readonly Guid DXB_SIN = new("50000000-0000-0000-0000-000000000019");
            public static readonly Guid DXB_JFK = new("50000000-0000-0000-0000-000000000020");
            public static readonly Guid DXB_BKK = new("50000000-0000-0000-0000-000000000021");
            public static readonly Guid LHR_DXB = new("50000000-0000-0000-0000-000000000022");
            public static readonly Guid FRA_JFK = new("50000000-0000-0000-0000-000000000023");
            public static readonly Guid LHR_JFK = new("50000000-0000-0000-0000-000000000024");
        }

        private static class SchIds
        {
            // THY domestic
            public static readonly Guid TK7_IST_ESB    = new("60000000-0000-0000-0000-000000000001");
            public static readonly Guid TK9_IST_ESB    = new("60000000-0000-0000-0000-000000000002");
            public static readonly Guid TK11_IST_ESB   = new("60000000-0000-0000-0000-000000000003");
            public static readonly Guid TK1793_IST_ADB = new("60000000-0000-0000-0000-000000000004");
            public static readonly Guid TK1795_IST_ADB = new("60000000-0000-0000-0000-000000000005");
            public static readonly Guid TK1797_IST_ADB = new("60000000-0000-0000-0000-000000000006");
            public static readonly Guid TK2120_IST_AYT = new("60000000-0000-0000-0000-000000000007");
            public static readonly Guid TK2122_IST_AYT = new("60000000-0000-0000-0000-000000000008");
            public static readonly Guid TK2124_IST_AYT = new("60000000-0000-0000-0000-000000000009");
            public static readonly Guid PC3150_IST_AYT = new("60000000-0000-0000-0000-000000000010");
            public static readonly Guid PC3152_IST_AYT = new("60000000-0000-0000-0000-000000000011");
            // THY international
            public static readonly Guid TK1_IST_LHR    = new("60000000-0000-0000-0000-000000000012");
            public static readonly Guid TK3_IST_LHR    = new("60000000-0000-0000-0000-000000000013");
            public static readonly Guid TK11_IST_DXB   = new("60000000-0000-0000-0000-000000000014");
            public static readonly Guid TK13_IST_DXB   = new("60000000-0000-0000-0000-000000000015");
            public static readonly Guid TK751_IST_FRA  = new("60000000-0000-0000-0000-000000000016");
            public static readonly Guid TK1841_IST_CDG = new("60000000-0000-0000-0000-000000000017");
            public static readonly Guid TK2010_IST_AMS = new("60000000-0000-0000-0000-000000000018");
            public static readonly Guid TK1881_IST_FCO = new("60000000-0000-0000-0000-000000000019");
            public static readonly Guid TK1851_IST_MAD = new("60000000-0000-0000-0000-000000000020");
            public static readonly Guid TK3_IST_JFK    = new("60000000-0000-0000-0000-000000000021");
            public static readonly Guid TK203_IST_LAX  = new("60000000-0000-0000-0000-000000000022");
            // Returns
            public static readonly Guid TK2_LHR_IST    = new("60000000-0000-0000-0000-000000000023");
            public static readonly Guid TK12_DXB_IST   = new("60000000-0000-0000-0000-000000000024");
            public static readonly Guid LH1290_FRA_IST = new("60000000-0000-0000-0000-000000000025");
            public static readonly Guid AF1471_CDG_IST = new("60000000-0000-0000-0000-000000000026");
            public static readonly Guid BA686_LHR_IST  = new("60000000-0000-0000-0000-000000000027");
            public static readonly Guid TK752_AMS_IST  = new("60000000-0000-0000-0000-000000000028");
            // Emirates
            public static readonly Guid EK1_DXB_LHR    = new("60000000-0000-0000-0000-000000000029");
            public static readonly Guid EK3_DXB_LHR    = new("60000000-0000-0000-0000-000000000030");
            public static readonly Guid EK352_DXB_SIN  = new("60000000-0000-0000-0000-000000000031");
            public static readonly Guid EK354_DXB_SIN  = new("60000000-0000-0000-0000-000000000032");
            public static readonly Guid EK201_DXB_JFK  = new("60000000-0000-0000-0000-000000000033");
            public static readonly Guid EK2_LHR_DXB    = new("60000000-0000-0000-0000-000000000034");
            public static readonly Guid EK374_DXB_BKK  = new("60000000-0000-0000-0000-000000000035");
            // LH / BA trans-Atlantic
            public static readonly Guid LH400_FRA_JFK  = new("60000000-0000-0000-0000-000000000036");
            public static readonly Guid BA175_LHR_JFK  = new("60000000-0000-0000-0000-000000000037");
            public static readonly Guid BA177_LHR_JFK  = new("60000000-0000-0000-0000-000000000038");
        }

        private static class AcIds
        {
            // Turkish Airlines
            public static readonly Guid TCLNA = new("70000000-0000-0000-0000-000000000001");
            public static readonly Guid TCKUE = new("70000000-0000-0000-0000-000000000002");
            public static readonly Guid TCJJK = new("70000000-0000-0000-0000-000000000003");
            public static readonly Guid TCLJT = new("70000000-0000-0000-0000-000000000004");
            public static readonly Guid TCJHM = new("70000000-0000-0000-0000-000000000005");
            // Emirates
            public static readonly Guid A6EDB = new("70000000-0000-0000-0000-000000000006");
            public static readonly Guid A6EWC = new("70000000-0000-0000-0000-000000000007");
            public static readonly Guid A6EDM = new("70000000-0000-0000-0000-000000000008");
            public static readonly Guid A6ENB = new("70000000-0000-0000-0000-000000000009");
            // Pegasus
            public static readonly Guid YBBSA = new("70000000-0000-0000-0000-000000000010");
            public static readonly Guid YBBSB = new("70000000-0000-0000-0000-000000000011");
            public static readonly Guid YBBSC = new("70000000-0000-0000-0000-000000000012");
            // Lufthansa
            public static readonly Guid DAIXA = new("70000000-0000-0000-0000-000000000013");
            public static readonly Guid DAIXB = new("70000000-0000-0000-0000-000000000014");
            public static readonly Guid DAIXC = new("70000000-0000-0000-0000-000000000015");
            // British Airways
            public static readonly Guid GXWBA = new("70000000-0000-0000-0000-000000000016");
            public static readonly Guid GEUXA = new("70000000-0000-0000-0000-000000000017");
            public static readonly Guid GXWBB = new("70000000-0000-0000-0000-000000000018");
            // Air France
            public static readonly Guid FGZNX = new("70000000-0000-0000-0000-000000000019");
            public static readonly Guid FHPJH = new("70000000-0000-0000-0000-000000000020");
            public static readonly Guid FGKXA = new("70000000-0000-0000-0000-000000000021");
        }
    }
}
