using API.Data.Models;
using API.Repositories.IRepositories;

namespace API.Repositories.IRepositories
{
    public interface ITheaterRepository
    {
        Task AddAsync(Theater entity);
        void Update(Theater entity);
        void Remove(Theater entity);

        Task<Theater?> GetAsync(int id);

    }

}