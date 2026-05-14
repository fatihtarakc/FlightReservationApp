namespace App.DataAccess.Concrete.Repositories.Concrete
{
    public class ModelRepository : GenericRepository<Model>, IModelRepository
    {
        public ModelRepository(FlightReservationDbContext db) : base(db) { }

        public async Task<IEnumerable<Model>> GetByManufacturerIdAsync(Guid manufacturerId, bool tracking = true) =>
            await GetAllByStatusIsNotDeletedByTracking(tracking)
                .Where(m => m.ManufacturerId == manufacturerId)
                .ToListAsync();
    }
}
