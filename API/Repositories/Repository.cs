using System.Linq.Expressions;
using System.Linq;
using API.Data.Models;
using Microsoft.EntityFrameworkCore;
using API.Repositories.IRepositories;

namespace API.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        internal DbSet<T> dbSet;

        public Repository(ApplicationDbContext context)
        {
            _context = context;
            this.dbSet = _context.Set<T>();
        }

        public async Task CreateAsync(T entity)
        {
            await dbSet.AddAsync(entity);
        }

        // Refactored GetAsync
        public async Task<T?> GetAsync(
            Expression<Func<T, bool>>? filter = null,
            bool tracked = true,
            Func<IQueryable<T>, IQueryable<T>>? include = null)
        {
            IQueryable<T> query = dbSet;
            if (!tracked)
            {
                query = query.AsNoTracking();
            }
            if (filter != null)
            {
                query = query.Where(filter);
            }
            if (include != null)
            {
                query = include(query);
            }
            return await query.FirstOrDefaultAsync();
        }

        // Refactored GetAllAsync
        public async Task<List<T>> GetAllAsync(
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IQueryable<T>>? include = null,
            int pageSize = 0, int pageNumber = 1)
        {
            IQueryable<T> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }
            if (include != null)
            {
                query = include(query);
            }
            if (pageSize > 0)
            {
                if (pageSize > 100)
                {
                    pageSize = 100;
                }
                query = query.Skip(pageSize * (pageNumber - 1)).Take(pageSize);
            }
            return await query.ToListAsync();
        }

        public Task RemoveAsync(T entity)
        {
            dbSet.Remove(entity);
            return Task.CompletedTask;
        }

        public Task<T> UpdateAsync(T entity)
        {
            dbSet.Update(entity);
            return Task.FromResult(entity);
        }
    }
}
