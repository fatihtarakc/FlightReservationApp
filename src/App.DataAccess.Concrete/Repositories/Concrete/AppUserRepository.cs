namespace App.DataAccess.Concrete.Repositories.Concrete
{
    public class AppUserRepository : GenericRepository<AppUser>, IAppUserRepository
    {
        public AppUserRepository(FlightReservationDbContext db) : base(db) { }

        public async Task<AppUser> GetByIdentityIdAsync(string identityId, bool tracking = true) =>
            await GetAllByStatusIsNotDeletedByTracking(tracking)
                .FirstOrDefaultAsync(u => u.IdentityId == identityId);

        public async Task<AppUser> GetByEmailAsync(string email, bool tracking = true) =>
            await GetAllByStatusIsNotDeletedByTracking(tracking)
                .FirstOrDefaultAsync(u => u.Email == email);
    }
}
