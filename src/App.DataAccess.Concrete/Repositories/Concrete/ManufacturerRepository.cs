namespace App.DataAccess.Concrete.Repositories.Concrete
{
    public class ManufacturerRepository : GenericRepository<Manufacturer>, IManufacturerRepository
    {
        public ManufacturerRepository(FlightReservationDbContext db) : base(db) { }
    }
}
