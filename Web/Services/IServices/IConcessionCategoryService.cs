using Web.Models.DTOs.Request;

namespace Web.Services.IServices;

public interface IConcessionCategoryService
{
    Task<T> GetAllConcessionCategoriesAsync<T>();
    Task<T> GetConcessionCategoryByIdAsync<T>(int id);
    Task<T> CreateConcessionCategoryAsync<T>(ConcessionCategoryCreateDTO concessionCategoryCreateDTO, string token);
    Task<T> UpdateConcessionCategoryAsync<T>(ConcessionCategoryUpdateDTO concessionCategoryUpdateDTO, string token);
    Task<T> DeleteConcessionCategoryAsync<T>(int id, string token);
}