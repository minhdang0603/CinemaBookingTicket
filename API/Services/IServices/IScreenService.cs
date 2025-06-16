using API.DTOs.Request;

namespace API.Services.IServices
{
    public interface IScreenService
    {
        Task AddScreenAsync(ScreenCreateDTO dto);
        Task UpdateScreenAsync(int id, ScreenUpdateDTO dto);
        Task DeleteScreenAsync(int id);
    }
}
