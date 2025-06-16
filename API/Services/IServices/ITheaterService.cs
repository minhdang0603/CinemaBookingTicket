using API.DTOs.Request;
using API.DTOs.Response;

namespace API.Services.IServices
{
    public interface ITheaterService
    {
        Task AddTheaterAsync(TheaterCreateDTO dto);
        Task UpdateTheaterAsync(int id, TheaterUpdateDTO dto);
        Task DeleteTheaterAsync(int id);
    }
}
