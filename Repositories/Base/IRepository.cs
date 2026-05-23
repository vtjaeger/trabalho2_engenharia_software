using System.Linq.Expressions;

namespace trabalho2.Repositories.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<T?> GetById(string id);

        Task<List<T>> GetAll();

        Task<List<T>> Find(Expression<Func<T, bool>> predicate);

        Task<T> Create(T entity);

        Task<T> Update(T entity);

        Task<bool> Delete(string id);
    }
}