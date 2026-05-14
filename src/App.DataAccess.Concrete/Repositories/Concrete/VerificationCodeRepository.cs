namespace App.DataAccess.Concrete.Repositories.Concrete
{
    public class VerificationCodeRepository : GenericRepository<VerificationCode>, IVerificationCodeRepository
    {
        public VerificationCodeRepository(FlightReservationDbContext db) : base(db) { }

        public async Task<VerificationCode> GetActiveCodeAsync(Guid userId, VerificationCodePurpose purpose, bool tracking = true) =>
            await GetAllByStatusIsNotDeletedByTracking(tracking)
                .Where(v =>
                    v.AppUserId == userId &&
                    v.Purpose == purpose &&
                    v.Status == VerificationCodeStatus.Active &&
                    v.ExpiresAt > DateTime.UtcNow)
                .OrderByDescending(v => v.CreatedDate)
                .FirstOrDefaultAsync();
    }
}
