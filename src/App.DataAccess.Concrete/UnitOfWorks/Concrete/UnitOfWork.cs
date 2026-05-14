using Microsoft.EntityFrameworkCore.Storage;

namespace App.DataAccess.Concrete.UnitOfWorks.Concrete
{
    public class UnitOfWork : IUnitOfWork
    {
        private bool _disposed = false;
        private readonly FlightReservationDbContext _db;

        public UnitOfWork(FlightReservationDbContext db) => _db = db;

        public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default) =>
            _db.Database.BeginTransactionAsync(cancellationToken);

        public async Task<IExecutionStrategy> CreateExecutionStrategy() =>
            await Task.FromResult(_db.Database.CreateExecutionStrategy());

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
            await _db.SaveChangesAsync(cancellationToken);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing) _db.Dispose();
                _disposed = true;
            }
        }

        public async ValueTask DisposeAsync()
        {
            await DisposeAsync(true);
            GC.SuppressFinalize(this);
        }

        protected virtual async ValueTask DisposeAsync(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing) await _db.DisposeAsync();
                _disposed = true;
            }
        }
    }
}
