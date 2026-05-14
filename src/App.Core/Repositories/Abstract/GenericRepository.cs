namespace App.Core.Repositories.Abstract
{
    public abstract class GenericRepository<Entity> :
        IAsyncAddableRepository<Entity>, IAsyncDeletableRepository<Entity>, IAsyncUpdatableRepository<Entity>,
        IAsyncQueryableRepository<Entity>, IAsyncOrderableRepository<Entity> where Entity : AuditableBaseEntity
    {
        protected readonly DbSet<Entity> dbEntity;
        private readonly IdentityDbContext<IdentityUser, IdentityRole, string> db;

        protected GenericRepository(IdentityDbContext<IdentityUser, IdentityRole, string> db)
        {
            this.db = db;
            dbEntity = this.db.Set<Entity>();
        }

        protected IQueryable<Entity> GetAllByStatusIsNotDeletedByTracking(bool tracking = true) =>
            tracking
                ? dbEntity.Where(e => e.EntityStatus != EntityStatus.Deleted)
                : dbEntity.Where(e => e.EntityStatus != EntityStatus.Deleted).AsNoTracking();

        public async ValueTask<Entity> AddAsync(Entity entity) =>
            (await dbEntity.AddAsync(entity)).Entity;

        public async Task AddRangeAsync(IEnumerable<Entity> entities) =>
            await dbEntity.AddRangeAsync(entities);

        public async ValueTask DeleteAsync(Entity entity)
        {
            entity.EntityStatus = EntityStatus.Deleted;
            entity.DeletedDate = DateTime.UtcNow;
            await Task.FromResult(dbEntity.Update(entity));
        }

        public async ValueTask<Entity> UpdateAsync(Entity entity) =>
            (await Task.FromResult(dbEntity.Update(entity))).Entity;

        public async Task<bool> AnyAsync(Expression<Func<Entity, bool>> expression = null!) =>
            expression is null
                ? await GetAllByStatusIsNotDeletedByTracking().AnyAsync()
                : await GetAllByStatusIsNotDeletedByTracking().AnyAsync(expression);

        public async Task<Entity> GetByIdAsync(Guid entityId, bool tracking = true) =>
            await GetAllByStatusIsNotDeletedByTracking(tracking).FirstOrDefaultAsync(e => e.Id == entityId);

        public async Task<Entity> GetFirstOrDefaultAsync(Expression<Func<Entity, bool>> expression, bool tracking = true) =>
            await GetAllByStatusIsNotDeletedByTracking(tracking).FirstOrDefaultAsync(expression);

        public async Task<IEnumerable<Entity>> GetAllWhereAsync(Expression<Func<Entity, bool>> expression, bool tracking = true) =>
            await GetAllByStatusIsNotDeletedByTracking(tracking).Where(expression).ToListAsync();

        public async Task<IEnumerable<Entity>> GetAllAsync(bool tracking = true) =>
            await GetAllByStatusIsNotDeletedByTracking(tracking).ToListAsync();

        public async Task<IEnumerable<Entity>> GetAllWhereAsync<TKey>(Expression<Func<Entity, TKey>> orderby, bool orderDesc = false, bool tracking = true) =>
            orderDesc
                ? await GetAllByStatusIsNotDeletedByTracking(tracking).OrderByDescending(orderby).ToListAsync()
                : await GetAllByStatusIsNotDeletedByTracking(tracking).OrderBy(orderby).ToListAsync();

        public async Task<IEnumerable<Entity>> GetAllWhereAsync<TKey>(Expression<Func<Entity, bool>> expression, Expression<Func<Entity, TKey>> orderby, bool orderDesc = false, bool tracking = true) =>
            orderDesc
                ? await GetAllByStatusIsNotDeletedByTracking(tracking).Where(expression).OrderByDescending(orderby).ToListAsync()
                : await GetAllByStatusIsNotDeletedByTracking(tracking).Where(expression).OrderBy(orderby).ToListAsync();
    }
}
