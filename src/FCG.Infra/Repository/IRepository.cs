using FCG.Core.Core.Models;

namespace FCG.Infra.Repository
{
    public interface IRepository<T> where T : Entity
    {
        Task<IEnumerable<T>> Get();
        Task<T?> Get(Guid id);
        Task<bool> Register(T entity);
        Task<bool> Edit(T entity);
        Task<bool> Delete(Guid id);
    }
}
