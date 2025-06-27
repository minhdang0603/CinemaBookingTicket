using API.DTOs.Request;
using API.DTOs.Response;

namespace API.Services.IServices;

public interface IConcessionService
{
    Task<List<ConcessionDTO>> GetAllConcessionsAsync();
    Task<ConcessionDTO> GetConcessionByIdAsync(int id);
    Task<ConcessionDTO> CreateConcessionAsync(ConcessionCreateDTO concessionCreateDTO);
    Task<ConcessionDTO> UpdateConcessionAsync(int id, ConcessionUpdateDTO concessionUpdateDTO);
    Task<ConcessionDTO> DeleteConcessionAsync(int id);
    Task<List<ConcessionDTO>> GetAllConcessionsByCategoryIdAsync(int categoryId);
}