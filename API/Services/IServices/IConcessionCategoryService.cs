using API.Data.Models;
using API.DTOs.Request;
using API.DTOs.Response;

namespace API.Services.IServices;

public interface IConcessionCategoryService
{
    Task<List<ConcessionCategoryDTO>> GetAllConcessionCategoriesAsync();
    Task<ConcessionCategoryDTO> GetConcessionCategoryByIdAsync(int id);
    Task<ConcessionCategoryDTO> CreateConcessionCategoryAsync(ConcessionCategoryCreateDTO concessionCategory);
    Task<ConcessionCategoryDTO> UpdateConcessionCategoryAsync(int id, ConcessionCategoryUpdateDTO concessionCategory);
    Task<ConcessionCategoryDTO> DeleteConcessionCategoryAsync(int id);
}