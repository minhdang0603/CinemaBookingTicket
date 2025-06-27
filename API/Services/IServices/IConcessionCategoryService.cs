using API.DTOs.Request;
using API.DTOs.Response;

namespace API.Services.IServices;

public interface IConcessionCategoryService
{
    Task<ConcessionCategoryDTO> CreateConcessionCategoryAsync(ConcessionCategoryCreateDTO concessionCategoryCreateDTO);
    Task<ConcessionCategoryDTO> UpdateConcessionCategoryAsync(int id, ConcessionCategoryUpdateDTO concessionCategoryUpdateDTO);
    Task<ConcessionCategoryDTO> DeleteConcessionCategoryAsync(int id);
    Task<ConcessionCategoryDTO> GetConcessionCategoryByIdAsync(int id);
    Task<IEnumerable<ConcessionCategoryDTO>> GetAllConcessionCategoriesAsync();
}