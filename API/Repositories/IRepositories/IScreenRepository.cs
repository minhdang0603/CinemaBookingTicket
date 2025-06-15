using API.Data.Models;
using API.Repositories.IRepositories;

namespace API.Repositories.IRepositories
{
    public interface IScreenRepository
    {
        Task AddAsync(Screen entity);
        void Update(Screen entity);
        void Remove(Screen entity);

        Task<Screen?> GetAsync(int id);
    }
}
