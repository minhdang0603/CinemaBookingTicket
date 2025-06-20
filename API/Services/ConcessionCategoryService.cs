using API.Data.Models;
using API.DTOs.Request;
using API.DTOs.Response;
using API.Exceptions;
using API.Repositories.IRepositories;
using API.Services.IServices;
using AutoMapper;

namespace API.Services.Services;

public class ConcessionCategoryService : IConcessionCategoryService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<MovieService> _logger;

    public ConcessionCategoryService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<MovieService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }
    public async Task<ConcessionCategoryDTO> CreateConcessionCategoryAsync(ConcessionCategoryCreateDTO concessionCategory)
    {
        if (concessionCategory == null)
        {
            _logger.LogError("ConcessionCategoryCreateDTO is null");
            throw new ArgumentNullException(nameof(concessionCategory));
        }
        var categoryExists = _unitOfWork.ConcessionCategory.GetAsync(
            c => c.Name.Equals(concessionCategory.Name, StringComparison.OrdinalIgnoreCase)).Result;
        if (categoryExists != null)
        {
            _logger.LogError($"Concession category with name {concessionCategory.Name} already exists.");
            throw new AppException(ErrorCodes.ConcessionCategoryAlreadyExists(concessionCategory.Name));
        }
        var category = _mapper.Map<ConcessionCategory>(concessionCategory);
        await _unitOfWork.ConcessionCategory.CreateAsync(category);
        await _unitOfWork.SaveAsync();
        _logger.LogInformation($"Concession category {category.Name} created successfully.");
        return _mapper.Map<ConcessionCategoryDTO>(category);
    }

    public async Task<ConcessionCategoryDTO> DeleteConcessionCategoryAsync(int id)
    {
        var category = await _unitOfWork.ConcessionCategory.GetAsync(c => c.Id == id);
        if (category == null)
        {
            _logger.LogError($"Concession category with id {id} not found.");
            throw new AppException(ErrorCodes.ConcessionCategoryNotFound(id));
        }
        category.IsActive = false; // Soft delete
        await _unitOfWork.ConcessionCategory.UpdateAsync(category);
        await _unitOfWork.SaveAsync();
        _logger.LogInformation($"Concession category {category.Name} deleted successfully.");
        return _mapper.Map<ConcessionCategoryDTO>(category);
    }

    public async Task<List<ConcessionCategoryDTO>> GetAllConcessionCategoriesAsync()
    {
        var categories = await _unitOfWork.ConcessionCategory.GetAllAsync(c => c.IsActive);
        if (categories == null || !categories.Any())
        {
            _logger.LogWarning("No active concession categories found.");
            return new List<ConcessionCategoryDTO>();
        }
        _logger.LogInformation($"{categories.Count} active concession categories retrieved successfully.");
        return _mapper.Map<List<ConcessionCategoryDTO>>(categories);
    }

    public async Task<ConcessionCategoryDTO> GetConcessionCategoryByIdAsync(int id)
    {
        var category = await _unitOfWork.ConcessionCategory.GetAsync(c => c.Id == id && c.IsActive);
        if (category == null)
        {
            _logger.LogError($"Concession category with id {id} not found.");
            throw new AppException(ErrorCodes.ConcessionCategoryNotFound(id));
        }
        _logger.LogInformation($"Concession category with id {id} retrieved successfully.");
        return _mapper.Map<ConcessionCategoryDTO>(category);
    }

    public async Task<ConcessionCategoryDTO> UpdateConcessionCategoryAsync(int id, ConcessionCategoryUpdateDTO concessionCategory)
    {
        if (concessionCategory == null)
        {
            _logger.LogError("ConcessionCategoryUpdateDTO is null");
            throw new ArgumentNullException(nameof(concessionCategory));
        }
        var category = await _unitOfWork.ConcessionCategory.GetAsync(c => c.Id == id && c.IsActive);
        if (category == null)
        {
            _logger.LogError($"Concession category with id {id} not found.");
            throw new AppException(ErrorCodes.ConcessionCategoryNotFound(id));
        }
        var categoryExists = await _unitOfWork.ConcessionCategory.GetAsync(
            c => c.Name.Equals(concessionCategory.Name, StringComparison.OrdinalIgnoreCase) && c.Id != id);
        if (categoryExists != null)
        {
            _logger.LogError($"Concession category with name {concessionCategory.Name} already exists.");
            throw new AppException(ErrorCodes.ConcessionCategoryAlreadyExists(concessionCategory.Name));
        }
        _mapper.Map(concessionCategory, category);
        await _unitOfWork.ConcessionCategory.UpdateAsync(category);
        await _unitOfWork.SaveAsync();
        _logger.LogInformation($"Concession category {category.Name} updated successfully.");
        return _mapper.Map<ConcessionCategoryDTO>(category);
    }
}
