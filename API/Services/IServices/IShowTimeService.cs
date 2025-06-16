using API.DTOs.Request;
using API.DTOs.Response;

namespace API.Services.IServices
{
    public interface IShowTimeService
    {
        Task<List<ShowTimeDTO>> GetAllShowTimesAsync();
        Task<(List<ShowTimeDTO> Added, List<string> Errors)> AddShowTimesAsync(List<ShowTimeCreateDTO> newShowTimes);
        Task<(ShowTimeDTO? Updated, string? Error)> UpdateShowTimeAsync(int id, ShowTimeUpdateDTO dto);
        Task<bool> DeleteShowTimeAsync(int id);

    }

}
