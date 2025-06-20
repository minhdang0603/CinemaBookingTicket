using API.DTOs.Request;
using API.DTOs.Response;

namespace API.Services.IServices;

public interface IConcessionService
{
    Task<List<ConcessionDTO>> GetAllConcessionsAsync();
    Task<List<ConcessionDTO>> GetAllConcessionsWithPaginationAsync(int pageNumber, int pageSize, bool? isActive = true);
    Task<ConcessionDTO> CreateConcessionAsync(ConcessionCreateDTO concessionCreateDTO);
    Task<ConcessionDTO> UpdateConcessionAsync(int id, ConcessionUpdateDTO concessionUpdateDTO);
    Task<ConcessionDTO> DeleteConcessionAsync(int id);
    Task<ConcessionDTO> GetConcessionByIdAsync(int id);
    Task<List<ConcessionDTO>> GetConcessionsByCategoryIdAsync(int categoryId);
    Task<List<ConcessionDTO>> SearchConcessionsByNameAsync(string name);
}