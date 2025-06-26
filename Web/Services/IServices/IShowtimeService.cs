using Web.Models.DTOs.Request;

namespace Web.Services.IServices
{
    public interface IShowtimeService
    {
        Task<T> GetAllShowTimesAsync<T>();
        Task<T> AddShowTimesAsync<T>(List<ShowTimeCreateDTO> newShowTimes, string? token = null);
        Task<T> AddShowTimeAsync<T>(ShowTimeCreateDTO dto, string? token = null);
        Task<T> GetShowTimeByIdAsync<T>(int showTimeId);
        Task<T> UpdateShowTimeAsync<T>(int showTimeId, ShowTimeUpdateDTO updatedShowTime, string? token = null);
        Task<T> DeleteShowTimeAsync<T>(int showTimeId, string? token = null);

    }
}
