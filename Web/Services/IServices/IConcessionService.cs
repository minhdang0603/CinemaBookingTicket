
using Web.Models.DTOs.Request;

namespace Web.Services.IServices;

public interface IConcessionService
{
    Task<T> GetAllConcessionsAsync<T>();
    Task<T> GetConcessionByIdAsync<T>(int id);
    Task<T> CreateConcessionAsync<T>(ConcessionCreateDTO concessionCreateDTO, string token);
    Task<T> UpdateConcessionAsync<T>(ConcessionUpdateDTO concessionUpdateDTO, string token);
    Task<T> DeleteConcessionAsync<T>(int id, string token);
}