using Web.Models.DTOs.Request;

namespace Web.Services.IServices;

public interface IGenreService
{
    Task<T> GetAllGenresAsync<T>();
    Task<T> GetGenreByIdAsync<T>(int id);
    Task<T> CreateGenreAsync<T>(GenreCreateDTO genre, string? token = null);
    Task<T> UpdateGenreAsync<T>(GenreUpdateDTO genre, string? token = null);
    Task<T> DeleteGenreAsync<T>(int id, string? token = null);
    Task<T> GetFiveTopGenresAsync<T>();
}