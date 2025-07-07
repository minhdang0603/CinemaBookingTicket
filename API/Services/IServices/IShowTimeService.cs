using API.DTOs.Request;
using API.DTOs.Response;

namespace API.Services.IServices
{
    public interface IShowTimeService
    {
        Task<List<ShowTimeDTO>> GetAllShowTimesAsync(bool? isActive = true);
        Task<ShowTimeBulkResultDTO> AddShowTimesAsync(List<ShowTimeCreateDTO> newShowTimes);
        Task<ShowTimeDTO> AddShowTimeAsync(ShowTimeCreateDTO dto);
        Task<ShowTimeDTO> UpdateShowTimeAsync(int id, ShowTimeUpdateDTO dto);
        Task<ShowTimeDTO> DeleteShowTimeAsync(int id);
        Task<List<ShowTimeDTO>> GetAllShowTimesWithPaginationAsync(int pageNumber, int pageSize, bool? isActive = true);
        Task<ShowTimeDTO> GetShowTimeByIdAsync(int id, bool? isActive = true);
        Task<ShowTimeSeatStatusDTO> GetShowTimeSeatStatusAsync(int showTimeId);
    }

}
