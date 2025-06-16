using API.DTOs.Request;
using API.DTOs.Response;
using API.Repositories.IRepositories;
using API.Services.IServices;

namespace API.Services
{
    public class ShowTimeService : IShowTimeService
    {
        private readonly IShowTimeRepository _repository;

        public ShowTimeService(IShowTimeRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<ShowTimeDTO>> GetAllShowTimesAsync()
        {
            return await _repository.GetAllShowTimesAsync();
        }

        public async Task<(List<ShowTimeDTO> Added, List<string> Errors)> AddShowTimesAsync(List<ShowTimeCreateDTO> newShowTimes)
        {
            return await _repository.AddShowTimesAsync(newShowTimes);
        }

        public async Task<(ShowTimeDTO? Updated, string? Error)> UpdateShowTimeAsync(int id, ShowTimeUpdateDTO dto)
        {
            return await _repository.UpdateShowTimeAsync(id, dto);
        }

        public async Task<bool> DeleteShowTimeAsync(int id)
        {
            return await _repository.DeleteShowTimeAsync(id);
        }
    }
}
