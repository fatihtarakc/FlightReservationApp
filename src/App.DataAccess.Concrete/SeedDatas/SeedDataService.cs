namespace App.DataAccess.Concrete.SeedDatas
{
    public static class SeedDataService
    {
        private const string AdminEmail = "admin@flightreservation.com";
        private const string AdminUsername = "admin";
        private const string AdminPassword = "Admin2026+-!?";

        private const string TestAdminEmail = "testadmin@flightreservation.com";
        private const string TestAdminUsername = "testadmin";
        private const string TestAdminPassword = "TestAdmin2026+-!?";

        private const string TestUserEmail = "testuser@flightreservation.com";
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

        private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            string[] roles = { "Admin", "AppUser" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        private static async Task SeedAdminAsync(FlightReservationDbContext db, UserManager<IdentityUser> userManager)
        {
            if (await userManager.FindByEmailAsync(AdminEmail) != null) return;

            var identityUser = new IdentityUser
            {
                Email = AdminEmail,
                UserName = AdminUsername,
                NormalizedEmail = AdminEmail.ToUpperInvariant(),
                NormalizedUserName = AdminUsername.ToUpperInvariant(),
                EmailConfirmed = true
            };
            await userManager.CreateAsync(identityUser, AdminPassword);
            await userManager.AddToRoleAsync(identityUser, "Admin");

            if (!await db.Admins.AnyAsync(a => a.Email == AdminEmail))
            {
                db.Admins.Add(new Admin
                {
                    Name = "Super",
                    Surname = "Admin",
                    Email = AdminEmail,
                    IdentityId = identityUser.Id,
                    CreatedBy = "system"
                });
                await db.SaveChangesAsync();
            }
        }

        private static async Task SeedTestAdminAsync(FlightReservationDbContext db, UserManager<IdentityUser> userManager)
        {
            if (await userManager.FindByEmailAsync(TestAdminEmail) != null) return;

            var identityUser = new IdentityUser
            {
                Email = TestAdminEmail,
                UserName = TestAdminUsername,
                NormalizedEmail = TestAdminEmail.ToUpperInvariant(),
                NormalizedUserName = TestAdminUsername.ToUpperInvariant(),
                EmailConfirmed = true
            };
            await userManager.CreateAsync(identityUser, TestAdminPassword);
            await userManager.AddToRoleAsync(identityUser, "Admin");

            if (!await db.Admins.AnyAsync(a => a.Email == TestAdminEmail))
            {
                db.Admins.Add(new Admin
                {
                    Name = "Test",
                    Surname = "Admin",
                    Email = TestAdminEmail,
                    IdentityId = identityUser.Id,
                    IsSuperAdmin = false,
                    CreatedBy = "system"
                });
                await db.SaveChangesAsync();
            }
        }

        private static async Task SeedTestAppUserAsync(FlightReservationDbContext db, UserManager<IdentityUser> userManager)
        {
            if (await userManager.FindByEmailAsync(TestUserEmail) != null) return;

            var identityUser = new IdentityUser
            {
                Email = TestUserEmail,
                UserName = TestUserUsername,
                NormalizedEmail = TestUserEmail.ToUpperInvariant(),
                NormalizedUserName = TestUserUsername.ToUpperInvariant(),
                EmailConfirmed = true
            };
            await userManager.CreateAsync(identityUser, TestUserPassword);
            await userManager.AddToRoleAsync(identityUser, "AppUser");

            if (!await db.AppUsers.AnyAsync(u => u.Email == TestUserEmail))
            {
                db.AppUsers.Add(new AppUser
                {
                    Name = "Test",
                    Surname = "User",
                    Email = TestUserEmail,
                    PhoneNumber = "+905001234567",
                    BirthDate = new DateTime(1996, 6, 15, 0, 0, 0, DateTimeKind.Utc),
                    UserStatus = UserStatus.Active,
                    PreferredNotificationChannel = NotificationChannel.Email,
                    Nationality = "TR",
                    IdentityId = identityUser.Id,
                    CreatedBy = "system"
                });
                await db.SaveChangesAsync();
            }
        }

        private static async Task SeedManufacturersAsync(FlightReservationDbContext db)
        {
            if (await db.Manufacturers.AnyAsync()) return;

            db.Manufacturers.AddRange(
                new Manufacturer { Id = ManufacturerIds.Boeing, Name = "Boeing", Country = "United States", CreatedBy = "system" },
                new Manufacturer { Id = ManufacturerIds.Airbus, Name = "Airbus", Country = "France", CreatedBy = "system" },
                new Manufacturer { Id = ManufacturerIds.Bombardier, Name = "Bombardier", Country = "Canada", CreatedBy = "system" }
            );
            await db.SaveChangesAsync();
        }

        private static async Task SeedModelsAsync(FlightReservationDbContext db)
        {
            if (await db.Models.AnyAsync()) return;

            db.Models.AddRange(
                new Model
                {
                    Id = ModelIds.B737800, ManufacturerId = ManufacturerIds.Boeing,
                    Name = "737-800", BodyType = BodyType.NarrowBody,
                    MaxPassengerCapacity = 189, EconomySeats = 162, PremiumEconomySeats = 0, BusinessSeats = 16, FirstClassSeats = 8,
                    MaxRangeKm = 5765, CreatedBy = "system"
                },
                new Model
                {
                    Id = ModelIds.B777300ER, ManufacturerId = ManufacturerIds.Boeing,
                    Name = "777-300ER", BodyType = BodyType.WideBody,
                    MaxPassengerCapacity = 396, EconomySeats = 304, PremiumEconomySeats = 40, BusinessSeats = 42, FirstClassSeats = 8,
                    MaxRangeKm = 13650, CreatedBy = "system"
                },
                new Model
                {
                    Id = ModelIds.A320Neo, ManufacturerId = ManufacturerIds.Airbus,
                    Name = "A320neo", BodyType = BodyType.NarrowBody,
                    MaxPassengerCapacity = 194, EconomySeats = 162, PremiumEconomySeats = 0, BusinessSeats = 20, FirstClassSeats = 0,
                    MaxRangeKm = 6300, CreatedBy = "system"
                },
                new Model
                {
                    Id = ModelIds.A350900, ManufacturerId = ManufacturerIds.Airbus,
                    Name = "A350-900", BodyType = BodyType.WideBody,
                    MaxPassengerCapacity = 440, EconomySeats = 340, PremiumEconomySeats = 48, BusinessSeats = 40, FirstClassSeats = 8,
                    MaxRangeKm = 15000, CreatedBy = "system"
                }
            );
            await db.SaveChangesAsync();
        }

        private static async Task SeedAirlinesAsync(FlightReservationDbContext db)
        {
            if (await db.Airlines.AnyAsync()) return;

            db.Airlines.AddRange(
                new Airline
                {
                    Id = AirlineIds.TurkishAirlines, Name = "Turkish Airlines",
                    IataCode = "TK", IcaoCode = "THY", Country = "Turkey",
                    Website = "https://www.turkishairlines.com", CreatedBy = "system"
                },
                new Airline
                {
                    Id = AirlineIds.Emirates, Name = "Emirates",
                    IataCode = "EK", IcaoCode = "UAE", Country = "United Arab Emirates",
                    Website = "https://www.emirates.com", CreatedBy = "system"
                },
                new Airline
                {
                    Id = AirlineIds.Pegasus, Name = "Pegasus Airlines",
                    IataCode = "PC", IcaoCode = "PGT", Country = "Turkey",
                    Website = "https://www.flypgs.com", CreatedBy = "system"
                }
            );
            await db.SaveChangesAsync();
        }

        private static async Task SeedAirportsAsync(FlightReservationDbContext db)
        {
            if (await db.Airports.AnyAsync()) return;

            db.Airports.AddRange(
                new Airport { Id = AirportIds.IST, Name = "Istanbul Airport", IataCode = "IST", IcaoCode = "LTFM", City = "Istanbul", Country = "Turkey", TimeZone = "Europe/Istanbul", CreatedBy = "system" },
                new Airport { Id = AirportIds.SAW, Name = "Sabiha Gokcen Airport", IataCode = "SAW", IcaoCode = "LTFJ", City = "Istanbul", Country = "Turkey", TimeZone = "Europe/Istanbul", CreatedBy = "system" },
                new Airport { Id = AirportIds.ESB, Name = "Esenboga Airport", IataCode = "ESB", IcaoCode = "LTAC", City = "Ankara", Country = "Turkey", TimeZone = "Europe/Istanbul", CreatedBy = "system" },
                new Airport { Id = AirportIds.ADB, Name = "Adnan Menderes Airport", IataCode = "ADB", IcaoCode = "LTBJ", City = "Izmir", Country = "Turkey", TimeZone = "Europe/Istanbul", CreatedBy = "system" },
                new Airport { Id = AirportIds.AYT, Name = "Antalya Airport", IataCode = "AYT", IcaoCode = "LTAI", City = "Antalya", Country = "Turkey", TimeZone = "Europe/Istanbul", CreatedBy = "system" },
                new Airport { Id = AirportIds.LHR, Name = "Heathrow Airport", IataCode = "LHR", IcaoCode = "EGLL", City = "London", Country = "United Kingdom", TimeZone = "Europe/London", CreatedBy = "system" },
                new Airport { Id = AirportIds.DXB, Name = "Dubai International Airport", IataCode = "DXB", IcaoCode = "OMDB", City = "Dubai", Country = "United Arab Emirates", TimeZone = "Asia/Dubai", CreatedBy = "system" },
                new Airport { Id = AirportIds.JFK, Name = "John F. Kennedy Airport", IataCode = "JFK", IcaoCode = "KJFK", City = "New York", Country = "United States", TimeZone = "America/New_York", CreatedBy = "system" },
                new Airport { Id = AirportIds.FRA, Name = "Frankfurt Airport", IataCode = "FRA", IcaoCode = "EDDF", City = "Frankfurt", Country = "Germany", TimeZone = "Europe/Berlin", CreatedBy = "system" },
                new Airport { Id = AirportIds.CDG, Name = "Charles de Gaulle Airport", IataCode = "CDG", IcaoCode = "LFPG", City = "Paris", Country = "France", TimeZone = "Europe/Paris", CreatedBy = "system" }
            );
            await db.SaveChangesAsync();
        }

        private static async Task SeedRoutesAsync(FlightReservationDbContext db)
        {
            if (await db.Routes.AnyAsync()) return;

            db.Routes.AddRange(
                new Route { Id = RouteIds.IST_LHR, DepartureAirportId = AirportIds.IST, ArrivalAirportId = AirportIds.LHR, DistanceKm = 2514, EstimatedDuration = TimeSpan.FromHours(4), CreatedBy = "system" },
                new Route { Id = RouteIds.IST_DXB, DepartureAirportId = AirportIds.IST, ArrivalAirportId = AirportIds.DXB, DistanceKm = 2573, EstimatedDuration = TimeSpan.FromHours(4.5), CreatedBy = "system" },
                new Route { Id = RouteIds.IST_JFK, DepartureAirportId = AirportIds.IST, ArrivalAirportId = AirportIds.JFK, DistanceKm = 9377, EstimatedDuration = TimeSpan.FromHours(10.5), CreatedBy = "system" },
                new Route { Id = RouteIds.IST_FRA, DepartureAirportId = AirportIds.IST, ArrivalAirportId = AirportIds.FRA, DistanceKm = 1869, EstimatedDuration = TimeSpan.FromHours(3.5), CreatedBy = "system" },
                new Route { Id = RouteIds.IST_CDG, DepartureAirportId = AirportIds.IST, ArrivalAirportId = AirportIds.CDG, DistanceKm = 2244, EstimatedDuration = TimeSpan.FromHours(3.75), CreatedBy = "system" },
                new Route { Id = RouteIds.IST_ESB, DepartureAirportId = AirportIds.IST, ArrivalAirportId = AirportIds.ESB, DistanceKm = 352, EstimatedDuration = TimeSpan.FromMinutes(70), CreatedBy = "system" },
                new Route { Id = RouteIds.IST_ADB, DepartureAirportId = AirportIds.IST, ArrivalAirportId = AirportIds.ADB, DistanceKm = 445, EstimatedDuration = TimeSpan.FromMinutes(80), CreatedBy = "system" },
                new Route { Id = RouteIds.IST_AYT, DepartureAirportId = AirportIds.IST, ArrivalAirportId = AirportIds.AYT, DistanceKm = 555, EstimatedDuration = TimeSpan.FromMinutes(90), CreatedBy = "system" },
                new Route { Id = RouteIds.LHR_IST, DepartureAirportId = AirportIds.LHR, ArrivalAirportId = AirportIds.IST, DistanceKm = 2514, EstimatedDuration = TimeSpan.FromHours(4), CreatedBy = "system" },
                new Route { Id = RouteIds.DXB_IST, DepartureAirportId = AirportIds.DXB, ArrivalAirportId = AirportIds.IST, DistanceKm = 2573, EstimatedDuration = TimeSpan.FromHours(4.5), CreatedBy = "system" }
            );
            await db.SaveChangesAsync();
        }

        private static async Task SeedSchedulesAsync(FlightReservationDbContext db)
        {
            if (await db.Schedules.AnyAsync()) return;

            db.Schedules.AddRange(
                new Schedule { Id = ScheduleIds.TK1_IST_LHR, RouteId = RouteIds.IST_LHR, Code = "TK1-IST-LHR", ValidFrom = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc), DaysOfWeek = DaysOfWeek.Monday | DaysOfWeek.Wednesday | DaysOfWeek.Friday | DaysOfWeek.Sunday, DepartureTime = TimeSpan.FromHours(8), TimeZone = "Europe/Istanbul", CreatedBy = "system" },
                new Schedule { Id = ScheduleIds.TK11_IST_DXB, RouteId = RouteIds.IST_DXB, Code = "TK11-IST-DXB", ValidFrom = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc), DaysOfWeek = DaysOfWeek.Tuesday | DaysOfWeek.Thursday | DaysOfWeek.Saturday, DepartureTime = TimeSpan.FromHours(10), TimeZone = "Europe/Istanbul", CreatedBy = "system" },
                new Schedule { Id = ScheduleIds.TK3_IST_JFK, RouteId = RouteIds.IST_JFK, Code = "TK3-IST-JFK", ValidFrom = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc), DaysOfWeek = DaysOfWeek.Daily, DepartureTime = TimeSpan.FromHours(23.5), TimeZone = "Europe/Istanbul", CreatedBy = "system" },
                new Schedule { Id = ScheduleIds.TK1793_IST_ADB, RouteId = RouteIds.IST_ADB, Code = "TK1793-IST-ADB", ValidFrom = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc), DaysOfWeek = DaysOfWeek.Daily, DepartureTime = TimeSpan.FromHours(7), TimeZone = "Europe/Istanbul", CreatedBy = "system" },
                new Schedule { Id = ScheduleIds.TK2120_IST_AYT, RouteId = RouteIds.IST_AYT, Code = "TK2120-IST-AYT", ValidFrom = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc), DaysOfWeek = DaysOfWeek.Daily, DepartureTime = TimeSpan.FromHours(9), TimeZone = "Europe/Istanbul", CreatedBy = "system" }
            );
            await db.SaveChangesAsync();
        }

        private static async Task SeedAircraftsAsync(FlightReservationDbContext db)
        {
            if (await db.Aircrafts.AnyAsync()) return;

            db.Aircrafts.AddRange(
                new Aircraft { Id = AircraftIds.TCLNA, TailNumber = "TCLNA", ManufactureYear = 2018, AircraftStatus = AircraftStatus.Active, AirlineId = AirlineIds.TurkishAirlines, ModelId = ModelIds.B777300ER, CreatedBy = "system" },
                new Aircraft { Id = AircraftIds.TCKUE, TailNumber = "TCKUE", ManufactureYear = 2020, AircraftStatus = AircraftStatus.Active, AirlineId = AirlineIds.TurkishAirlines, ModelId = ModelIds.A320Neo, CreatedBy = "system" },
                new Aircraft { Id = AircraftIds.A6EDB, TailNumber = "A6EDB", ManufactureYear = 2016, AircraftStatus = AircraftStatus.Active, AirlineId = AirlineIds.Emirates, ModelId = ModelIds.A350900, CreatedBy = "system" },
                new Aircraft { Id = AircraftIds.YBBSA, TailNumber = "YBBSA", ManufactureYear = 2019, AircraftStatus = AircraftStatus.Active, AirlineId = AirlineIds.Pegasus, ModelId = ModelIds.A320Neo, CreatedBy = "system" }
            );
            await db.SaveChangesAsync();
        }

        private static async Task SeedSeatsAsync(FlightReservationDbContext db)
        {
            if (await db.Seats.AnyAsync()) return;

            var seats = new List<Seat>();

            // B777-300ER for TCLNA: 8 First, 42 Business, 40 Premium Economy, 306 Economy
            GenerateSeats(seats, AircraftIds.TCLNA,
                firstRows: 2, businessRows: 7, premiumRows: 5, economyRows: 34);

            // A320neo for TCKUE: 20 Business, 162 Economy (no first/premium)
            GenerateSeats(seats, AircraftIds.TCKUE,
                firstRows: 0, businessRows: 4, premiumRows: 0, economyRows: 27);

            // A350-900 for A6EDB: 8 First, 40 Business, 48 Premium, 344 Economy
            GenerateSeats(seats, AircraftIds.A6EDB,
                firstRows: 2, businessRows: 7, premiumRows: 6, economyRows: 43);

            // A320neo for YBBSA
            GenerateSeats(seats, AircraftIds.YBBSA,
                firstRows: 0, businessRows: 4, premiumRows: 0, economyRows: 27);

            db.Seats.AddRange(seats);
            await db.SaveChangesAsync();
        }

        private static void GenerateSeats(List<Seat> seats, Guid aircraftId,
            int firstRows, int businessRows, int premiumRows, int economyRows)
        {
            int row = 1;
            var columns = new[] { SeatColumn.A, SeatColumn.B, SeatColumn.C, SeatColumn.D, SeatColumn.E, SeatColumn.F };

            Action<int, int, SeatClass> addRows = (startRow, count, seatClass) =>
            {
                var cols = seatClass == SeatClass.First
                    ? new[] { SeatColumn.A, SeatColumn.B, SeatColumn.C, SeatColumn.D }
                    : columns;
                for (int r = startRow; r < startRow + count; r++)
                    foreach (var col in cols)
                        seats.Add(new Seat
                        {
                            AircraftId = aircraftId,
                            Row = r, Column = col, SeatClass = seatClass,
                            IsWindowSeat = col == SeatColumn.A || col == SeatColumn.F,
                            IsAisleSeat = col == SeatColumn.C || col == SeatColumn.D,
                            HasExtraLegRoom = r == startRow,
                            CreatedBy = "system"
                        });
            };

            if (firstRows > 0) { addRows(row, firstRows, SeatClass.First); row += firstRows; }
            if (businessRows > 0) { addRows(row, businessRows, SeatClass.Business); row += businessRows; }
            if (premiumRows > 0) { addRows(row, premiumRows, SeatClass.PremiumEconomy); row += premiumRows; }
            if (economyRows > 0) { addRows(row, economyRows, SeatClass.Economy); }
        }

        private static async Task SeedFlightsAsync(FlightReservationDbContext db)
        {
            if (await db.Flights.AnyAsync()) return;

            var today = DateTime.UtcNow.Date;
            var flights = new List<Flight>();

            // IST → LHR
            for (int i = 1; i <= 14; i++)
            {
                var dep = today.AddDays(i).AddHours(8);
                flights.Add(new Flight
                {
                    Number = $"TK{(1).ToString().PadLeft(3, '0')}", DepartureDateTime = dep, ArrivalDateTime = dep.AddHours(4),
                    Duration = TimeSpan.FromHours(4), BaseEconomyPrice = 450, BasePremiumEconomyPrice = 750, BaseBusinessPrice = 1800, BaseFirstClassPrice = 3500,
                    Currency = Currency.USD, FlightStatus = FlightStatus.Scheduled,
                    AircraftId = AircraftIds.TCLNA, AirlineId = AirlineIds.TurkishAirlines, ScheduleId = ScheduleIds.TK1_IST_LHR, CreatedBy = "system"
                });
            }

            // IST → DXB
            for (int i = 1; i <= 14; i++)
            {
                var dep = today.AddDays(i).AddHours(10);
                flights.Add(new Flight
                {
                    Number = $"TK{(11).ToString().PadLeft(3, '0')}", DepartureDateTime = dep, ArrivalDateTime = dep.AddHours(4).AddMinutes(30),
                    Duration = TimeSpan.FromHours(4.5), BaseEconomyPrice = 350, BasePremiumEconomyPrice = 600, BaseBusinessPrice = 1500, BaseFirstClassPrice = 3000,
                    Currency = Currency.USD, FlightStatus = FlightStatus.Scheduled,
                    AircraftId = AircraftIds.A6EDB, AirlineId = AirlineIds.TurkishAirlines, ScheduleId = ScheduleIds.TK11_IST_DXB, CreatedBy = "system"
                });
            }

            // IST → JFK
            for (int i = 1; i <= 14; i++)
            {
                var dep = today.AddDays(i).AddHours(23).AddMinutes(30);
                flights.Add(new Flight
                {
                    Number = $"TK{(3).ToString().PadLeft(4, '0')}", DepartureDateTime = dep, ArrivalDateTime = dep.AddHours(10).AddMinutes(30),
                    Duration = TimeSpan.FromHours(10.5), BaseEconomyPrice = 650, BasePremiumEconomyPrice = 1100, BaseBusinessPrice = 2800, BaseFirstClassPrice = 5500,
                    Currency = Currency.USD, FlightStatus = FlightStatus.Scheduled,
                    AircraftId = AircraftIds.TCLNA, AirlineId = AirlineIds.TurkishAirlines, ScheduleId = ScheduleIds.TK3_IST_JFK, CreatedBy = "system"
                });
            }

            // IST → ADB (domestic)
            for (int i = 1; i <= 14; i++)
            {
                var dep = today.AddDays(i).AddHours(7);
                flights.Add(new Flight
                {
                    Number = $"TK{(1793).ToString().PadLeft(4, '0')}", DepartureDateTime = dep, ArrivalDateTime = dep.AddHours(1).AddMinutes(20),
                    Duration = TimeSpan.FromMinutes(80), BaseEconomyPrice = 120, BasePremiumEconomyPrice = 200, BaseBusinessPrice = 450, BaseFirstClassPrice = 450,
                    Currency = Currency.TRY, FlightStatus = FlightStatus.Scheduled,
                    AircraftId = AircraftIds.TCKUE, AirlineId = AirlineIds.TurkishAirlines, ScheduleId = ScheduleIds.TK1793_IST_ADB, CreatedBy = "system"
                });
            }

            // IST → AYT (domestic)
            for (int i = 1; i <= 14; i++)
            {
                var dep = today.AddDays(i).AddHours(9);
                flights.Add(new Flight
                {
                    Number = $"PC{(2120).ToString().PadLeft(4, '0')}", DepartureDateTime = dep, ArrivalDateTime = dep.AddHours(1).AddMinutes(30),
                    Duration = TimeSpan.FromMinutes(90), BaseEconomyPrice = 99, BasePremiumEconomyPrice = 160, BaseBusinessPrice = 380, BaseFirstClassPrice = 380,
                    Currency = Currency.TRY, FlightStatus = FlightStatus.Scheduled,
                    AircraftId = AircraftIds.YBBSA, AirlineId = AirlineIds.Pegasus, ScheduleId = ScheduleIds.TK2120_IST_AYT, CreatedBy = "system"
                });
            }

            db.Flights.AddRange(flights);
            await db.SaveChangesAsync();
        }

        // ID constants
        private static class ManufacturerIds
        {
            public static readonly Guid Boeing = new("10000000-0000-0000-0000-000000000001");
            public static readonly Guid Airbus = new("10000000-0000-0000-0000-000000000002");
            public static readonly Guid Bombardier = new("10000000-0000-0000-0000-000000000003");
        }

        private static class ModelIds
        {
            public static readonly Guid B737800 = new("20000000-0000-0000-0000-000000000001");
            public static readonly Guid B777300ER = new("20000000-0000-0000-0000-000000000002");
            public static readonly Guid A320Neo = new("20000000-0000-0000-0000-000000000003");
            public static readonly Guid A350900 = new("20000000-0000-0000-0000-000000000004");
        }

        private static class AirlineIds
        {
            public static readonly Guid TurkishAirlines = new("30000000-0000-0000-0000-000000000001");
            public static readonly Guid Emirates = new("30000000-0000-0000-0000-000000000002");
            public static readonly Guid Pegasus = new("30000000-0000-0000-0000-000000000003");
        }

        private static class AirportIds
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
        }

        private static class RouteIds
        {
            public static readonly Guid IST_LHR = new("50000000-0000-0000-0000-000000000001");
            public static readonly Guid IST_DXB = new("50000000-0000-0000-0000-000000000002");
            public static readonly Guid IST_JFK = new("50000000-0000-0000-0000-000000000003");
            public static readonly Guid IST_FRA = new("50000000-0000-0000-0000-000000000004");
            public static readonly Guid IST_CDG = new("50000000-0000-0000-0000-000000000005");
            public static readonly Guid IST_ESB = new("50000000-0000-0000-0000-000000000006");
            public static readonly Guid IST_ADB = new("50000000-0000-0000-0000-000000000007");
            public static readonly Guid IST_AYT = new("50000000-0000-0000-0000-000000000008");
            public static readonly Guid LHR_IST = new("50000000-0000-0000-0000-000000000009");
            public static readonly Guid DXB_IST = new("50000000-0000-0000-0000-000000000010");
        }

        private static class ScheduleIds
        {
            public static readonly Guid TK1_IST_LHR = new("60000000-0000-0000-0000-000000000001");
            public static readonly Guid TK11_IST_DXB = new("60000000-0000-0000-0000-000000000002");
            public static readonly Guid TK3_IST_JFK = new("60000000-0000-0000-0000-000000000003");
            public static readonly Guid TK1793_IST_ADB = new("60000000-0000-0000-0000-000000000004");
            public static readonly Guid TK2120_IST_AYT = new("60000000-0000-0000-0000-000000000005");
        }

        private static class AircraftIds
        {
            public static readonly Guid TCLNA = new("70000000-0000-0000-0000-000000000001");
            public static readonly Guid TCKUE = new("70000000-0000-0000-0000-000000000002");
            public static readonly Guid A6EDB = new("70000000-0000-0000-0000-000000000003");
            public static readonly Guid YBBSA = new("70000000-0000-0000-0000-000000000004");
        }
    }
}

