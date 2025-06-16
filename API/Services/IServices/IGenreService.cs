using API.DTOs.Request;
using API.DTOs.Response;

namespace API.Services.IServices
{
    public interface IGenreService
    {
        Task<List<GenreDTO>> GetAllGenresAsync(bool? isActive = true);
        Task<List<GenreDTO>> GetAllGenresWithPaginationAsync(int pageNumber, int pageSize, bool? isActive = true);
        Task<List<GenreDTO>> SearchGenresAsync(string searchTerm, bool? isActive = true);
        Task<GenreDTO> GetGenreByIdAsync(int id, bool? isActive = true);
        Task<GenreDTO> CreateGenreAsync(GenreCreateDTO genreCreateDTO);
        Task<GenreDTO> UpdateGenreAsync(int id, GenreUpdateDTO genreUpdateDTO);
        Task<GenreDTO> DeleteGenreAsync(int id);
    }
}
