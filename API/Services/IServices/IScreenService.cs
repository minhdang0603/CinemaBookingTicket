using API.DTOs.Request;
using API.DTOs.Response;

namespace API.Services.IServices
{
    public interface IScreenService
    {
        Task AddScreenAsync(ScreenCreateDTO dto);
        Task UpdateScreenAsync(int id, ScreenUpdateDTO dto);
        Task DeleteScreenAsync(int id);

        Task<ScreenDetailDTO> GetScreenByIdAsync(int id, bool? isActive = true);
        Task<List<ScreenDTO>> GetAllScreensAsync(bool? isActive = true);
        Task<List<ScreenDTO>> GetAllScreensWithPaginationAsync(int pageNumber, int pageSize, bool? isActive = true);
    }
}
