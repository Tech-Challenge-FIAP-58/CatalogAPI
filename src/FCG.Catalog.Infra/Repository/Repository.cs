using System.Linq;
using Microsoft.EntityFrameworkCore;
using FCG.Catalog.Domain.Events;
using FCG.Catalog.Domain.Common;
using FCG.Catalog.Infra.Context;

namespace FCG.Catalog.Infra.Repository
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : EntityBase, IAggregateRoot
    {
        protected ApplicationDbContext _context;
        protected DbSet<TEntity> _dbSet;

        public Repository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<TEntity>();
        }

        public async Task<IEnumerable<TEntity>> GetAll() => await _dbSet.ToListAsync();

        public async Task<TEntity?> GetById(Guid id) => await _dbSet.FirstOrDefaultAsync(entity => entity.Id == id);

        public void Update(TEntity entity)
        {
            AddDomainEventIfMissing(entity, DomainEventAction.Updated);
            _dbSet.Update(entity);
        }

        public void Add(TEntity entity)
        {
            AddDomainEventIfMissing(entity, DomainEventAction.Created);
            _dbSet.Add(entity);
        }

        public void Delete(TEntity entity)
        {
            AddDomainEventIfMissing(entity, DomainEventAction.Deleted);
            _dbSet.Remove(entity);
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return _context.SaveChangesAsync(cancellationToken);
        }

        private static void AddDomainEventIfMissing(TEntity entity, DomainEventAction action)
        {
            var domainEvent = entity.CreateDomainEvent(action);

            if (entity.Notifications?.Any(existing => existing.GetType() == domainEvent.GetType()) == true)
            {
                return;
            }

            entity.AddEvent(domainEvent);
        }
    }
}
