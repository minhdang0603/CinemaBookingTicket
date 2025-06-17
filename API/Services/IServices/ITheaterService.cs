using API.DTOs.Request;
using API.DTOs.Response;

namespace API.Services.IServices
{
    public interface ITheaterService
    {
        Task AddTheaterAsync(TheaterCreateDTO dto);
        Task UpdateTheaterAsync(int id, TheaterUpdateDTO dto);
        Task DeleteTheaterAsync(int id);

        Task<List<TheaterDTO>> GetAllTheatersAsync(bool? isActive = true);
        Task<TheaterDetailDTO> GetTheaterByIdAsync(int id, bool? isActive = true);
        Task<List<TheaterDTO>> GetAllTheatersWithPaginationAsync(int pageNumber, int pageSize, bool? isActive = true);
    }
}
