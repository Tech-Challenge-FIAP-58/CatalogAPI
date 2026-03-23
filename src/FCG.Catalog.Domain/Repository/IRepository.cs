using FCG.Catalog.Domain.Common;

namespace FCG.Catalog.Domain.Repository
{
    public interface IRepository<TEntity> where TEntity : EntityBase, IAggregateRoot
    {
        Task<IEnumerable<TEntity>> GetAll();
        Task<TEntity?> GetById(Guid id);
        void Add(TEntity entity);
        void Update(TEntity entity);
        void Delete(TEntity entity);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
