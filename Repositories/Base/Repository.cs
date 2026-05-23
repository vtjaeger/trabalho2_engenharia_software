using System.Linq.Expressions;
using trabalho2.Data;
using Microsoft.EntityFrameworkCore;

namespace trabalho2.Repositories.Interfaces
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;

        public Repository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<T?> GetById(string id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public async Task<List<T>> GetAll()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public async Task<List<T>> Find(Expression<Func<T, bool>> predicate)
        {
            return await _context.Set<T>()
                .Where(predicate)
                .ToListAsync();
        }

        public async Task<T> Create(T entity)
        {
            _context.Set<T>().Add(entity);

            await _context.SaveChangesAsync();

            return entity;
        }

        public async Task<T> Update(T entity)
        {
            _context.Set<T>().Update(entity);

            await _context.SaveChangesAsync();

            return entity;
        }

        public async Task<bool> Delete(string id)
        {
            var entity = await GetById(id);

            if (entity == null)
                return false;

            _context.Set<T>().Remove(entity);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}