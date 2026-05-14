namespace App.DataAccess.Context
{
    public class FlightReservationDbContext : IdentityDbContext<IdentityUser, IdentityRole, string>
    {
        public FlightReservationDbContext(DbContextOptions<FlightReservationDbContext> options) : base(options) { }

        public virtual DbSet<Admin> Admins { get; set; } = null!;
        public virtual DbSet<Aircraft> Aircrafts { get; set; } = null!;
        public virtual DbSet<Airline> Airlines { get; set; } = null!;
        public virtual DbSet<Airport> Airports { get; set; } = null!;
        public virtual DbSet<AppUser> AppUsers { get; set; } = null!;
        public virtual DbSet<Booking> Bookings { get; set; } = null!;
        public virtual DbSet<Flight> Flights { get; set; } = null!;
        public virtual DbSet<Manufacturer> Manufacturers { get; set; } = null!;
        public virtual DbSet<Model> Models { get; set; } = null!;
        public virtual DbSet<Route> Routes { get; set; } = null!;
        public virtual DbSet<Schedule> Schedules { get; set; } = null!;
        public virtual DbSet<Seat> Seats { get; set; } = null!;
        public virtual DbSet<VerificationCode> VerificationCodes { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(typeof(IEntityConfiguration).Assembly);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            SetAuditProperties();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void SetAuditProperties()
        {
            var entries = ChangeTracker.Entries<App.Core.Entities.Abstract.AuditableBaseEntity>();
            foreach (var entry in entries)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.EntityStatus = EntityStatus.Added;
                        entry.Entity.CreatedDate = DateTime.UtcNow;
                        if (string.IsNullOrEmpty(entry.Entity.CreatedBy))
                            entry.Entity.CreatedBy = "system";
                        break;
                    case EntityState.Modified:
                        entry.Entity.EntityStatus = EntityStatus.Updated;
                        entry.Entity.ModifiedDate = DateTime.UtcNow;
                        break;
                    case EntityState.Deleted:
                        entry.State = EntityState.Modified;
                        entry.Entity.EntityStatus = EntityStatus.Deleted;
                        entry.Entity.DeletedDate = DateTime.UtcNow;
                        break;
                }
            }
        }
    }
}
