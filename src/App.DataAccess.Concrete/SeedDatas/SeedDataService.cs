namespace App.DataAccess.Concrete.SeedDatas
{
    public static class SeedDataService
    {
        private const string AdminEmail    = "admin@flightreservation.com";
        private const string AdminUsername = "admin";
        private const string AdminPassword = "Admin2026+-!?";

        private static readonly (string username, string email, string name, string surname, string phone, DateTime birth, string nationality, NotificationChannel notif)[] UserData =
        {
            ("user01","user01@skyway.com","Ahmet",   "Yilmaz", "+905001000001", new DateTime(1990,3,15,0,0,0,DateTimeKind.Utc), "TR", NotificationChannel.Email),
            ("user02","user02@skyway.com","Mehmet",  "Kaya",   "+905001000002", new DateTime(1988,7,22,0,0,0,DateTimeKind.Utc), "TR", NotificationChannel.Sms),
            ("user03","user03@skyway.com","Ayse",    "Demir",  "+905001000003", new DateTime(1995,1,10,0,0,0,DateTimeKind.Utc), "TR", NotificationChannel.Email),
            ("user04","user04@skyway.com","Fatma",   "Celik",  "+905001000004", new DateTime(1992,11,5,0,0,0,DateTimeKind.Utc), "TR", NotificationChannel.Email),
            ("user05","user05@skyway.com","Ali",     "Sahin",  "+905001000005", new DateTime(1985,6,18,0,0,0,DateTimeKind.Utc), "TR", NotificationChannel.Sms),
            ("user06","user06@skyway.com","Zeynep",  "Arslan", "+905001000006", new DateTime(1998,2,28,0,0,0,DateTimeKind.Utc), "TR", NotificationChannel.Email),
            ("user07","user07@skyway.com","Mustafa", "Dogan",  "+905001000007", new DateTime(1993,9,12,0,0,0,DateTimeKind.Utc), "TR", NotificationChannel.Email),
            ("user08","user08@skyway.com","Hatice",  "Kilic",  "+905001000008", new DateTime(1987,4,3,0,0,0,DateTimeKind.Utc),  "TR", NotificationChannel.Sms),
            ("user09","user09@skyway.com","Ibrahim", "Aydin",  "+905001000009", new DateTime(1991,12,20,0,0,0,DateTimeKind.Utc),"TR", NotificationChannel.Email),
            ("user10","user10@skyway.com","Elif",    "Ozturk", "+905001000010", new DateTime(1996,8,7,0,0,0,DateTimeKind.Utc),  "TR", NotificationChannel.Email),
            ("user11","user11@skyway.com","Hasan",   "Yildiz", "+905001000011", new DateTime(1984,5,25,0,0,0,DateTimeKind.Utc), "TR", NotificationChannel.Sms),
            ("user12","user12@skyway.com","Merve",   "Gunes",  "+905001000012", new DateTime(1999,10,14,0,0,0,DateTimeKind.Utc),"TR", NotificationChannel.Email),
            ("user13","user13@skyway.com","Huseyin", "Polat",  "+905001000013", new DateTime(1986,3,8,0,0,0,DateTimeKind.Utc),  "TR", NotificationChannel.Email),
            ("user14","user14@skyway.com","Selin",   "Cetin",  "+905001000014", new DateTime(1994,7,31,0,0,0,DateTimeKind.Utc), "TR", NotificationChannel.Sms),
            ("user15","user15@skyway.com","Ozgur",   "Bozkurt","+905001000015", new DateTime(1989,1,17,0,0,0,DateTimeKind.Utc), "TR", NotificationChannel.Email),
            ("user16","user16@skyway.com","Burcu",   "Akin",   "+905001000016", new DateTime(1997,6,4,0,0,0,DateTimeKind.Utc),  "TR", NotificationChannel.Email),
            ("user17","user17@skyway.com","Emre",    "Simsek", "+905001000017", new DateTime(1983,11,29,0,0,0,DateTimeKind.Utc),"TR", NotificationChannel.Sms),
            ("user18","user18@skyway.com","Cansu",   "Bulut",  "+905001000018", new DateTime(2000,4,19,0,0,0,DateTimeKind.Utc), "TR", NotificationChannel.Email),
            ("user19","user19@skyway.com","Oguz",    "Erdogan","+905001000019", new DateTime(1982,9,6,0,0,0,DateTimeKind.Utc),  "TR", NotificationChannel.Email),
            ("user20","user20@skyway.com","Pinar",   "Kaplan", "+905001000020", new DateTime(1995,12,11,0,0,0,DateTimeKind.Utc),"TR", NotificationChannel.Sms),
        };

        public static async Task SeedAsync(
            FlightReservationDbContext db,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            await SeedRolesAsync(roleManager);
            await SeedAdminAsync(db, userManager);
            await SeedAppUsersAsync(db, userManager);
            await SeedManufacturersAsync(db);
            await SeedModelsAsync(db);
            await SeedAirlinesAsync(db);
            await SeedAirportsAsync(db);
            await SeedRoutesAsync(db);
            await SeedSchedulesAsync(db);
            await SeedAircraftsAsync(db);
            await SeedSeatsAsync(db);
            await SeedFlightsAsync(db);
            await SeedBookingsAsync(db);
        }

        // ── Roles ─────────────────────────────────────────────────────────────────

        private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            foreach (var role in new[] { "Admin", "AppUser" })
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
        }

        // ── Admin ─────────────────────────────────────────────────────────────────

        private static async Task SeedAdminAsync(FlightReservationDbContext db, UserManager<IdentityUser> userManager)
        {
            if (await userManager.FindByEmailAsync(AdminEmail) != null) return;
            var identity = new IdentityUser
            {
                Email = AdminEmail, UserName = AdminUsername,
                NormalizedEmail = AdminEmail.ToUpperInvariant(),
                NormalizedUserName = AdminUsername.ToUpperInvariant(),
                EmailConfirmed = true
            };
            await userManager.CreateAsync(identity, AdminPassword);
            await userManager.AddToRoleAsync(identity, "Admin");
            if (!await db.Admins.AnyAsync(a => a.Email == AdminEmail))
            {
                db.Admins.Add(new Admin { Name = "Super", Surname = "Admin", Email = AdminEmail, IdentityId = identity.Id, IsSuperAdmin = true, CreatedBy = "system" });
                await db.SaveChangesAsync();
            }
        }

        // ── 20 App Users ──────────────────────────────────────────────────────────

        private static async Task SeedAppUsersAsync(FlightReservationDbContext db, UserManager<IdentityUser> userManager)
        {
            if (await db.AppUsers.CountAsync() >= 20) return;

            for (int i = 0; i < UserData.Length; i++)
            {
                var (username, email, name, surname, phone, birth, nationality, notif) = UserData[i];
                if (await userManager.FindByEmailAsync(email) != null) continue;

                var identity = new IdentityUser
                {
                    Email = email, UserName = username,
                    NormalizedEmail = email.ToUpperInvariant(),
                    NormalizedUserName = username.ToUpperInvariant(),
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(identity, "User2026+-!?");
                if (!result.Succeeded) continue;
                await userManager.AddToRoleAsync(identity, "AppUser");

                if (!await db.AppUsers.AnyAsync(u => u.Email == email))
                {
                    db.AppUsers.Add(new AppUser
                    {
                        Id = UserIds.Ids[i],
                        Name = name, Surname = surname, Email = email,
                        PhoneNumber = phone, BirthDate = birth,
                        UserStatus = UserStatus.Active,
                        PreferredNotificationChannel = notif,
                        Nationality = nationality,
                        IdentityId = identity.Id,
                        CreatedBy = "system"
                    });
                }
            }
            await db.SaveChangesAsync();
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
                new Model { Id = ModelIds.B737800,   ManufacturerId = MfgIds.Boeing, Name = "737-800",   BodyType = BodyType.NarrowBody, MaxPassengerCapacity = 189, EconomySeats = 162, PremiumEconomySeats = 0,  BusinessSeats = 12,  FirstClassSeats = 0, MaxRangeKm = 5765,  CreatedBy = "system" },
                new Model { Id = ModelIds.B777300ER, ManufacturerId = MfgIds.Boeing, Name = "777-300ER", BodyType = BodyType.WideBody,   MaxPassengerCapacity = 396, EconomySeats = 304, PremiumEconomySeats = 40, BusinessSeats = 42,  FirstClassSeats = 8, MaxRangeKm = 13650, CreatedBy = "system" },
                new Model { Id = ModelIds.B7879,     ManufacturerId = MfgIds.Boeing, Name = "787-9",     BodyType = BodyType.WideBody,   MaxPassengerCapacity = 296, EconomySeats = 222, PremiumEconomySeats = 28, BusinessSeats = 42,  FirstClassSeats = 0, MaxRangeKm = 14140, CreatedBy = "system" },
                new Model { Id = ModelIds.A320Neo,   ManufacturerId = MfgIds.Airbus, Name = "A320neo",   BodyType = BodyType.NarrowBody, MaxPassengerCapacity = 194, EconomySeats = 150, PremiumEconomySeats = 0,  BusinessSeats = 12,  FirstClassSeats = 0, MaxRangeKm = 6300,  CreatedBy = "system" },
                new Model { Id = ModelIds.A350900,   ManufacturerId = MfgIds.Airbus, Name = "A350-900",  BodyType = BodyType.WideBody,   MaxPassengerCapacity = 369, EconomySeats = 253, PremiumEconomySeats = 48, BusinessSeats = 40,  FirstClassSeats = 8, MaxRangeKm = 15000, CreatedBy = "system" },
                new Model { Id = ModelIds.A380800,   ManufacturerId = MfgIds.Airbus, Name = "A380-800",  BodyType = BodyType.WideBody,   MaxPassengerCapacity = 555, EconomySeats = 399, PremiumEconomySeats = 76, BusinessSeats = 76,  FirstClassSeats = 14,MaxRangeKm = 15200, CreatedBy = "system" }
            );
            await db.SaveChangesAsync();
        }

        // ── Airlines ──────────────────────────────────────────────────────────────

        private static async Task SeedAirlinesAsync(FlightReservationDbContext db)
        {
            if (await db.Airlines.AnyAsync()) return;
            db.Airlines.AddRange(
                new Airline { Id = AirlineIds.THY, Name = "Turkish Airlines", IataCode = "TK", IcaoCode = "THY", Country = "Turkey",               Website = "https://www.turkishairlines.com", CreatedBy = "system" },
                new Airline { Id = AirlineIds.EK,  Name = "Emirates",         IataCode = "EK", IcaoCode = "UAE", Country = "United Arab Emirates",   Website = "https://www.emirates.com",        CreatedBy = "system" },
                new Airline { Id = AirlineIds.PC,  Name = "Pegasus Airlines", IataCode = "PC", IcaoCode = "PGT", Country = "Turkey",               Website = "https://www.flypgs.com",          CreatedBy = "system" },
                new Airline { Id = AirlineIds.LH,  Name = "Lufthansa",        IataCode = "LH", IcaoCode = "DLH", Country = "Germany",              Website = "https://www.lufthansa.com",       CreatedBy = "system" },
                new Airline { Id = AirlineIds.BA,  Name = "British Airways",  IataCode = "BA", IcaoCode = "BAW", Country = "United Kingdom",        Website = "https://www.britishairways.com",  CreatedBy = "system" },
                new Airline { Id = AirlineIds.AF,  Name = "Air France",       IataCode = "AF", IcaoCode = "AFR", Country = "France",               Website = "https://www.airfrance.com",       CreatedBy = "system" }
            );
            await db.SaveChangesAsync();
        }

        // ── Airports ──────────────────────────────────────────────────────────────

        private static async Task SeedAirportsAsync(FlightReservationDbContext db)
        {
            if (await db.Airports.AnyAsync()) return;
            db.Airports.AddRange(
                // ── Turkey ───────────────────────────────────────────────────────────
                new Airport { Id = AptIds.IST, IataCode = "IST", IcaoCode = "LTFM", Name = "Istanbul Airport",              City = "Istanbul",          Country = "Turkey", TimeZone = "Europe/Istanbul", CreatedBy = "system" },
                new Airport { Id = AptIds.SAW, IataCode = "SAW", IcaoCode = "LTFJ", Name = "Sabiha Gokcen Airport",         City = "Istanbul",          Country = "Turkey", TimeZone = "Europe/Istanbul", CreatedBy = "system" },
                new Airport { Id = AptIds.ESB, IataCode = "ESB", IcaoCode = "LTAC", Name = "Esenboga Airport",              City = "Ankara",            Country = "Turkey", TimeZone = "Europe/Istanbul", CreatedBy = "system" },
                new Airport { Id = AptIds.ADB, IataCode = "ADB", IcaoCode = "LTBJ", Name = "Adnan Menderes Airport",        City = "Izmir",             Country = "Turkey", TimeZone = "Europe/Istanbul", CreatedBy = "system" },
                new Airport { Id = AptIds.AYT, IataCode = "AYT", IcaoCode = "LTAI", Name = "Antalya Airport",               City = "Antalya",           Country = "Turkey", TimeZone = "Europe/Istanbul", CreatedBy = "system" },
                new Airport { Id = AptIds.BJV, IataCode = "BJV", IcaoCode = "LTFE", Name = "Milas-Bodrum Airport",          City = "Bodrum",            Country = "Turkey", TimeZone = "Europe/Istanbul", CreatedBy = "system" },
                new Airport { Id = AptIds.DLM, IataCode = "DLM", IcaoCode = "LTBS", Name = "Dalaman Airport",               City = "Dalaman",           Country = "Turkey", TimeZone = "Europe/Istanbul", CreatedBy = "system" },
                new Airport { Id = AptIds.GZT, IataCode = "GZT", IcaoCode = "LTAJ", Name = "Gaziantep Airport",             City = "Gaziantep",         Country = "Turkey", TimeZone = "Europe/Istanbul", CreatedBy = "system" },
                new Airport { Id = AptIds.TZX, IataCode = "TZX", IcaoCode = "LTCG", Name = "Trabzon Airport",               City = "Trabzon",           Country = "Turkey", TimeZone = "Europe/Istanbul", CreatedBy = "system" },
                new Airport { Id = AptIds.SZF, IataCode = "SZF", IcaoCode = "LTFH", Name = "Samsun-Carsamba Airport",       City = "Samsun",            Country = "Turkey", TimeZone = "Europe/Istanbul", CreatedBy = "system" },
                new Airport { Id = AptIds.ADA, IataCode = "ADA", IcaoCode = "LTAF", Name = "Adana Sakirpasa Airport",       City = "Adana",             Country = "Turkey", TimeZone = "Europe/Istanbul", CreatedBy = "system" },
                new Airport { Id = AptIds.ADF, IataCode = "ADF", IcaoCode = "LTCP", Name = "Adiyaman Airport",              City = "Adiyaman",          Country = "Turkey", TimeZone = "Europe/Istanbul", CreatedBy = "system" },
                new Airport { Id = AptIds.AJI, IataCode = "AJI", IcaoCode = "LTCO", Name = "Agri Airport",                  City = "Agri",              Country = "Turkey", TimeZone = "Europe/Istanbul", CreatedBy = "system" },
                new Airport { Id = AptIds.GZP, IataCode = "GZP", IcaoCode = "LTGP", Name = "Alanya-Gazipasa Airport",       City = "Alanya",            Country = "Turkey", TimeZone = "Europe/Istanbul", CreatedBy = "system" },
                new Airport { Id = AptIds.MZH, IataCode = "MZH", IcaoCode = "LTAS", Name = "Amasya Merzifon Airport",       City = "Amasya",            Country = "Turkey", TimeZone = "Europe/Istanbul", CreatedBy = "system" },
                new Airport { Id = AptIds.EDO, IataCode = "EDO", IcaoCode = "LTFD", Name = "Balikesir Koca Seyit Airport",  City = "Balikesir",         Country = "Turkey", TimeZone = "Europe/Istanbul", CreatedBy = "system" },
                new Airport { Id = AptIds.BAL, IataCode = "BAL", IcaoCode = "LTBL", Name = "Batman Airport",                City = "Batman",            Country = "Turkey", TimeZone = "Europe/Istanbul", CreatedBy = "system" },
                new Airport { Id = AptIds.DNZ, IataCode = "DNZ", IcaoCode = "LTAY", Name = "Denizli Cardak Airport",        City = "Denizli",           Country = "Turkey", TimeZone = "Europe/Istanbul", CreatedBy = "system" },
                new Airport { Id = AptIds.DIY, IataCode = "DIY", IcaoCode = "LTCC", Name = "Diyarbakir Airport",            City = "Diyarbakir",        Country = "Turkey", TimeZone = "Europe/Istanbul", CreatedBy = "system" },
                new Airport { Id = AptIds.EZS, IataCode = "EZS", IcaoCode = "LTCA", Name = "Elazig Airport",                City = "Elazig",            Country = "Turkey", TimeZone = "Europe/Istanbul", CreatedBy = "system" },
                new Airport { Id = AptIds.ERC, IataCode = "ERC", IcaoCode = "LTCD", Name = "Erzincan Airport",              City = "Erzincan",          Country = "Turkey", TimeZone = "Europe/Istanbul", CreatedBy = "system" },
                new Airport { Id = AptIds.ERZ, IataCode = "ERZ", IcaoCode = "LTCE", Name = "Erzurum Airport",               City = "Erzurum",           Country = "Turkey", TimeZone = "Europe/Istanbul", CreatedBy = "system" },
                new Airport { Id = AptIds.HTY, IataCode = "HTY", IcaoCode = "LTDA", Name = "Hatay Airport",                 City = "Hatay",             Country = "Turkey", TimeZone = "Europe/Istanbul", CreatedBy = "system" },
                new Airport { Id = AptIds.IGD, IataCode = "IGD", IcaoCode = "LTCT", Name = "Igdir Airport",                 City = "Igdir",             Country = "Turkey", TimeZone = "Europe/Istanbul", CreatedBy = "system" },
                new Airport { Id = AptIds.KCM, IataCode = "KCM", IcaoCode = "LTAG", Name = "Kahramanmaras Airport",         City = "Kahramanmaras",     Country = "Turkey", TimeZone = "Europe/Istanbul", CreatedBy = "system" },
                new Airport { Id = AptIds.KSY, IataCode = "KSY", IcaoCode = "LTCF", Name = "Kars Harakani Airport",         City = "Kars",              Country = "Turkey", TimeZone = "Europe/Istanbul", CreatedBy = "system" },
                new Airport { Id = AptIds.KFS, IataCode = "KFS", IcaoCode = "LTAK", Name = "Kastamonu Airport",             City = "Kastamonu",         Country = "Turkey", TimeZone = "Europe/Istanbul", CreatedBy = "system" },
                new Airport { Id = AptIds.ASR, IataCode = "ASR", IcaoCode = "LTAU", Name = "Kayseri Erkilet Airport",       City = "Kayseri",           Country = "Turkey", TimeZone = "Europe/Istanbul", CreatedBy = "system" },
                new Airport { Id = AptIds.KYA, IataCode = "KYA", IcaoCode = "LTAN", Name = "Konya Airport",                 City = "Konya",             Country = "Turkey", TimeZone = "Europe/Istanbul", CreatedBy = "system" },
                new Airport { Id = AptIds.MLX, IataCode = "MLX", IcaoCode = "LTAT", Name = "Malatya Battalgazi Airport",    City = "Malatya",           Country = "Turkey", TimeZone = "Europe/Istanbul", CreatedBy = "system" },
                new Airport { Id = AptIds.VAN, IataCode = "VAN", IcaoCode = "LTCI", Name = "Van Ferit Melen Airport",       City = "Van",               Country = "Turkey", TimeZone = "Europe/Istanbul", CreatedBy = "system" },
                new Airport { Id = AptIds.NVS, IataCode = "NAV", IcaoCode = "LTAZ", Name = "Nevsehir Kapadokya Airport",    City = "Nevsehir",          Country = "Turkey", TimeZone = "Europe/Istanbul", CreatedBy = "system" },
                new Airport { Id = AptIds.SIC, IataCode = "SIC", IcaoCode = "LTCL", Name = "Siirt Airport",                 City = "Siirt",             Country = "Turkey", TimeZone = "Europe/Istanbul", CreatedBy = "system" },
                new Airport { Id = AptIds.VAS, IataCode = "VAS", IcaoCode = "LTAR", Name = "Sivas Nuri Demirag Airport",    City = "Sivas",             Country = "Turkey", TimeZone = "Europe/Istanbul", CreatedBy = "system" },
                new Airport { Id = AptIds.GNY, IataCode = "GNY", IcaoCode = "LTCS", Name = "Sanliurfa GAP Airport",         City = "Sanliurfa",         Country = "Turkey", TimeZone = "Europe/Istanbul", CreatedBy = "system" },
                new Airport { Id = AptIds.MUS, IataCode = "MSR", IcaoCode = "LTCK", Name = "Mus Airport",                   City = "Mus",               Country = "Turkey", TimeZone = "Europe/Istanbul", CreatedBy = "system" },
                new Airport { Id = AptIds.ECN, IataCode = "ECN", IcaoCode = "LCEN", Name = "Ercan Airport",                 City = "Lefkosa",           Country = "Northern Cyprus", TimeZone = "Asia/Nicosia", CreatedBy = "system" },
                // ── United Kingdom ───────────────────────────────────────────────────
                new Airport { Id = AptIds.LHR, IataCode = "LHR", IcaoCode = "EGLL", Name = "Heathrow Airport",              City = "London",            Country = "United Kingdom", TimeZone = "Europe/London", CreatedBy = "system" },
                new Airport { Id = AptIds.LGW, IataCode = "LGW", IcaoCode = "EGKK", Name = "Gatwick Airport",               City = "London",            Country = "United Kingdom", TimeZone = "Europe/London", CreatedBy = "system" },
                new Airport { Id = AptIds.MAN, IataCode = "MAN", IcaoCode = "EGCC", Name = "Manchester Airport",            City = "Manchester",        Country = "United Kingdom", TimeZone = "Europe/London", CreatedBy = "system" },
                new Airport { Id = AptIds.BHX, IataCode = "BHX", IcaoCode = "EGBB", Name = "Birmingham Airport",            City = "Birmingham",        Country = "United Kingdom", TimeZone = "Europe/London", CreatedBy = "system" },
                new Airport { Id = AptIds.BRS, IataCode = "BRS", IcaoCode = "EGGD", Name = "Bristol Airport",               City = "Bristol",           Country = "United Kingdom", TimeZone = "Europe/London", CreatedBy = "system" },
                new Airport { Id = AptIds.EDI, IataCode = "EDI", IcaoCode = "EGPH", Name = "Edinburgh Airport",             City = "Edinburgh",         Country = "United Kingdom", TimeZone = "Europe/London", CreatedBy = "system" },
                new Airport { Id = AptIds.STN, IataCode = "STN", IcaoCode = "EGSS", Name = "London Stansted Airport",       City = "London",            Country = "United Kingdom", TimeZone = "Europe/London", CreatedBy = "system" },
                // ── United Arab Emirates ─────────────────────────────────────────────
                new Airport { Id = AptIds.DXB, IataCode = "DXB", IcaoCode = "OMDB", Name = "Dubai International Airport",   City = "Dubai",             Country = "United Arab Emirates", TimeZone = "Asia/Dubai", CreatedBy = "system" },
                new Airport { Id = AptIds.AUH, IataCode = "AUH", IcaoCode = "OMAA", Name = "Abu Dhabi International Airport",City = "Abu Dhabi",        Country = "United Arab Emirates", TimeZone = "Asia/Dubai", CreatedBy = "system" },
                // ── United States ────────────────────────────────────────────────────
                new Airport { Id = AptIds.JFK, IataCode = "JFK", IcaoCode = "KJFK", Name = "John F. Kennedy Airport",       City = "New York",          Country = "United States", TimeZone = "America/New_York",    CreatedBy = "system" },
                new Airport { Id = AptIds.LAX, IataCode = "LAX", IcaoCode = "KLAX", Name = "Los Angeles Airport",           City = "Los Angeles",       Country = "United States", TimeZone = "America/Los_Angeles", CreatedBy = "system" },
                new Airport { Id = AptIds.ORD, IataCode = "ORD", IcaoCode = "KORD", Name = "O'Hare International Airport",  City = "Chicago",           Country = "United States", TimeZone = "America/Chicago",     CreatedBy = "system" },
                new Airport { Id = AptIds.MIA, IataCode = "MIA", IcaoCode = "KMIA", Name = "Miami International Airport",   City = "Miami",             Country = "United States", TimeZone = "America/New_York",    CreatedBy = "system" },
                new Airport { Id = AptIds.BOS, IataCode = "BOS", IcaoCode = "KBOS", Name = "Boston Logan Airport",          City = "Boston",            Country = "United States", TimeZone = "America/New_York",    CreatedBy = "system" },
                // ── Germany ──────────────────────────────────────────────────────────
                new Airport { Id = AptIds.FRA, IataCode = "FRA", IcaoCode = "EDDF", Name = "Frankfurt Airport",             City = "Frankfurt",         Country = "Germany", TimeZone = "Europe/Berlin", CreatedBy = "system" },
                new Airport { Id = AptIds.MUC, IataCode = "MUC", IcaoCode = "EDDM", Name = "Munich Airport",                City = "Munich",            Country = "Germany", TimeZone = "Europe/Berlin", CreatedBy = "system" },
                new Airport { Id = AptIds.BER, IataCode = "BER", IcaoCode = "EDDB", Name = "Berlin Brandenburg Airport",    City = "Berlin",            Country = "Germany", TimeZone = "Europe/Berlin", CreatedBy = "system" },
                new Airport { Id = AptIds.HAM, IataCode = "HAM", IcaoCode = "EDDH", Name = "Hamburg Airport",               City = "Hamburg",           Country = "Germany", TimeZone = "Europe/Berlin", CreatedBy = "system" },
                new Airport { Id = AptIds.DUS, IataCode = "DUS", IcaoCode = "EDDL", Name = "Dusseldorf Airport",            City = "Dusseldorf",        Country = "Germany", TimeZone = "Europe/Berlin", CreatedBy = "system" },
                new Airport { Id = AptIds.CGN, IataCode = "CGN", IcaoCode = "EDDK", Name = "Cologne Bonn Airport",          City = "Cologne",           Country = "Germany", TimeZone = "Europe/Berlin", CreatedBy = "system" },
                new Airport { Id = AptIds.HAJ, IataCode = "HAJ", IcaoCode = "EDDV", Name = "Hannover Airport",              City = "Hannover",          Country = "Germany", TimeZone = "Europe/Berlin", CreatedBy = "system" },
                new Airport { Id = AptIds.STR, IataCode = "STR", IcaoCode = "EDDS", Name = "Stuttgart Airport",             City = "Stuttgart",         Country = "Germany", TimeZone = "Europe/Berlin", CreatedBy = "system" },
                new Airport { Id = AptIds.LEJ, IataCode = "LEJ", IcaoCode = "EDDP", Name = "Leipzig Halle Airport",         City = "Leipzig",           Country = "Germany", TimeZone = "Europe/Berlin", CreatedBy = "system" },
                new Airport { Id = AptIds.DRS, IataCode = "DRS", IcaoCode = "EDDC", Name = "Dresden Airport",               City = "Dresden",           Country = "Germany", TimeZone = "Europe/Berlin", CreatedBy = "system" },
                new Airport { Id = AptIds.DTM, IataCode = "DTM", IcaoCode = "EDLW", Name = "Dortmund Airport",              City = "Dortmund",          Country = "Germany", TimeZone = "Europe/Berlin", CreatedBy = "system" },
                new Airport { Id = AptIds.BRE, IataCode = "BRE", IcaoCode = "EDDW", Name = "Bremen Airport",                City = "Bremen",            Country = "Germany", TimeZone = "Europe/Berlin", CreatedBy = "system" },
                new Airport { Id = AptIds.ERF, IataCode = "ERF", IcaoCode = "EDDE", Name = "Erfurt-Weimar Airport",         City = "Erfurt",            Country = "Germany", TimeZone = "Europe/Berlin", CreatedBy = "system" },
                // ── France ───────────────────────────────────────────────────────────
                new Airport { Id = AptIds.CDG, IataCode = "CDG", IcaoCode = "LFPG", Name = "Charles de Gaulle Airport",     City = "Paris",             Country = "France", TimeZone = "Europe/Paris", CreatedBy = "system" },
                new Airport { Id = AptIds.NCE, IataCode = "NCE", IcaoCode = "LFMN", Name = "Nice Cote d'Azur Airport",      City = "Nice",              Country = "France", TimeZone = "Europe/Paris", CreatedBy = "system" },
                new Airport { Id = AptIds.LYS, IataCode = "LYS", IcaoCode = "LFLL", Name = "Lyon Saint-Exupery Airport",   City = "Lyon",              Country = "France", TimeZone = "Europe/Paris", CreatedBy = "system" },
                new Airport { Id = AptIds.MRS, IataCode = "MRS", IcaoCode = "LFML", Name = "Marseille Provence Airport",    City = "Marseille",         Country = "France", TimeZone = "Europe/Paris", CreatedBy = "system" },
                // ── Netherlands ──────────────────────────────────────────────────────
                new Airport { Id = AptIds.AMS, IataCode = "AMS", IcaoCode = "EHAM", Name = "Amsterdam Schiphol Airport",    City = "Amsterdam",         Country = "Netherlands", TimeZone = "Europe/Amsterdam", CreatedBy = "system" },
                new Airport { Id = AptIds.EIN, IataCode = "EIN", IcaoCode = "EHEH", Name = "Eindhoven Airport",             City = "Eindhoven",         Country = "Netherlands", TimeZone = "Europe/Amsterdam", CreatedBy = "system" },
                // ── Spain ────────────────────────────────────────────────────────────
                new Airport { Id = AptIds.MAD, IataCode = "MAD", IcaoCode = "LEMD", Name = "Adolfo Suarez Madrid Airport",  City = "Madrid",            Country = "Spain", TimeZone = "Europe/Madrid", CreatedBy = "system" },
                new Airport { Id = AptIds.BCN, IataCode = "BCN", IcaoCode = "LEBL", Name = "Barcelona El Prat Airport",     City = "Barcelona",         Country = "Spain", TimeZone = "Europe/Madrid", CreatedBy = "system" },
                new Airport { Id = AptIds.ALC, IataCode = "ALC", IcaoCode = "LEAL", Name = "Alicante Elche Airport",        City = "Alicante",          Country = "Spain", TimeZone = "Europe/Madrid", CreatedBy = "system" },
                new Airport { Id = AptIds.OVD, IataCode = "OVD", IcaoCode = "LEAS", Name = "Asturias Airport",              City = "Asturias",          Country = "Spain", TimeZone = "Europe/Madrid", CreatedBy = "system" },
                new Airport { Id = AptIds.BIO, IataCode = "BIO", IcaoCode = "LEBB", Name = "Bilbao Airport",                City = "Bilbao",            Country = "Spain", TimeZone = "Europe/Madrid", CreatedBy = "system" },
                new Airport { Id = AptIds.FUE, IataCode = "FUE", IcaoCode = "GCFV", Name = "Fuerteventura Airport",         City = "Fuerteventura",     Country = "Spain", TimeZone = "Atlantic/Canary", CreatedBy = "system" },
                new Airport { Id = AptIds.LPA, IataCode = "LPA", IcaoCode = "GCLP", Name = "Gran Canaria Airport",          City = "Gran Canaria",      Country = "Spain", TimeZone = "Atlantic/Canary", CreatedBy = "system" },
                new Airport { Id = AptIds.IBZ, IataCode = "IBZ", IcaoCode = "LEIB", Name = "Ibiza Airport",                 City = "Ibiza",             Country = "Spain", TimeZone = "Europe/Madrid", CreatedBy = "system" },
                new Airport { Id = AptIds.ACE, IataCode = "ACE", IcaoCode = "GCRR", Name = "Lanzarote Airport",             City = "Lanzarote",         Country = "Spain", TimeZone = "Atlantic/Canary", CreatedBy = "system" },
                new Airport { Id = AptIds.LCG, IataCode = "LCG", IcaoCode = "LECO", Name = "A Coruna Airport",              City = "A Coruna",          Country = "Spain", TimeZone = "Europe/Madrid", CreatedBy = "system" },
                new Airport { Id = AptIds.PMI, IataCode = "PMI", IcaoCode = "LEPA", Name = "Palma de Mallorca Airport",     City = "Palma",             Country = "Spain", TimeZone = "Europe/Madrid", CreatedBy = "system" },
                new Airport { Id = AptIds.AGP, IataCode = "AGP", IcaoCode = "LEMG", Name = "Malaga Airport",                City = "Malaga",            Country = "Spain", TimeZone = "Europe/Madrid", CreatedBy = "system" },
                // ── Italy ────────────────────────────────────────────────────────────
                new Airport { Id = AptIds.FCO, IataCode = "FCO", IcaoCode = "LIRF", Name = "Fiumicino Airport",             City = "Rome",              Country = "Italy", TimeZone = "Europe/Rome", CreatedBy = "system" },
                new Airport { Id = AptIds.MXP, IataCode = "MXP", IcaoCode = "LIMC", Name = "Milan Malpensa Airport",        City = "Milan",             Country = "Italy", TimeZone = "Europe/Rome", CreatedBy = "system" },
                new Airport { Id = AptIds.BLQ, IataCode = "BLQ", IcaoCode = "LIPE", Name = "Bologna Airport",               City = "Bologna",           Country = "Italy", TimeZone = "Europe/Rome", CreatedBy = "system" },
                new Airport { Id = AptIds.VCE, IataCode = "VCE", IcaoCode = "LIPZ", Name = "Venice Marco Polo Airport",     City = "Venice",            Country = "Italy", TimeZone = "Europe/Rome", CreatedBy = "system" },
                new Airport { Id = AptIds.NAP, IataCode = "NAP", IcaoCode = "LIRN", Name = "Naples International Airport",  City = "Naples",            Country = "Italy", TimeZone = "Europe/Rome", CreatedBy = "system" },
                // ── Greece ───────────────────────────────────────────────────────────
                new Airport { Id = AptIds.ATH, IataCode = "ATH", IcaoCode = "LGAV", Name = "Athens Eleftherios Venizelos",  City = "Athens",            Country = "Greece", TimeZone = "Europe/Athens", CreatedBy = "system" },
                new Airport { Id = AptIds.SKG, IataCode = "SKG", IcaoCode = "LGTS", Name = "Thessaloniki Macedonia Airport", City = "Thessaloniki",      Country = "Greece", TimeZone = "Europe/Athens", CreatedBy = "system" },
                // ── Belgium ──────────────────────────────────────────────────────────
                new Airport { Id = AptIds.BRU, IataCode = "BRU", IcaoCode = "EBBR", Name = "Brussels Zaventem Airport",     City = "Brussels",          Country = "Belgium", TimeZone = "Europe/Brussels", CreatedBy = "system" },
                new Airport { Id = AptIds.CRL, IataCode = "CRL", IcaoCode = "EBCI", Name = "Brussels Charleroi Airport",    City = "Brussels",          Country = "Belgium", TimeZone = "Europe/Brussels", CreatedBy = "system" },
                // ── Switzerland ──────────────────────────────────────────────────────
                new Airport { Id = AptIds.ZRH, IataCode = "ZRH", IcaoCode = "LSZH", Name = "Zurich Airport",                City = "Zurich",            Country = "Switzerland", TimeZone = "Europe/Zurich", CreatedBy = "system" },
                new Airport { Id = AptIds.GVA, IataCode = "GVA", IcaoCode = "LSGG", Name = "Geneva Airport",                City = "Geneva",            Country = "Switzerland", TimeZone = "Europe/Zurich", CreatedBy = "system" },
                new Airport { Id = AptIds.BSL, IataCode = "BSL", IcaoCode = "LFSB", Name = "Basel-Mulhouse Airport",        City = "Basel",             Country = "Switzerland", TimeZone = "Europe/Zurich", CreatedBy = "system" },
                // ── Austria ──────────────────────────────────────────────────────────
                new Airport { Id = AptIds.VIE, IataCode = "VIE", IcaoCode = "LOWW", Name = "Vienna International Airport",  City = "Vienna",            Country = "Austria", TimeZone = "Europe/Vienna", CreatedBy = "system" },
                new Airport { Id = AptIds.GRZ, IataCode = "GRZ", IcaoCode = "LOWG", Name = "Graz Airport",                  City = "Graz",              Country = "Austria", TimeZone = "Europe/Vienna", CreatedBy = "system" },
                // ── Denmark ──────────────────────────────────────────────────────────
                new Airport { Id = AptIds.CPH, IataCode = "CPH", IcaoCode = "EKCH", Name = "Copenhagen Airport",            City = "Copenhagen",        Country = "Denmark", TimeZone = "Europe/Copenhagen", CreatedBy = "system" },
                new Airport { Id = AptIds.AAL, IataCode = "AAL", IcaoCode = "EKYT", Name = "Aalborg Airport",               City = "Aalborg",           Country = "Denmark", TimeZone = "Europe/Copenhagen", CreatedBy = "system" },
                new Airport { Id = AptIds.AAR, IataCode = "AAR", IcaoCode = "EKAH", Name = "Aarhus Airport",                City = "Aarhus",            Country = "Denmark", TimeZone = "Europe/Copenhagen", CreatedBy = "system" },
                new Airport { Id = AptIds.BLL, IataCode = "BLL", IcaoCode = "EKBI", Name = "Billund Airport",               City = "Billund",           Country = "Denmark", TimeZone = "Europe/Copenhagen", CreatedBy = "system" },
                // ── Norway ───────────────────────────────────────────────────────────
                new Airport { Id = AptIds.OSL, IataCode = "OSL", IcaoCode = "ENGM", Name = "Oslo Gardermoen Airport",       City = "Oslo",              Country = "Norway", TimeZone = "Europe/Oslo", CreatedBy = "system" },
                // ── Sweden ───────────────────────────────────────────────────────────
                new Airport { Id = AptIds.ARN, IataCode = "ARN", IcaoCode = "ESSA", Name = "Stockholm Arlanda Airport",     City = "Stockholm",         Country = "Sweden", TimeZone = "Europe/Stockholm", CreatedBy = "system" },
                new Airport { Id = AptIds.GOT, IataCode = "GOT", IcaoCode = "ESGG", Name = "Goteborg Landvetter Airport",   City = "Gothenburg",        Country = "Sweden", TimeZone = "Europe/Stockholm", CreatedBy = "system" },
                // ── Finland ──────────────────────────────────────────────────────────
                new Airport { Id = AptIds.HEL, IataCode = "HEL", IcaoCode = "EFHK", Name = "Helsinki Vantaa Airport",      City = "Helsinki",          Country = "Finland", TimeZone = "Europe/Helsinki", CreatedBy = "system" },
                // ── Poland ───────────────────────────────────────────────────────────
                new Airport { Id = AptIds.WAW, IataCode = "WAW", IcaoCode = "EPWA", Name = "Warsaw Chopin Airport",         City = "Warsaw",            Country = "Poland", TimeZone = "Europe/Warsaw", CreatedBy = "system" },
                new Airport { Id = AptIds.KRK, IataCode = "KRK", IcaoCode = "EPKK", Name = "Krakow John Paul II Airport",   City = "Krakow",            Country = "Poland", TimeZone = "Europe/Warsaw", CreatedBy = "system" },
                // ── Hungary ──────────────────────────────────────────────────────────
                new Airport { Id = AptIds.BUD, IataCode = "BUD", IcaoCode = "LHBP", Name = "Budapest Liszt Ferenc Airport", City = "Budapest",          Country = "Hungary", TimeZone = "Europe/Budapest", CreatedBy = "system" },
                // ── Romania ──────────────────────────────────────────────────────────
                new Airport { Id = AptIds.OTP, IataCode = "OTP", IcaoCode = "LROP", Name = "Bucharest Henri Coanda Airport",City = "Bucharest",         Country = "Romania", TimeZone = "Europe/Bucharest", CreatedBy = "system" },
                // ── Serbia ───────────────────────────────────────────────────────────
                new Airport { Id = AptIds.BEG, IataCode = "BEG", IcaoCode = "LYBE", Name = "Belgrade Nikola Tesla Airport", City = "Belgrade",          Country = "Serbia", TimeZone = "Europe/Belgrade", CreatedBy = "system" },
                // ── Slovakia ─────────────────────────────────────────────────────────
                new Airport { Id = AptIds.BTS, IataCode = "BTS", IcaoCode = "LZIB", Name = "Bratislava Airport",            City = "Bratislava",        Country = "Slovakia", TimeZone = "Europe/Bratislava", CreatedBy = "system" },
                // ── Slovenia ─────────────────────────────────────────────────────────
                new Airport { Id = AptIds.LJU, IataCode = "LJU", IcaoCode = "LJLJ", Name = "Ljubljana Joze Pucnik Airport", City = "Ljubljana",         Country = "Slovenia", TimeZone = "Europe/Ljubljana", CreatedBy = "system" },
                // ── Portugal ─────────────────────────────────────────────────────────
                new Airport { Id = AptIds.LIS, IataCode = "LIS", IcaoCode = "LPPT", Name = "Lisbon Humberto Delgado Airport",City = "Lisbon",           Country = "Portugal", TimeZone = "Europe/Lisbon", CreatedBy = "system" },
                new Airport { Id = AptIds.OPO, IataCode = "OPO", IcaoCode = "LPPR", Name = "Porto Airport",                 City = "Porto",             Country = "Portugal", TimeZone = "Europe/Lisbon", CreatedBy = "system" },
                // ── Ireland ──────────────────────────────────────────────────────────
                new Airport { Id = AptIds.DUB, IataCode = "DUB", IcaoCode = "EIDW", Name = "Dublin Airport",                City = "Dublin",            Country = "Ireland", TimeZone = "Europe/Dublin", CreatedBy = "system" },
                // ── Ukraine ──────────────────────────────────────────────────────────
                new Airport { Id = AptIds.KBP, IataCode = "KBP", IcaoCode = "UKBB", Name = "Kyiv Boryspil Airport",         City = "Kyiv",              Country = "Ukraine", TimeZone = "Europe/Kiev", CreatedBy = "system" },
                new Airport { Id = AptIds.HRK, IataCode = "HRK", IcaoCode = "UKHH", Name = "Kharkiv International Airport", City = "Kharkiv",           Country = "Ukraine", TimeZone = "Europe/Kiev", CreatedBy = "system" },
                new Airport { Id = AptIds.LWO, IataCode = "LWO", IcaoCode = "UKLL", Name = "Lviv Danylo Halytskyi Airport", City = "Lviv",              Country = "Ukraine", TimeZone = "Europe/Kiev", CreatedBy = "system" },
                // ── Moldova ──────────────────────────────────────────────────────────
                new Airport { Id = AptIds.KIV, IataCode = "KIV", IcaoCode = "LUKK", Name = "Chisinau International Airport",City = "Chisinau",          Country = "Moldova", TimeZone = "Europe/Chisinau", CreatedBy = "system" },
                // ── Russia ───────────────────────────────────────────────────────────
                new Airport { Id = AptIds.SVO, IataCode = "SVO", IcaoCode = "UUEE", Name = "Moscow Sheremetyevo Airport",   City = "Moscow",            Country = "Russia", TimeZone = "Europe/Moscow", CreatedBy = "system" },
                new Airport { Id = AptIds.LED, IataCode = "LED", IcaoCode = "ULLI", Name = "St. Petersburg Pulkovo Airport",City = "St. Petersburg",    Country = "Russia", TimeZone = "Europe/Moscow", CreatedBy = "system" },
                new Airport { Id = AptIds.KRR, IataCode = "KRR", IcaoCode = "URKK", Name = "Krasnodar Pashkovsky Airport",  City = "Krasnodar",         Country = "Russia", TimeZone = "Europe/Moscow", CreatedBy = "system" },
                new Airport { Id = AptIds.MCX, IataCode = "MCX", IcaoCode = "URML", Name = "Makhachkala Uytash Airport",    City = "Makhachkala",       Country = "Russia", TimeZone = "Europe/Moscow", CreatedBy = "system" },
                new Airport { Id = AptIds.GRV, IataCode = "GRV", IcaoCode = "URMG", Name = "Grozny Airport",                City = "Grozny",            Country = "Russia", TimeZone = "Europe/Moscow", CreatedBy = "system" },
                // ── Armenia ──────────────────────────────────────────────────────────
                new Airport { Id = AptIds.EVN, IataCode = "EVN", IcaoCode = "UDYZ", Name = "Yerevan Zvartnots Airport",     City = "Yerevan",           Country = "Armenia", TimeZone = "Asia/Yerevan", CreatedBy = "system" },
                // ── Georgia ──────────────────────────────────────────────────────────
                new Airport { Id = AptIds.TBS, IataCode = "TBS", IcaoCode = "UGTB", Name = "Tbilisi International Airport", City = "Tbilisi",           Country = "Georgia", TimeZone = "Asia/Tbilisi", CreatedBy = "system" },
                new Airport { Id = AptIds.BUS, IataCode = "BUS", IcaoCode = "UGSB", Name = "Batumi International Airport",  City = "Batumi",            Country = "Georgia", TimeZone = "Asia/Tbilisi", CreatedBy = "system" },
                new Airport { Id = AptIds.KUT, IataCode = "KUT", IcaoCode = "UGKO", Name = "Kutaisi David the Builder Airport",City = "Kutaisi",        Country = "Georgia", TimeZone = "Asia/Tbilisi", CreatedBy = "system" },
                // ── Azerbaijan ───────────────────────────────────────────────────────
                new Airport { Id = AptIds.GYD, IataCode = "GYD", IcaoCode = "UBBB", Name = "Heydar Aliyev International",   City = "Baku",              Country = "Azerbaijan", TimeZone = "Asia/Baku", CreatedBy = "system" },
                new Airport { Id = AptIds.GNJ, IataCode = "GNJ", IcaoCode = "UBBG", Name = "Ganja International Airport",   City = "Ganja",             Country = "Azerbaijan", TimeZone = "Asia/Baku", CreatedBy = "system" },
                // ── Kazakhstan ───────────────────────────────────────────────────────
                new Airport { Id = AptIds.ALA, IataCode = "ALA", IcaoCode = "UAAA", Name = "Almaty International Airport",  City = "Almaty",            Country = "Kazakhstan", TimeZone = "Asia/Almaty", CreatedBy = "system" },
                new Airport { Id = AptIds.NQZ, IataCode = "NQZ", IcaoCode = "UACC", Name = "Astana International Airport",  City = "Astana",            Country = "Kazakhstan", TimeZone = "Asia/Qyzylorda", CreatedBy = "system" },
                new Airport { Id = AptIds.GUW, IataCode = "GUW", IcaoCode = "UATG", Name = "Atyrau Airport",                City = "Atyrau",            Country = "Kazakhstan", TimeZone = "Asia/Aqtau", CreatedBy = "system" },
                new Airport { Id = AptIds.SCO, IataCode = "SCO", IcaoCode = "UATE", Name = "Aktau Airport",                 City = "Aktau",             Country = "Kazakhstan", TimeZone = "Asia/Aqtau", CreatedBy = "system" },
                new Airport { Id = AptIds.AKX, IataCode = "AKX", IcaoCode = "UATT", Name = "Aktobe Airport",                City = "Aktobe",            Country = "Kazakhstan", TimeZone = "Asia/Aqtobe", CreatedBy = "system" },
                new Airport { Id = AptIds.CIT, IataCode = "CIT", IcaoCode = "UAII", Name = "Shymkent International Airport",City = "Shymkent",          Country = "Kazakhstan", TimeZone = "Asia/Almaty", CreatedBy = "system" },
                // ── Kyrgyzstan ───────────────────────────────────────────────────────
                new Airport { Id = AptIds.FRU, IataCode = "FRU", IcaoCode = "UAFM", Name = "Manas International Airport",   City = "Bishkek",           Country = "Kyrgyzstan", TimeZone = "Asia/Bishkek", CreatedBy = "system" },
                // ── Tajikistan ───────────────────────────────────────────────────────
                new Airport { Id = AptIds.DYU, IataCode = "DYU", IcaoCode = "UTDD", Name = "Dushanbe Airport",              City = "Dushanbe",          Country = "Tajikistan", TimeZone = "Asia/Dushanbe", CreatedBy = "system" },
                // ── Qatar ────────────────────────────────────────────────────────────
                new Airport { Id = AptIds.DOH, IataCode = "DOH", IcaoCode = "OTHH", Name = "Hamad International Airport",   City = "Doha",              Country = "Qatar", TimeZone = "Asia/Qatar", CreatedBy = "system" },
                // ── Kuwait ───────────────────────────────────────────────────────────
                new Airport { Id = AptIds.KWI, IataCode = "KWI", IcaoCode = "OKBK", Name = "Kuwait International Airport",  City = "Kuwait City",       Country = "Kuwait", TimeZone = "Asia/Kuwait", CreatedBy = "system" },
                // ── Bahrain ──────────────────────────────────────────────────────────
                new Airport { Id = AptIds.BAH, IataCode = "BAH", IcaoCode = "OBBI", Name = "Bahrain International Airport", City = "Manama",            Country = "Bahrain", TimeZone = "Asia/Bahrain", CreatedBy = "system" },
                // ── Saudi Arabia ─────────────────────────────────────────────────────
                new Airport { Id = AptIds.RUH, IataCode = "RUH", IcaoCode = "OERK", Name = "King Khalid International",     City = "Riyadh",            Country = "Saudi Arabia", TimeZone = "Asia/Riyadh", CreatedBy = "system" },
                new Airport { Id = AptIds.JED, IataCode = "JED", IcaoCode = "OEJN", Name = "King Abdulaziz International",  City = "Jeddah",            Country = "Saudi Arabia", TimeZone = "Asia/Riyadh", CreatedBy = "system" },
                new Airport { Id = AptIds.DMM, IataCode = "DMM", IcaoCode = "OEDF", Name = "King Fahd International",       City = "Dammam",            Country = "Saudi Arabia", TimeZone = "Asia/Riyadh", CreatedBy = "system" },
                // ── Iraq ─────────────────────────────────────────────────────────────
                new Airport { Id = AptIds.BGW, IataCode = "BGW", IcaoCode = "ORBI", Name = "Baghdad International Airport", City = "Baghdad",           Country = "Iraq", TimeZone = "Asia/Baghdad", CreatedBy = "system" },
                new Airport { Id = AptIds.BSR, IataCode = "BSR", IcaoCode = "ORMM", Name = "Basra International Airport",   City = "Basra",             Country = "Iraq", TimeZone = "Asia/Baghdad", CreatedBy = "system" },
                new Airport { Id = AptIds.EBL, IataCode = "EBL", IcaoCode = "ORER", Name = "Erbil International Airport",   City = "Erbil",             Country = "Iraq", TimeZone = "Asia/Baghdad", CreatedBy = "system" },
                // ── Jordan ───────────────────────────────────────────────────────────
                new Airport { Id = AptIds.AMM, IataCode = "AMM", IcaoCode = "OJAI", Name = "Queen Alia International",      City = "Amman",             Country = "Jordan", TimeZone = "Asia/Amman", CreatedBy = "system" },
                // ── Lebanon ──────────────────────────────────────────────────────────
                new Airport { Id = AptIds.BEY, IataCode = "BEY", IcaoCode = "OLBA", Name = "Beirut Rafic Hariri Airport",   City = "Beirut",            Country = "Lebanon", TimeZone = "Asia/Beirut", CreatedBy = "system" },
                // ── Syria ────────────────────────────────────────────────────────────
                new Airport { Id = AptIds.DAM, IataCode = "DAM", IcaoCode = "OSDI", Name = "Damascus International Airport",City = "Damascus",          Country = "Syria", TimeZone = "Asia/Damascus", CreatedBy = "system" },
                new Airport { Id = AptIds.ALP, IataCode = "ALP", IcaoCode = "OSAP", Name = "Aleppo International Airport",  City = "Aleppo",            Country = "Syria", TimeZone = "Asia/Damascus", CreatedBy = "system" },
                // ── Iran ─────────────────────────────────────────────────────────────
                new Airport { Id = AptIds.IKA, IataCode = "IKA", IcaoCode = "OIIE", Name = "Tehran Imam Khomeini Airport",  City = "Tehran",            Country = "Iran", TimeZone = "Asia/Tehran", CreatedBy = "system" },
                new Airport { Id = AptIds.IFN, IataCode = "IFN", IcaoCode = "OIFM", Name = "Isfahan International Airport", City = "Isfahan",           Country = "Iran", TimeZone = "Asia/Tehran", CreatedBy = "system" },
                // ── Pakistan ─────────────────────────────────────────────────────────
                new Airport { Id = AptIds.KHI, IataCode = "KHI", IcaoCode = "OPKC", Name = "Jinnah International Airport",  City = "Karachi",           Country = "Pakistan", TimeZone = "Asia/Karachi", CreatedBy = "system" },
                // ── Egypt ────────────────────────────────────────────────────────────
                new Airport { Id = AptIds.CAI, IataCode = "CAI", IcaoCode = "HECA", Name = "Cairo International Airport",   City = "Cairo",             Country = "Egypt", TimeZone = "Africa/Cairo", CreatedBy = "system" },
                new Airport { Id = AptIds.HRG, IataCode = "HRG", IcaoCode = "HEGN", Name = "Hurghada International Airport",City = "Hurghada",          Country = "Egypt", TimeZone = "Africa/Cairo", CreatedBy = "system" },
                new Airport { Id = AptIds.SSH, IataCode = "SSH", IcaoCode = "HESH", Name = "Sharm el-Sheikh Airport",       City = "Sharm el-Sheikh",   Country = "Egypt", TimeZone = "Africa/Cairo", CreatedBy = "system" },
                new Airport { Id = AptIds.LXR, IataCode = "LXR", IcaoCode = "HELX", Name = "Luxor International Airport",   City = "Luxor",             Country = "Egypt", TimeZone = "Africa/Cairo", CreatedBy = "system" },
                new Airport { Id = AptIds.HBE, IataCode = "HBE", IcaoCode = "HEBA", Name = "Borg el Arab Airport",          City = "Alexandria",        Country = "Egypt", TimeZone = "Africa/Cairo", CreatedBy = "system" },
                // ── Algeria ──────────────────────────────────────────────────────────
                new Airport { Id = AptIds.ALG, IataCode = "ALG", IcaoCode = "DAAG", Name = "Algiers Houari Boumediene",     City = "Algiers",           Country = "Algeria", TimeZone = "Africa/Algiers", CreatedBy = "system" },
                // ── Morocco ──────────────────────────────────────────────────────────
                new Airport { Id = AptIds.CMN, IataCode = "CMN", IcaoCode = "GMMN", Name = "Mohammed V International",      City = "Casablanca",        Country = "Morocco", TimeZone = "Africa/Casablanca", CreatedBy = "system" },
                // ── Singapore ────────────────────────────────────────────────────────
                new Airport { Id = AptIds.SIN, IataCode = "SIN", IcaoCode = "WSSS", Name = "Changi Airport",                City = "Singapore",         Country = "Singapore", TimeZone = "Asia/Singapore", CreatedBy = "system" },
                // ── Thailand ─────────────────────────────────────────────────────────
                new Airport { Id = AptIds.BKK, IataCode = "BKK", IcaoCode = "VTBS", Name = "Suvarnabhumi Airport",          City = "Bangkok",           Country = "Thailand", TimeZone = "Asia/Bangkok", CreatedBy = "system" },
                new Airport { Id = AptIds.HKT, IataCode = "HKT", IcaoCode = "VTSP", Name = "Phuket International Airport",  City = "Phuket",            Country = "Thailand", TimeZone = "Asia/Bangkok", CreatedBy = "system" },
                // ── Japan ────────────────────────────────────────────────────────────
                new Airport { Id = AptIds.NRT, IataCode = "NRT", IcaoCode = "RJAA", Name = "Tokyo Narita Airport",          City = "Tokyo",             Country = "Japan", TimeZone = "Asia/Tokyo", CreatedBy = "system" },
                // ── China ────────────────────────────────────────────────────────────
                new Airport { Id = AptIds.PEK, IataCode = "PEK", IcaoCode = "ZBAA", Name = "Beijing Capital Airport",       City = "Beijing",           Country = "China", TimeZone = "Asia/Shanghai", CreatedBy = "system" },
                // ── India ────────────────────────────────────────────────────────────
                new Airport { Id = AptIds.BOM, IataCode = "BOM", IcaoCode = "VABB", Name = "Chhatrapati Shivaji Airport",   City = "Mumbai",            Country = "India", TimeZone = "Asia/Kolkata", CreatedBy = "system" },
                new Airport { Id = AptIds.DEL, IataCode = "DEL", IcaoCode = "VIDP", Name = "Indira Gandhi International",   City = "New Delhi",         Country = "India", TimeZone = "Asia/Kolkata", CreatedBy = "system" },
                // ── Colombia ─────────────────────────────────────────────────────────
                new Airport { Id = AptIds.BOG, IataCode = "BOG", IcaoCode = "SKBO", Name = "El Dorado International Airport",City = "Bogota",           Country = "Colombia", TimeZone = "America/Bogota", CreatedBy = "system" }
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
                // Returns to IST
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
                // Trans-Atlantic
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
                // THY domestic
                new Schedule { Id = SchIds.TK7_IST_ESB,    RouteId = RouteIds.IST_ESB, Code = "TK7-IST-ESB",    ValidFrom = v, DaysOfWeek = DaysOfWeek.Daily, DepartureTime = TimeSpan.FromHours(7),    TimeZone = "Europe/Istanbul", CreatedBy = "system" },
                new Schedule { Id = SchIds.TK9_IST_ESB,    RouteId = RouteIds.IST_ESB, Code = "TK9-IST-ESB",    ValidFrom = v, DaysOfWeek = DaysOfWeek.Daily, DepartureTime = TimeSpan.FromHours(13),   TimeZone = "Europe/Istanbul", CreatedBy = "system" },
                new Schedule { Id = SchIds.TK1793_IST_ADB, RouteId = RouteIds.IST_ADB, Code = "TK1793-IST-ADB", ValidFrom = v, DaysOfWeek = DaysOfWeek.Daily, DepartureTime = TimeSpan.FromHours(7),    TimeZone = "Europe/Istanbul", CreatedBy = "system" },
                new Schedule { Id = SchIds.TK1795_IST_ADB, RouteId = RouteIds.IST_ADB, Code = "TK1795-IST-ADB", ValidFrom = v, DaysOfWeek = DaysOfWeek.Daily, DepartureTime = TimeSpan.FromHours(12.5), TimeZone = "Europe/Istanbul", CreatedBy = "system" },
                new Schedule { Id = SchIds.TK2120_IST_AYT, RouteId = RouteIds.IST_AYT, Code = "TK2120-IST-AYT", ValidFrom = v, DaysOfWeek = DaysOfWeek.Daily, DepartureTime = TimeSpan.FromHours(8),    TimeZone = "Europe/Istanbul", CreatedBy = "system" },
                new Schedule { Id = SchIds.TK2122_IST_AYT, RouteId = RouteIds.IST_AYT, Code = "TK2122-IST-AYT", ValidFrom = v, DaysOfWeek = DaysOfWeek.Daily, DepartureTime = TimeSpan.FromHours(14),   TimeZone = "Europe/Istanbul", CreatedBy = "system" },
                // THY international
                new Schedule { Id = SchIds.TK1_IST_LHR,   RouteId = RouteIds.IST_LHR, Code = "TK1-IST-LHR",    ValidFrom = v, DaysOfWeek = DaysOfWeek.Daily, DepartureTime = TimeSpan.FromHours(8),    TimeZone = "Europe/Istanbul", CreatedBy = "system" },
                new Schedule { Id = SchIds.TK3_IST_LHR,   RouteId = RouteIds.IST_LHR, Code = "TK3-IST-LHR",    ValidFrom = v, DaysOfWeek = DaysOfWeek.Daily, DepartureTime = TimeSpan.FromHours(16),   TimeZone = "Europe/Istanbul", CreatedBy = "system" },
                new Schedule { Id = SchIds.TK11_IST_DXB,  RouteId = RouteIds.IST_DXB, Code = "TK11-IST-DXB",   ValidFrom = v, DaysOfWeek = DaysOfWeek.Daily, DepartureTime = TimeSpan.FromHours(10),   TimeZone = "Europe/Istanbul", CreatedBy = "system" },
                new Schedule { Id = SchIds.TK13_IST_DXB,  RouteId = RouteIds.IST_DXB, Code = "TK13-IST-DXB",   ValidFrom = v, DaysOfWeek = DaysOfWeek.Daily, DepartureTime = TimeSpan.FromHours(18),   TimeZone = "Europe/Istanbul", CreatedBy = "system" },
                new Schedule { Id = SchIds.TK751_IST_FRA, RouteId = RouteIds.IST_FRA, Code = "TK751-IST-FRA",  ValidFrom = v, DaysOfWeek = DaysOfWeek.Daily, DepartureTime = TimeSpan.FromHours(9),    TimeZone = "Europe/Istanbul", CreatedBy = "system" },
                new Schedule { Id = SchIds.TK1841_IST_CDG,RouteId = RouteIds.IST_CDG, Code = "TK1841-IST-CDG", ValidFrom = v, DaysOfWeek = DaysOfWeek.Daily, DepartureTime = TimeSpan.FromHours(8),    TimeZone = "Europe/Istanbul", CreatedBy = "system" },
                new Schedule { Id = SchIds.TK2010_IST_AMS,RouteId = RouteIds.IST_AMS, Code = "TK2010-IST-AMS", ValidFrom = v, DaysOfWeek = DaysOfWeek.Daily, DepartureTime = TimeSpan.FromHours(10),   TimeZone = "Europe/Istanbul", CreatedBy = "system" },
                new Schedule { Id = SchIds.TK1881_IST_FCO,RouteId = RouteIds.IST_FCO, Code = "TK1881-IST-FCO", ValidFrom = v, DaysOfWeek = DaysOfWeek.Daily, DepartureTime = TimeSpan.FromHours(9.5),  TimeZone = "Europe/Istanbul", CreatedBy = "system" },
                new Schedule { Id = SchIds.TK1851_IST_MAD,RouteId = RouteIds.IST_MAD, Code = "TK1851-IST-MAD", ValidFrom = v, DaysOfWeek = DaysOfWeek.Daily, DepartureTime = TimeSpan.FromHours(10),   TimeZone = "Europe/Istanbul", CreatedBy = "system" },
                new Schedule { Id = SchIds.TK3_IST_JFK,   RouteId = RouteIds.IST_JFK, Code = "TK3-IST-JFK",    ValidFrom = v, DaysOfWeek = DaysOfWeek.Daily, DepartureTime = TimeSpan.FromHours(23.5), TimeZone = "Europe/Istanbul", CreatedBy = "system" },
                new Schedule { Id = SchIds.TK203_IST_LAX, RouteId = RouteIds.IST_LAX, Code = "TK203-IST-LAX",  ValidFrom = v, DaysOfWeek = DaysOfWeek.Daily, DepartureTime = TimeSpan.FromHours(22),   TimeZone = "Europe/Istanbul", CreatedBy = "system" },
                // Returns
                new Schedule { Id = SchIds.TK2_LHR_IST,   RouteId = RouteIds.LHR_IST, Code = "TK2-LHR-IST",    ValidFrom = v, DaysOfWeek = DaysOfWeek.Daily, DepartureTime = TimeSpan.FromHours(11),   TimeZone = "Europe/London",   CreatedBy = "system" },
                new Schedule { Id = SchIds.TK12_DXB_IST,  RouteId = RouteIds.DXB_IST, Code = "TK12-DXB-IST",   ValidFrom = v, DaysOfWeek = DaysOfWeek.Daily, DepartureTime = TimeSpan.FromHours(8),    TimeZone = "Asia/Dubai",      CreatedBy = "system" },
                new Schedule { Id = SchIds.LH1290_FRA_IST,RouteId = RouteIds.FRA_IST, Code = "LH1290-FRA-IST",  ValidFrom = v, DaysOfWeek = DaysOfWeek.Daily, DepartureTime = TimeSpan.FromHours(10),   TimeZone = "Europe/Berlin",   CreatedBy = "system" },
                new Schedule { Id = SchIds.AF1471_CDG_IST,RouteId = RouteIds.CDG_IST, Code = "AF1471-CDG-IST",  ValidFrom = v, DaysOfWeek = DaysOfWeek.Daily, DepartureTime = TimeSpan.FromHours(8),    TimeZone = "Europe/Paris",    CreatedBy = "system" },
                new Schedule { Id = SchIds.BA686_LHR_IST, RouteId = RouteIds.LHR_IST, Code = "BA686-LHR-IST",   ValidFrom = v, DaysOfWeek = DaysOfWeek.Daily, DepartureTime = TimeSpan.FromHours(14),   TimeZone = "Europe/London",   CreatedBy = "system" },
                new Schedule { Id = SchIds.TK752_AMS_IST, RouteId = RouteIds.AMS_IST, Code = "TK752-AMS-IST",   ValidFrom = v, DaysOfWeek = DaysOfWeek.Daily, DepartureTime = TimeSpan.FromHours(14),   TimeZone = "Europe/Amsterdam",CreatedBy = "system" },
                // Emirates DXB
                new Schedule { Id = SchIds.EK1_DXB_LHR,   RouteId = RouteIds.DXB_LHR, Code = "EK1-DXB-LHR",    ValidFrom = v, DaysOfWeek = DaysOfWeek.Daily, DepartureTime = TimeSpan.FromHours(8),    TimeZone = "Asia/Dubai",      CreatedBy = "system" },
                new Schedule { Id = SchIds.EK3_DXB_LHR,   RouteId = RouteIds.DXB_LHR, Code = "EK3-DXB-LHR",    ValidFrom = v, DaysOfWeek = DaysOfWeek.Daily, DepartureTime = TimeSpan.FromHours(14.5), TimeZone = "Asia/Dubai",      CreatedBy = "system" },
                new Schedule { Id = SchIds.EK352_DXB_SIN,  RouteId = RouteIds.DXB_SIN, Code = "EK352-DXB-SIN",   ValidFrom = v, DaysOfWeek = DaysOfWeek.Daily, DepartureTime = TimeSpan.FromHours(2.5),  TimeZone = "Asia/Dubai",      CreatedBy = "system" },
                new Schedule { Id = SchIds.EK354_DXB_SIN,  RouteId = RouteIds.DXB_SIN, Code = "EK354-DXB-SIN",   ValidFrom = v, DaysOfWeek = DaysOfWeek.Daily, DepartureTime = TimeSpan.FromHours(14),   TimeZone = "Asia/Dubai",      CreatedBy = "system" },
                new Schedule { Id = SchIds.EK201_DXB_JFK,  RouteId = RouteIds.DXB_JFK, Code = "EK201-DXB-JFK",   ValidFrom = v, DaysOfWeek = DaysOfWeek.Daily, DepartureTime = TimeSpan.FromHours(2),    TimeZone = "Asia/Dubai",      CreatedBy = "system" },
                new Schedule { Id = SchIds.EK2_LHR_DXB,   RouteId = RouteIds.LHR_DXB, Code = "EK2-LHR-DXB",    ValidFrom = v, DaysOfWeek = DaysOfWeek.Daily, DepartureTime = TimeSpan.FromHours(9),    TimeZone = "Europe/London",   CreatedBy = "system" },
                new Schedule { Id = SchIds.EK374_DXB_BKK,  RouteId = RouteIds.DXB_BKK, Code = "EK374-DXB-BKK",   ValidFrom = v, DaysOfWeek = DaysOfWeek.Daily, DepartureTime = TimeSpan.FromHours(3.5),  TimeZone = "Asia/Dubai",      CreatedBy = "system" },
                // Lufthansa / BA trans-Atlantic
                new Schedule { Id = SchIds.LH400_FRA_JFK,  RouteId = RouteIds.FRA_JFK, Code = "LH400-FRA-JFK",   ValidFrom = v, DaysOfWeek = DaysOfWeek.Daily, DepartureTime = TimeSpan.FromHours(10.5), TimeZone = "Europe/Berlin",   CreatedBy = "system" },
                new Schedule { Id = SchIds.BA175_LHR_JFK,  RouteId = RouteIds.LHR_JFK, Code = "BA175-LHR-JFK",   ValidFrom = v, DaysOfWeek = DaysOfWeek.Daily, DepartureTime = TimeSpan.FromHours(11),   TimeZone = "Europe/London",   CreatedBy = "system" },
                new Schedule { Id = SchIds.BA177_LHR_JFK,  RouteId = RouteIds.LHR_JFK, Code = "BA177-LHR-JFK",   ValidFrom = v, DaysOfWeek = DaysOfWeek.Daily, DepartureTime = TimeSpan.FromHours(15),   TimeZone = "Europe/London",   CreatedBy = "system" }
            );
            await db.SaveChangesAsync();
        }

        // ── Aircraft ──────────────────────────────────────────────────────────────

        private static async Task SeedAircraftsAsync(FlightReservationDbContext db)
        {
            if (await db.Aircrafts.AnyAsync()) return;
            db.Aircrafts.AddRange(
                new Aircraft { Id = AcIds.TCLNA,  TailNumber = "TC-LNA",  ManufactureYear = 2018, AircraftStatus = AircraftStatus.Active, AirlineId = AirlineIds.THY, ModelId = ModelIds.B777300ER, CreatedBy = "system" },
                new Aircraft { Id = AcIds.TCKUE,  TailNumber = "TC-KUE",  ManufactureYear = 2020, AircraftStatus = AircraftStatus.Active, AirlineId = AirlineIds.THY, ModelId = ModelIds.A320Neo,   CreatedBy = "system" },
                new Aircraft { Id = AcIds.TCJJK,  TailNumber = "TC-JJK",  ManufactureYear = 2019, AircraftStatus = AircraftStatus.Active, AirlineId = AirlineIds.THY, ModelId = ModelIds.A350900,   CreatedBy = "system" },
                new Aircraft { Id = AcIds.TCLJT,  TailNumber = "TC-LJT",  ManufactureYear = 2021, AircraftStatus = AircraftStatus.Active, AirlineId = AirlineIds.THY, ModelId = ModelIds.B7879,     CreatedBy = "system" },
                new Aircraft { Id = AcIds.TCJHM,  TailNumber = "TC-JHM",  ManufactureYear = 2022, AircraftStatus = AircraftStatus.Active, AirlineId = AirlineIds.THY, ModelId = ModelIds.A320Neo,   CreatedBy = "system" },
                new Aircraft { Id = AcIds.A6EDB,  TailNumber = "A6-EDB",  ManufactureYear = 2016, AircraftStatus = AircraftStatus.Active, AirlineId = AirlineIds.EK,  ModelId = ModelIds.A350900,   CreatedBy = "system" },
                new Aircraft { Id = AcIds.A6EWC,  TailNumber = "A6-EWC",  ManufactureYear = 2017, AircraftStatus = AircraftStatus.Active, AirlineId = AirlineIds.EK,  ModelId = ModelIds.A380800,   CreatedBy = "system" },
                new Aircraft { Id = AcIds.A6EDM,  TailNumber = "A6-EDM",  ManufactureYear = 2015, AircraftStatus = AircraftStatus.Active, AirlineId = AirlineIds.EK,  ModelId = ModelIds.B777300ER, CreatedBy = "system" },
                new Aircraft { Id = AcIds.A6ENB,  TailNumber = "A6-ENB",  ManufactureYear = 2019, AircraftStatus = AircraftStatus.Active, AirlineId = AirlineIds.EK,  ModelId = ModelIds.B777300ER, CreatedBy = "system" },
                new Aircraft { Id = AcIds.YBBSA,  TailNumber = "YB-BSA",  ManufactureYear = 2019, AircraftStatus = AircraftStatus.Active, AirlineId = AirlineIds.PC,  ModelId = ModelIds.A320Neo,   CreatedBy = "system" },
                new Aircraft { Id = AcIds.YBBSB,  TailNumber = "YB-BSB",  ManufactureYear = 2020, AircraftStatus = AircraftStatus.Active, AirlineId = AirlineIds.PC,  ModelId = ModelIds.A320Neo,   CreatedBy = "system" },
                new Aircraft { Id = AcIds.YBBSC,  TailNumber = "YB-BSC",  ManufactureYear = 2018, AircraftStatus = AircraftStatus.Active, AirlineId = AirlineIds.PC,  ModelId = ModelIds.B737800,   CreatedBy = "system" },
                new Aircraft { Id = AcIds.DAIXA,  TailNumber = "D-AIXA",  ManufactureYear = 2020, AircraftStatus = AircraftStatus.Active, AirlineId = AirlineIds.LH,  ModelId = ModelIds.A350900,   CreatedBy = "system" },
                new Aircraft { Id = AcIds.DAIXB,  TailNumber = "D-AIXB",  ManufactureYear = 2021, AircraftStatus = AircraftStatus.Active, AirlineId = AirlineIds.LH,  ModelId = ModelIds.A320Neo,   CreatedBy = "system" },
                new Aircraft { Id = AcIds.DAIXC,  TailNumber = "D-AIXC",  ManufactureYear = 2019, AircraftStatus = AircraftStatus.Active, AirlineId = AirlineIds.LH,  ModelId = ModelIds.B777300ER, CreatedBy = "system" },
                new Aircraft { Id = AcIds.GXWBA,  TailNumber = "G-XWBA",  ManufactureYear = 2020, AircraftStatus = AircraftStatus.Active, AirlineId = AirlineIds.BA,  ModelId = ModelIds.B7879,     CreatedBy = "system" },
                new Aircraft { Id = AcIds.GEUXA,  TailNumber = "G-EUXA",  ManufactureYear = 2019, AircraftStatus = AircraftStatus.Active, AirlineId = AirlineIds.BA,  ModelId = ModelIds.A320Neo,   CreatedBy = "system" },
                new Aircraft { Id = AcIds.GXWBB,  TailNumber = "G-XWBB",  ManufactureYear = 2018, AircraftStatus = AircraftStatus.Active, AirlineId = AirlineIds.BA,  ModelId = ModelIds.B777300ER, CreatedBy = "system" },
                new Aircraft { Id = AcIds.FGZNX,  TailNumber = "F-GZNX",  ManufactureYear = 2017, AircraftStatus = AircraftStatus.Active, AirlineId = AirlineIds.AF,  ModelId = ModelIds.B777300ER, CreatedBy = "system" },
                new Aircraft { Id = AcIds.FHPJH,  TailNumber = "F-HPJH",  ManufactureYear = 2020, AircraftStatus = AircraftStatus.Active, AirlineId = AirlineIds.AF,  ModelId = ModelIds.A350900,   CreatedBy = "system" },
                new Aircraft { Id = AcIds.FGKXA,  TailNumber = "F-GKXA",  ManufactureYear = 2021, AircraftStatus = AircraftStatus.Active, AirlineId = AirlineIds.AF,  ModelId = ModelIds.A320Neo,   CreatedBy = "system" }
            );
            await db.SaveChangesAsync();
        }

        // ── Seats ─────────────────────────────────────────────────────────────────

        private static async Task SeedSeatsAsync(FlightReservationDbContext db)
        {
            if (await db.Seats.AnyAsync()) return;
            var seats = new List<Seat>();
            // THY
            GenerateSeats(seats, AcIds.TCLNA,  true,  firstRows:2, businessRows:4, premiumRows:3, economyRows:22);
            GenerateSeats(seats, AcIds.TCKUE,  false, firstRows:0, businessRows:3, premiumRows:0, economyRows:20);
            GenerateSeats(seats, AcIds.TCJJK,  true,  firstRows:2, businessRows:4, premiumRows:3, economyRows:20);
            GenerateSeats(seats, AcIds.TCLJT,  true,  firstRows:0, businessRows:3, premiumRows:2, economyRows:18);
            GenerateSeats(seats, AcIds.TCJHM,  false, firstRows:0, businessRows:3, premiumRows:0, economyRows:20);
            // Emirates
            GenerateSeats(seats, AcIds.A6EDB,  true,  firstRows:2, businessRows:4, premiumRows:3, economyRows:20);
            GenerateSeats(seats, AcIds.A6EWC,  true,  firstRows:2, businessRows:6, premiumRows:4, economyRows:24);
            GenerateSeats(seats, AcIds.A6EDM,  true,  firstRows:2, businessRows:4, premiumRows:3, economyRows:22);
            GenerateSeats(seats, AcIds.A6ENB,  true,  firstRows:2, businessRows:4, premiumRows:3, economyRows:22);
            // Pegasus
            GenerateSeats(seats, AcIds.YBBSA,  false, firstRows:0, businessRows:3, premiumRows:0, economyRows:20);
            GenerateSeats(seats, AcIds.YBBSB,  false, firstRows:0, businessRows:3, premiumRows:0, economyRows:20);
            GenerateSeats(seats, AcIds.YBBSC,  false, firstRows:0, businessRows:3, premiumRows:0, economyRows:20);
            // Lufthansa
            GenerateSeats(seats, AcIds.DAIXA,  true,  firstRows:2, businessRows:4, premiumRows:3, economyRows:20);
            GenerateSeats(seats, AcIds.DAIXB,  false, firstRows:0, businessRows:3, premiumRows:0, economyRows:20);
            GenerateSeats(seats, AcIds.DAIXC,  true,  firstRows:2, businessRows:4, premiumRows:3, economyRows:22);
            // British Airways
            GenerateSeats(seats, AcIds.GXWBA,  true,  firstRows:0, businessRows:3, premiumRows:2, economyRows:18);
            GenerateSeats(seats, AcIds.GEUXA,  false, firstRows:0, businessRows:3, premiumRows:0, economyRows:20);
            GenerateSeats(seats, AcIds.GXWBB,  true,  firstRows:2, businessRows:4, premiumRows:3, economyRows:22);
            // Air France
            GenerateSeats(seats, AcIds.FGZNX,  true,  firstRows:2, businessRows:4, premiumRows:3, economyRows:22);
            GenerateSeats(seats, AcIds.FHPJH,  true,  firstRows:2, businessRows:4, premiumRows:3, economyRows:20);
            GenerateSeats(seats, AcIds.FGKXA,  false, firstRows:0, businessRows:3, premiumRows:0, economyRows:20);
            db.Seats.AddRange(seats);
            await db.SaveChangesAsync();
        }

        private static void GenerateSeats(List<Seat> seats, Guid aircraftId, bool isWideBody,
            int firstRows, int businessRows, int premiumRows, int economyRows)
        {
            var firstCols      = new[] { SeatColumn.A, SeatColumn.B, SeatColumn.C, SeatColumn.D };
            var bizColsWide    = new[] { SeatColumn.A, SeatColumn.B, SeatColumn.C, SeatColumn.D, SeatColumn.E, SeatColumn.F };
            var bizColsNarrow  = new[] { SeatColumn.A, SeatColumn.B, SeatColumn.C, SeatColumn.D };
            var wideBodyCols   = new[] { SeatColumn.A, SeatColumn.B, SeatColumn.C, SeatColumn.D, SeatColumn.E, SeatColumn.F, SeatColumn.G, SeatColumn.H, SeatColumn.J, SeatColumn.K };
            var narrowBodyCols = new[] { SeatColumn.A, SeatColumn.B, SeatColumn.C, SeatColumn.D, SeatColumn.E, SeatColumn.F };

            bool IsWindow(SeatColumn col) => isWideBody ? (col == SeatColumn.A || col == SeatColumn.K) : (col == SeatColumn.A || col == SeatColumn.F);
            bool IsAisle(SeatColumn col)  => isWideBody
                ? (col == SeatColumn.C || col == SeatColumn.D || col == SeatColumn.G || col == SeatColumn.H)
                : (col == SeatColumn.C || col == SeatColumn.D);

            int row = 1;
            void AddRows(int count, SeatClass cls, SeatColumn[] cols)
            {
                for (int r = row; r < row + count; r++)
                    foreach (var col in cols)
                        seats.Add(new Seat { AircraftId = aircraftId, Row = r, Column = col, SeatClass = cls, IsWindowSeat = IsWindow(col), IsAisleSeat = IsAisle(col), HasExtraLegRoom = (r == row), CreatedBy = "system" });
                row += count;
            }

            if (firstRows    > 0) AddRows(firstRows,    SeatClass.First,          firstCols);
            if (businessRows > 0) AddRows(businessRows, SeatClass.Business,       isWideBody ? bizColsWide : bizColsNarrow);
            if (premiumRows  > 0) AddRows(premiumRows,  SeatClass.PremiumEconomy, wideBodyCols);
            if (economyRows  > 0) AddRows(economyRows,  SeatClass.Economy,        isWideBody ? wideBodyCols : narrowBodyCols);
        }

        // ── Flights ───────────────────────────────────────────────────────────────
        // 50 Arrived (25 economy narrow-body domestic + 25 business wide-body international)
        // 50 Scheduled (25 economy + 25 business) — same split
        private static async Task SeedFlightsAsync(FlightReservationDbContext db)
        {
            if (await db.Flights.AnyAsync()) return;
            var today  = DateTime.UtcNow.Date;
            var flights = new List<Flight>();

            void Add(string number, DateTime dep, double durH,
                decimal eco, decimal peco, decimal biz, decimal first,
                Currency cur, Guid acId, Guid alId, Guid schId, FlightStatus status)
            {
                flights.Add(new Flight
                {
                    Number = number,
                    DepartureDateTime = dep,
                    ArrivalDateTime   = dep.AddHours(durH),
                    Duration          = TimeSpan.FromHours(durH),
                    BaseEconomyPrice  = eco, BasePremiumEconomyPrice = peco,
                    BaseBusinessPrice = biz, BaseFirstClassPrice     = first,
                    Currency = cur, FlightStatus = status,
                    AircraftId = acId, AirlineId = alId, ScheduleId = schId,
                    CreatedBy = "system"
                });
            }

            // ── 25 Economy Arrived (domestic, narrow-body, past) ──────────────────
            // IST→ESB — TK7 at 07:00, days -1 to -9
            for (int i = 1; i <= 9; i++)
                Add("TK7",    today.AddDays(-i).AddHours(7),    1.17, 2300,3500,7000,7000, Currency.TRY, AcIds.TCKUE,  AirlineIds.THY, SchIds.TK7_IST_ESB,    FlightStatus.Arrived);
            // IST→ADB — TK1793 at 07:00, days -1 to -8
            for (int i = 1; i <= 8; i++)
                Add("TK1793", today.AddDays(-i).AddHours(7),    1.33, 2500,3800,8000,8000, Currency.TRY, AcIds.TCJHM,  AirlineIds.THY, SchIds.TK1793_IST_ADB, FlightStatus.Arrived);
            // IST→AYT — TK2120 at 08:00, days -1 to -8
            for (int i = 1; i <= 8; i++)
                Add("TK2120", today.AddDays(-i).AddHours(8),    1.5,  2800,4200,9000,9000, Currency.TRY, AcIds.YBBSA,  AirlineIds.PC,  SchIds.TK2120_IST_AYT, FlightStatus.Arrived);

            // ── 25 Business Arrived (international, wide-body, past) ──────────────
            // IST→LHR — TK1 at 08:00, days -1 to -4
            for (int i = 1; i <= 4; i++)
                Add("TK1",    today.AddDays(-i).AddHours(8),    4,    450,750,1800,3500,  Currency.USD, AcIds.TCLNA,  AirlineIds.THY, SchIds.TK1_IST_LHR,    FlightStatus.Arrived);
            // IST→DXB — TK11 at 10:00, days -1 to -4
            for (int i = 1; i <= 4; i++)
                Add("TK11",   today.AddDays(-i).AddHours(10),   4.5,  380,650,1600,3200,  Currency.USD, AcIds.TCLNA,  AirlineIds.THY, SchIds.TK11_IST_DXB,   FlightStatus.Arrived);
            // IST→FRA — TK751 at 09:00, days -1 to -3
            for (int i = 1; i <= 3; i++)
                Add("TK751",  today.AddDays(-i).AddHours(9),    3.5,  320,540,1400,2800,  Currency.EUR, AcIds.TCJJK,  AirlineIds.THY, SchIds.TK751_IST_FRA,  FlightStatus.Arrived);
            // IST→CDG — TK1841 at 08:00, days -1 to -3  (08:00 differs from TK1 08:00 via number)
            for (int i = 1; i <= 3; i++)
                Add("TK1841", today.AddDays(-i).AddHours(8).AddMinutes(30), 3.75, 310,520,1350,2700, Currency.EUR, AcIds.TCLJT, AirlineIds.THY, SchIds.TK1841_IST_CDG, FlightStatus.Arrived);
            // IST→JFK — TK3j at 23:30, days -1 to -3
            for (int i = 1; i <= 3; i++)
                Add("TK3",    today.AddDays(-i).AddHours(23.5), 10.5, 650,1100,2800,5500, Currency.USD, AcIds.TCLJT,  AirlineIds.THY, SchIds.TK3_IST_JFK,    FlightStatus.Arrived);
            // DXB→LHR — EK1 at 08:00, days -1 to -4
            for (int i = 1; i <= 4; i++)
                Add("EK1",    today.AddDays(-i).AddHours(8),    7.5,  620,1050,2800,5600, Currency.USD, AcIds.A6EWC,  AirlineIds.EK,  SchIds.EK1_DXB_LHR,    FlightStatus.Arrived);
            // DXB→SIN — EK352 at 02:30, days -1 to -4
            for (int i = 1; i <= 4; i++)
                Add("EK352",  today.AddDays(-i).AddHours(2.5),  7.5,  550,940,2500,5000,  Currency.USD, AcIds.A6EDM,  AirlineIds.EK,  SchIds.EK352_DXB_SIN,  FlightStatus.Arrived);

            // ── 25 Economy Scheduled (domestic, narrow-body, future) ──────────────
            // IST→ESB — TK9 at 13:00, days +1 to +9
            for (int i = 1; i <= 9; i++)
                Add("TK9",    today.AddDays(i).AddHours(13),    1.17, 2300,3500,7000,7000, Currency.TRY, AcIds.TCJHM,  AirlineIds.THY, SchIds.TK9_IST_ESB,    FlightStatus.Scheduled);
            // IST→ADB — TK1795 at 12:30, days +1 to +8
            for (int i = 1; i <= 8; i++)
                Add("TK1795", today.AddDays(i).AddHours(12.5),  1.33, 2500,3800,8000,8000, Currency.TRY, AcIds.TCKUE,  AirlineIds.THY, SchIds.TK1795_IST_ADB, FlightStatus.Scheduled);
            // IST→AYT — TK2122 at 14:00, days +1 to +8
            for (int i = 1; i <= 8; i++)
                Add("TK2122", today.AddDays(i).AddHours(14),    1.5,  2800,4200,9000,9000, Currency.TRY, AcIds.YBBSB,  AirlineIds.PC,  SchIds.TK2122_IST_AYT, FlightStatus.Scheduled);

            // ── 25 Business Scheduled (international, wide-body, future) ──────────
            // IST→LHR — TK3 at 16:00, days +1 to +4
            for (int i = 1; i <= 4; i++)
                Add("TK3",    today.AddDays(i).AddHours(16),    4,    450,750,1800,3500,  Currency.USD, AcIds.TCJJK,  AirlineIds.THY, SchIds.TK3_IST_LHR,    FlightStatus.Scheduled);
            // IST→DXB — TK13 at 18:00, days +1 to +4
            for (int i = 1; i <= 4; i++)
                Add("TK13",   today.AddDays(i).AddHours(18),    4.5,  380,650,1600,3200,  Currency.USD, AcIds.TCLNA,  AirlineIds.THY, SchIds.TK13_IST_DXB,   FlightStatus.Scheduled);
            // FRA→IST — LH1290 at 10:00, days +1 to +3
            for (int i = 1; i <= 3; i++)
                Add("LH1290", today.AddDays(i).AddHours(10),    3.5,  310,520,1400,2800,  Currency.EUR, AcIds.DAIXA,  AirlineIds.LH,  SchIds.LH1290_FRA_IST, FlightStatus.Scheduled);
            // CDG→IST — AF1471 at 08:00, days +1 to +3
            for (int i = 1; i <= 3; i++)
                Add("AF1471", today.AddDays(i).AddHours(8),     3.75, 300,510,1350,2700,  Currency.EUR, AcIds.FGZNX,  AirlineIds.AF,  SchIds.AF1471_CDG_IST, FlightStatus.Scheduled);
            // IST→JFK — TK3 at 23:30, days +1 to +3 (different time from TK3 16:00)
            for (int i = 1; i <= 3; i++)
                Add("TK3",    today.AddDays(i).AddHours(23.5),  10.5, 650,1100,2800,5500, Currency.USD, AcIds.TCLJT,  AirlineIds.THY, SchIds.TK3_IST_JFK,    FlightStatus.Scheduled);
            // DXB→LHR — EK3 at 14:30, days +1 to +4
            for (int i = 1; i <= 4; i++)
                Add("EK3",    today.AddDays(i).AddHours(14.5),  7.5,  620,1050,2800,5600, Currency.USD, AcIds.A6EWC,  AirlineIds.EK,  SchIds.EK3_DXB_LHR,    FlightStatus.Scheduled);
            // DXB→SIN — EK354 at 14:00, days +1 to +4
            for (int i = 1; i <= 4; i++)
                Add("EK354",  today.AddDays(i).AddHours(14),    7.5,  550,940,2500,5000,  Currency.USD, AcIds.A6ENB,  AirlineIds.EK,  SchIds.EK354_DXB_SIN,  FlightStatus.Scheduled);

            db.Flights.AddRange(flights);
            await db.SaveChangesAsync();
        }

        // ── Bookings ──────────────────────────────────────────────────────────────

        private static async Task SeedBookingsAsync(FlightReservationDbContext db)
        {
            if (await db.Bookings.AnyAsync()) return;

            var flights = await db.Flights
                .Include(f => f.Aircraft)
                    .ThenInclude(a => a!.Seats)
                .ToListAsync();

            var rng = new Random(42);
            var bookings = new List<Booking>();
            var bookedByFlight = new Dictionary<Guid, HashSet<Guid>>();
            int userIdx = 0;

            foreach (var flight in flights)
            {
                var seats = flight.Aircraft?.Seats?.ToList() ?? new List<Seat>();
                if (seats.Count == 0) continue;

                if (!bookedByFlight.ContainsKey(flight.Id))
                    bookedByFlight[flight.Id] = new HashSet<Guid>();

                var taken = bookedByFlight[flight.Id];
                var available = seats.Where(s => !taken.Contains(s.Id)).ToList();

                bool isPast = flight.FlightStatus == FlightStatus.Arrived;
                int count = Math.Min(rng.Next(3, 7), available.Count);

                for (int i = 0; i < count && available.Count > 0; i++)
                {
                    int idx = rng.Next(available.Count);
                    var seat = available[idx];
                    available.RemoveAt(idx);
                    taken.Add(seat.Id);

                    var status = isPast
                        ? (rng.Next(6) == 0 ? BookingStatus.Cancelled : BookingStatus.Completed)
                        : (rng.Next(5) == 0 ? BookingStatus.CheckedIn : BookingStatus.Confirmed);

                    var price = seat.SeatClass switch
                    {
                        SeatClass.Economy        => flight.BaseEconomyPrice,
                        SeatClass.PremiumEconomy => flight.BasePremiumEconomyPrice,
                        SeatClass.Business       => flight.BaseBusinessPrice,
                        SeatClass.First          => flight.BaseFirstClassPrice,
                        _                        => flight.BaseEconomyPrice
                    };

                    bookings.Add(new Booking
                    {
                        PnrNumber     = GeneratePnr(rng),
                        AppUserId     = UserIds.Ids[userIdx % 20],
                        FlightId      = flight.Id,
                        SeatId        = seat.Id,
                        TotalPrice    = price,
                        Currency      = flight.Currency,
                        BookingStatus = status,
                        CreatedBy     = "system"
                    });
                    userIdx++;
                }
            }

            db.Bookings.AddRange(bookings);
            await db.SaveChangesAsync();
        }

        private static string GeneratePnr(Random rng)
        {
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
            return new string(Enumerable.Range(0, 6).Select(_ => chars[rng.Next(chars.Length)]).ToArray());
        }

        // ── ID Constants ──────────────────────────────────────────────────────────

        private static class UserIds
        {
            public static readonly Guid[] Ids = Enumerable.Range(1, 20)
                .Select(i => new Guid($"80000000-0000-0000-0000-{i:D12}"))
                .ToArray();
        }

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
            // Turkey — original 10
            public static readonly Guid IST = new("40000000-0000-0000-0000-000000000001");
            public static readonly Guid SAW = new("40000000-0000-0000-0000-000000000002");
            public static readonly Guid ESB = new("40000000-0000-0000-0000-000000000003");
            public static readonly Guid ADB = new("40000000-0000-0000-0000-000000000004");
            public static readonly Guid AYT = new("40000000-0000-0000-0000-000000000005");
            public static readonly Guid BJV = new("40000000-0000-0000-0000-000000000006");
            public static readonly Guid DLM = new("40000000-0000-0000-0000-000000000007");
            public static readonly Guid GZT = new("40000000-0000-0000-0000-000000000008");
            public static readonly Guid TZX = new("40000000-0000-0000-0000-000000000009");
            public static readonly Guid SZF = new("40000000-0000-0000-0000-000000000010");
            // United Kingdom — original 3
            public static readonly Guid LHR = new("40000000-0000-0000-0000-000000000011");
            public static readonly Guid LGW = new("40000000-0000-0000-0000-000000000012");
            public static readonly Guid MAN = new("40000000-0000-0000-0000-000000000013");
            // UAE — original 2
            public static readonly Guid DXB = new("40000000-0000-0000-0000-000000000014");
            public static readonly Guid AUH = new("40000000-0000-0000-0000-000000000015");
            // USA — original 5
            public static readonly Guid JFK = new("40000000-0000-0000-0000-000000000016");
            public static readonly Guid LAX = new("40000000-0000-0000-0000-000000000017");
            public static readonly Guid ORD = new("40000000-0000-0000-0000-000000000018");
            public static readonly Guid MIA = new("40000000-0000-0000-0000-000000000019");
            public static readonly Guid BOS = new("40000000-0000-0000-0000-000000000020");
            // Germany — original 3
            public static readonly Guid FRA = new("40000000-0000-0000-0000-000000000021");
            public static readonly Guid MUC = new("40000000-0000-0000-0000-000000000022");
            public static readonly Guid BER = new("40000000-0000-0000-0000-000000000023");
            // France — original 2
            public static readonly Guid CDG = new("40000000-0000-0000-0000-000000000024");
            public static readonly Guid NCE = new("40000000-0000-0000-0000-000000000025");
            // Netherlands — original 1
            public static readonly Guid AMS = new("40000000-0000-0000-0000-000000000026");
            // Spain — original 2
            public static readonly Guid MAD = new("40000000-0000-0000-0000-000000000027");
            public static readonly Guid BCN = new("40000000-0000-0000-0000-000000000028");
            // Italy — original 2
            public static readonly Guid FCO = new("40000000-0000-0000-0000-000000000029");
            public static readonly Guid MXP = new("40000000-0000-0000-0000-000000000030");
            // Asia — original 3
            public static readonly Guid SIN = new("40000000-0000-0000-0000-000000000031");
            public static readonly Guid DOH = new("40000000-0000-0000-0000-000000000032");
            public static readonly Guid BKK = new("40000000-0000-0000-0000-000000000033");
            // Turkey — extended
            public static readonly Guid ADA = new("40000000-0000-0000-0000-000000000034");
            public static readonly Guid ADF = new("40000000-0000-0000-0000-000000000035");
            public static readonly Guid AJI = new("40000000-0000-0000-0000-000000000036");
            public static readonly Guid GZP = new("40000000-0000-0000-0000-000000000037");
            public static readonly Guid MZH = new("40000000-0000-0000-0000-000000000038");
            public static readonly Guid EDO = new("40000000-0000-0000-0000-000000000039");
            public static readonly Guid BAL = new("40000000-0000-0000-0000-000000000040");
            public static readonly Guid DNZ = new("40000000-0000-0000-0000-000000000041");
            public static readonly Guid DIY = new("40000000-0000-0000-0000-000000000042");
            public static readonly Guid EZS = new("40000000-0000-0000-0000-000000000043");
            public static readonly Guid ERC = new("40000000-0000-0000-0000-000000000044");
            public static readonly Guid ERZ = new("40000000-0000-0000-0000-000000000045");
            public static readonly Guid HTY = new("40000000-0000-0000-0000-000000000046");
            public static readonly Guid IGD = new("40000000-0000-0000-0000-000000000047");
            public static readonly Guid KCM = new("40000000-0000-0000-0000-000000000048");
            public static readonly Guid KSY = new("40000000-0000-0000-0000-000000000049");
            public static readonly Guid KFS = new("40000000-0000-0000-0000-000000000050");
            public static readonly Guid ASR = new("40000000-0000-0000-0000-000000000051");
            public static readonly Guid KYA = new("40000000-0000-0000-0000-000000000052");
            public static readonly Guid MLX = new("40000000-0000-0000-0000-000000000053");
            public static readonly Guid VAN = new("40000000-0000-0000-0000-000000000054");
            public static readonly Guid NVS = new("40000000-0000-0000-0000-000000000055");
            public static readonly Guid SIC = new("40000000-0000-0000-0000-000000000056");
            public static readonly Guid VAS = new("40000000-0000-0000-0000-000000000057");
            public static readonly Guid GNY = new("40000000-0000-0000-0000-000000000058");
            public static readonly Guid MUS = new("40000000-0000-0000-0000-000000000059");
            public static readonly Guid ECN = new("40000000-0000-0000-0000-000000000060");
            // United Kingdom — extended
            public static readonly Guid BHX = new("40000000-0000-0000-0000-000000000061");
            public static readonly Guid BRS = new("40000000-0000-0000-0000-000000000062");
            public static readonly Guid EDI = new("40000000-0000-0000-0000-000000000063");
            public static readonly Guid STN = new("40000000-0000-0000-0000-000000000064");
            // Germany — extended
            public static readonly Guid HAM = new("40000000-0000-0000-0000-000000000065");
            public static readonly Guid DUS = new("40000000-0000-0000-0000-000000000066");
            public static readonly Guid CGN = new("40000000-0000-0000-0000-000000000067");
            public static readonly Guid HAJ = new("40000000-0000-0000-0000-000000000068");
            public static readonly Guid STR = new("40000000-0000-0000-0000-000000000069");
            public static readonly Guid LEJ = new("40000000-0000-0000-0000-000000000070");
            public static readonly Guid DRS = new("40000000-0000-0000-0000-000000000071");
            public static readonly Guid DTM = new("40000000-0000-0000-0000-000000000072");
            public static readonly Guid BRE = new("40000000-0000-0000-0000-000000000073");
            public static readonly Guid ERF = new("40000000-0000-0000-0000-000000000074");
            // France — extended
            public static readonly Guid LYS = new("40000000-0000-0000-0000-000000000075");
            public static readonly Guid MRS = new("40000000-0000-0000-0000-000000000076");
            // Netherlands — extended
            public static readonly Guid EIN = new("40000000-0000-0000-0000-000000000077");
            // Spain — extended
            public static readonly Guid ALC = new("40000000-0000-0000-0000-000000000078");
            public static readonly Guid OVD = new("40000000-0000-0000-0000-000000000079");
            public static readonly Guid BIO = new("40000000-0000-0000-0000-000000000080");
            public static readonly Guid FUE = new("40000000-0000-0000-0000-000000000081");
            public static readonly Guid LPA = new("40000000-0000-0000-0000-000000000082");
            public static readonly Guid IBZ = new("40000000-0000-0000-0000-000000000083");
            public static readonly Guid ACE = new("40000000-0000-0000-0000-000000000084");
            public static readonly Guid LCG = new("40000000-0000-0000-0000-000000000085");
            public static readonly Guid PMI = new("40000000-0000-0000-0000-000000000086");
            public static readonly Guid AGP = new("40000000-0000-0000-0000-000000000087");
            // Italy — extended
            public static readonly Guid BLQ = new("40000000-0000-0000-0000-000000000088");
            public static readonly Guid VCE = new("40000000-0000-0000-0000-000000000089");
            public static readonly Guid NAP = new("40000000-0000-0000-0000-000000000090");
            // Greece
            public static readonly Guid ATH = new("40000000-0000-0000-0000-000000000091");
            public static readonly Guid SKG = new("40000000-0000-0000-0000-000000000092");
            // Belgium
            public static readonly Guid BRU = new("40000000-0000-0000-0000-000000000093");
            public static readonly Guid CRL = new("40000000-0000-0000-0000-000000000094");
            // Switzerland
            public static readonly Guid ZRH = new("40000000-0000-0000-0000-000000000095");
            public static readonly Guid GVA = new("40000000-0000-0000-0000-000000000096");
            public static readonly Guid BSL = new("40000000-0000-0000-0000-000000000097");
            // Austria
            public static readonly Guid VIE = new("40000000-0000-0000-0000-000000000098");
            public static readonly Guid GRZ = new("40000000-0000-0000-0000-000000000099");
            // Denmark
            public static readonly Guid CPH = new("40000000-0000-0000-0000-000000000100");
            public static readonly Guid AAL = new("40000000-0000-0000-0000-000000000101");
            public static readonly Guid AAR = new("40000000-0000-0000-0000-000000000102");
            public static readonly Guid BLL = new("40000000-0000-0000-0000-000000000103");
            // Norway
            public static readonly Guid OSL = new("40000000-0000-0000-0000-000000000104");
            // Sweden
            public static readonly Guid ARN = new("40000000-0000-0000-0000-000000000105");
            public static readonly Guid GOT = new("40000000-0000-0000-0000-000000000106");
            // Finland
            public static readonly Guid HEL = new("40000000-0000-0000-0000-000000000107");
            // Poland
            public static readonly Guid WAW = new("40000000-0000-0000-0000-000000000108");
            public static readonly Guid KRK = new("40000000-0000-0000-0000-000000000109");
            // Hungary
            public static readonly Guid BUD = new("40000000-0000-0000-0000-000000000110");
            // Romania
            public static readonly Guid OTP = new("40000000-0000-0000-0000-000000000111");
            // Serbia
            public static readonly Guid BEG = new("40000000-0000-0000-0000-000000000112");
            // Slovakia
            public static readonly Guid BTS = new("40000000-0000-0000-0000-000000000113");
            // Slovenia
            public static readonly Guid LJU = new("40000000-0000-0000-0000-000000000114");
            // Portugal
            public static readonly Guid LIS = new("40000000-0000-0000-0000-000000000115");
            public static readonly Guid OPO = new("40000000-0000-0000-0000-000000000116");
            // Ireland
            public static readonly Guid DUB = new("40000000-0000-0000-0000-000000000117");
            // Ukraine
            public static readonly Guid KBP = new("40000000-0000-0000-0000-000000000118");
            public static readonly Guid HRK = new("40000000-0000-0000-0000-000000000119");
            public static readonly Guid LWO = new("40000000-0000-0000-0000-000000000120");
            // Moldova
            public static readonly Guid KIV = new("40000000-0000-0000-0000-000000000121");
            // Russia
            public static readonly Guid SVO = new("40000000-0000-0000-0000-000000000122");
            public static readonly Guid LED = new("40000000-0000-0000-0000-000000000123");
            public static readonly Guid KRR = new("40000000-0000-0000-0000-000000000124");
            public static readonly Guid MCX = new("40000000-0000-0000-0000-000000000125");
            public static readonly Guid GRV = new("40000000-0000-0000-0000-000000000126");
            // Armenia
            public static readonly Guid EVN = new("40000000-0000-0000-0000-000000000127");
            // Georgia
            public static readonly Guid TBS = new("40000000-0000-0000-0000-000000000128");
            public static readonly Guid BUS = new("40000000-0000-0000-0000-000000000129");
            public static readonly Guid KUT = new("40000000-0000-0000-0000-000000000130");
            // Azerbaijan
            public static readonly Guid GYD = new("40000000-0000-0000-0000-000000000131");
            public static readonly Guid GNJ = new("40000000-0000-0000-0000-000000000132");
            // Kazakhstan
            public static readonly Guid ALA = new("40000000-0000-0000-0000-000000000133");
            public static readonly Guid NQZ = new("40000000-0000-0000-0000-000000000134");
            public static readonly Guid GUW = new("40000000-0000-0000-0000-000000000135");
            public static readonly Guid SCO = new("40000000-0000-0000-0000-000000000136");
            public static readonly Guid AKX = new("40000000-0000-0000-0000-000000000137");
            public static readonly Guid CIT = new("40000000-0000-0000-0000-000000000138");
            // Kyrgyzstan
            public static readonly Guid FRU = new("40000000-0000-0000-0000-000000000139");
            // Tajikistan
            public static readonly Guid DYU = new("40000000-0000-0000-0000-000000000140");
            // Kuwait
            public static readonly Guid KWI = new("40000000-0000-0000-0000-000000000141");
            // Bahrain
            public static readonly Guid BAH = new("40000000-0000-0000-0000-000000000142");
            // Saudi Arabia
            public static readonly Guid RUH = new("40000000-0000-0000-0000-000000000143");
            public static readonly Guid JED = new("40000000-0000-0000-0000-000000000144");
            public static readonly Guid DMM = new("40000000-0000-0000-0000-000000000145");
            // Iraq
            public static readonly Guid BGW = new("40000000-0000-0000-0000-000000000146");
            public static readonly Guid BSR = new("40000000-0000-0000-0000-000000000147");
            public static readonly Guid EBL = new("40000000-0000-0000-0000-000000000148");
            // Jordan
            public static readonly Guid AMM = new("40000000-0000-0000-0000-000000000149");
            // Lebanon
            public static readonly Guid BEY = new("40000000-0000-0000-0000-000000000150");
            // Syria
            public static readonly Guid DAM = new("40000000-0000-0000-0000-000000000151");
            public static readonly Guid ALP = new("40000000-0000-0000-0000-000000000152");
            // Iran
            public static readonly Guid IKA = new("40000000-0000-0000-0000-000000000153");
            public static readonly Guid IFN = new("40000000-0000-0000-0000-000000000154");
            // Pakistan
            public static readonly Guid KHI = new("40000000-0000-0000-0000-000000000155");
            // Egypt
            public static readonly Guid CAI = new("40000000-0000-0000-0000-000000000156");
            public static readonly Guid HRG = new("40000000-0000-0000-0000-000000000157");
            public static readonly Guid SSH = new("40000000-0000-0000-0000-000000000158");
            public static readonly Guid LXR = new("40000000-0000-0000-0000-000000000159");
            public static readonly Guid HBE = new("40000000-0000-0000-0000-000000000160");
            // Algeria
            public static readonly Guid ALG = new("40000000-0000-0000-0000-000000000161");
            // Morocco
            public static readonly Guid CMN = new("40000000-0000-0000-0000-000000000162");
            // Thailand — extended
            public static readonly Guid HKT = new("40000000-0000-0000-0000-000000000163");
            // Japan
            public static readonly Guid NRT = new("40000000-0000-0000-0000-000000000164");
            // China
            public static readonly Guid PEK = new("40000000-0000-0000-0000-000000000165");
            // India
            public static readonly Guid BOM = new("40000000-0000-0000-0000-000000000166");
            public static readonly Guid DEL = new("40000000-0000-0000-0000-000000000167");
            // Colombia
            public static readonly Guid BOG = new("40000000-0000-0000-0000-000000000168");
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
            public static readonly Guid TK1793_IST_ADB = new("60000000-0000-0000-0000-000000000004");
            public static readonly Guid TK1795_IST_ADB = new("60000000-0000-0000-0000-000000000005");
            public static readonly Guid TK2120_IST_AYT = new("60000000-0000-0000-0000-000000000007");
            public static readonly Guid TK2122_IST_AYT = new("60000000-0000-0000-0000-000000000008");
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
            // LH / BA
            public static readonly Guid LH400_FRA_JFK  = new("60000000-0000-0000-0000-000000000036");
            public static readonly Guid BA175_LHR_JFK  = new("60000000-0000-0000-0000-000000000037");
            public static readonly Guid BA177_LHR_JFK  = new("60000000-0000-0000-0000-000000000038");
        }

        private static class AcIds
        {
            public static readonly Guid TCLNA = new("70000000-0000-0000-0000-000000000001");
            public static readonly Guid TCKUE = new("70000000-0000-0000-0000-000000000002");
            public static readonly Guid TCJJK = new("70000000-0000-0000-0000-000000000003");
            public static readonly Guid TCLJT = new("70000000-0000-0000-0000-000000000004");
            public static readonly Guid TCJHM = new("70000000-0000-0000-0000-000000000005");
            public static readonly Guid A6EDB = new("70000000-0000-0000-0000-000000000006");
            public static readonly Guid A6EWC = new("70000000-0000-0000-0000-000000000007");
            public static readonly Guid A6EDM = new("70000000-0000-0000-0000-000000000008");
            public static readonly Guid A6ENB = new("70000000-0000-0000-0000-000000000009");
            public static readonly Guid YBBSA = new("70000000-0000-0000-0000-000000000010");
            public static readonly Guid YBBSB = new("70000000-0000-0000-0000-000000000011");
            public static readonly Guid YBBSC = new("70000000-0000-0000-0000-000000000012");
            public static readonly Guid DAIXA = new("70000000-0000-0000-0000-000000000013");
            public static readonly Guid DAIXB = new("70000000-0000-0000-0000-000000000014");
            public static readonly Guid DAIXC = new("70000000-0000-0000-0000-000000000015");
            public static readonly Guid GXWBA = new("70000000-0000-0000-0000-000000000016");
            public static readonly Guid GEUXA = new("70000000-0000-0000-0000-000000000017");
            public static readonly Guid GXWBB = new("70000000-0000-0000-0000-000000000018");
            public static readonly Guid FGZNX = new("70000000-0000-0000-0000-000000000019");
            public static readonly Guid FHPJH = new("70000000-0000-0000-0000-000000000020");
            public static readonly Guid FGKXA = new("70000000-0000-0000-0000-000000000021");
        }
    }
}
