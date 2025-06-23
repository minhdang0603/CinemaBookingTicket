using System.Linq.Expressions;

namespace API.Repositories.IRepositories
{
    public interface IRepository<T> where T : class
    {
        Task<List<T>> GetAllAsync(
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IQueryable<T>>? include = null,
            int pageSize = 0, int pageNumber = 1);
        Task<T?> GetAsync(
            Expression<Func<T, bool>>? filter = null,
            bool tracked = true,
            Func<IQueryable<T>, IQueryable<T>>? include = null);
        Task CreateAsync(T entity);
        Task RemoveAsync(T entity);
        Task<T> UpdateAsync(T entity);
    }
}
