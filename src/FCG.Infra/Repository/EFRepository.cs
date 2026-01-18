using FCG.Core.Core.Models;
using FCG.Infra.Context;
using Microsoft.EntityFrameworkCore;

namespace FCG.Infra.Repository
{
    public class EFRepository<T> : IRepository<T> where T : Entity
    {
        protected ApplicationDbContext _context;
        protected DbSet<T> _dbSet;

        public EFRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task<bool> Edit(T entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Register(T entity)
        {
            _dbSet.Add(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Delete(Guid id)
        {
            var entity = await Get(id) ?? throw new ArgumentNullException(nameof(id), $"Erro ao deletar: Entidade inexistente!");
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<T?> Get(Guid id) => await _dbSet.FirstOrDefaultAsync(entity => entity.Id == id);

        public async Task<IEnumerable<T>> Get() => await _dbSet.ToListAsync();
    }
}
