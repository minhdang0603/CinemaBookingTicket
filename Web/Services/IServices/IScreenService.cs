using System;
using Web.Models.DTOs.Request;
using Web.Models.DTOs.Response;

namespace Web.Services.IServices;

public interface IScreenService
{
    Task<T> GetAllScreensAsync<T>();
    Task<List<T>> GetAllScreensWithPaginationAsync<T>(int pageNumber, int pageSize);
    Task<T> GetScreenByIdAsync<T>(int id);
    Task<T> CreateScreenAsync<T>(ScreenCreateDTO screen, string? token = null);
    Task<T> UpdateScreenAsync<T>(ScreenUpdateDTO screen, string? token = null);
    Task<T> DeleteScreenAsync<T>(int id, string? token = null);
}
