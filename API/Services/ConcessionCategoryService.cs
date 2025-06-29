using API.Data.Models;
using API.DTOs.Request;
using API.DTOs.Response;
using API.Exceptions;
using API.Repositories.IRepositories;
using API.Services.IServices;
using AutoMapper;

namespace API.Services;

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

    public async Task<ConcessionCategoryDTO> CreateConcessionCategoryAsync(ConcessionCategoryCreateDTO concessionCategoryCreateDTO)
    {
        if (concessionCategoryCreateDTO == null)
        {
            _logger.LogError("ConcessionCategoryCreateDTO is null");
            throw new ArgumentNullException(nameof(concessionCategoryCreateDTO));
        }

        var existingCategory = await _unitOfWork.ConcessionCategory.GetAsync(c => c.Name == concessionCategoryCreateDTO.Name);
        if (existingCategory != null)
        {
            _logger.LogError($"Concession Category with Name {concessionCategoryCreateDTO.Name} already exists");
            throw new AppException(ErrorCodes.ConcessionCategoryAlreadyExists(concessionCategoryCreateDTO.Name));
        }

        var concessionCategory = _mapper.Map<ConcessionCategory>(concessionCategoryCreateDTO);
        await _unitOfWork.ConcessionCategory.CreateAsync(concessionCategory);
        await _unitOfWork.SaveAsync();

        _logger.LogInformation($"Concession Category {concessionCategory.Name} created successfully with ID {concessionCategory.Id}");
        return _mapper.Map<ConcessionCategoryDTO>(concessionCategory);
    }
    public async Task<ConcessionCategoryDTO> UpdateConcessionCategoryAsync(int id, ConcessionCategoryUpdateDTO concessionCategoryUpdateDTO)
    {
        if (concessionCategoryUpdateDTO == null)
        {
            _logger.LogError("ConcessionCategoryUpdateDTO is null");
            throw new ArgumentNullException(nameof(concessionCategoryUpdateDTO));
        }

        var concessionCategory = await _unitOfWork.ConcessionCategory.GetAsync(c => c.Id == id && c.IsActive);
        if (concessionCategory == null)
        {
            _logger.LogError($"Concession Category with ID {id} not found");
            throw new AppException(ErrorCodes.ConcessionCategoryNotFound(id));
        }

        _mapper.Map(concessionCategoryUpdateDTO, concessionCategory);

        await _unitOfWork.ConcessionCategory.UpdateAsync(concessionCategory);
        await _unitOfWork.SaveAsync();

        _logger.LogInformation($"Concession Category {concessionCategory.Name} updated successfully with ID {concessionCategory.Id}");
        return _mapper.Map<ConcessionCategoryDTO>(concessionCategory);
    }
    public async Task<ConcessionCategoryDTO> DeleteConcessionCategoryAsync(int id)
    {
        var concessionCategory = await _unitOfWork.ConcessionCategory.GetAsync(c => c.Id == id && c.IsActive);
        if (concessionCategory == null)
        {
            _logger.LogError($"Concession Category with ID {id} not found");
            throw new AppException(ErrorCodes.ConcessionCategoryNotFound(id));
        }

        concessionCategory.IsActive = false; // Soft delete
        await _unitOfWork.ConcessionCategory.UpdateAsync(concessionCategory);
        await _unitOfWork.SaveAsync();

        _logger.LogInformation($"Concession Category {concessionCategory.Name} deleted successfully with ID {concessionCategory.Id}");
        return _mapper.Map<ConcessionCategoryDTO>(concessionCategory);
    }
    public async Task<ConcessionCategoryDTO> GetConcessionCategoryByIdAsync(int id)
    {
        var concessionCategory = await _unitOfWork.ConcessionCategory.GetAsync(c => c.Id == id && c.IsActive);
        if (concessionCategory == null)
        {
            _logger.LogError($"Concession Category with ID {id} not found");
            throw new AppException(ErrorCodes.ConcessionCategoryNotFound(id));
        }

        return _mapper.Map<ConcessionCategoryDTO>(concessionCategory);
    }
    public async Task<IEnumerable<ConcessionCategoryDTO>> GetAllConcessionCategoriesAsync()
    {
        var concessionCategories = await _unitOfWork.ConcessionCategory.GetAllAsync(c => c.IsActive);
        if (concessionCategories == null || !concessionCategories.Any())
        {
            _logger.LogInformation("No concession categories found");
            return new List<ConcessionCategoryDTO>();
        }

        return _mapper.Map<IEnumerable<ConcessionCategoryDTO>>(concessionCategories);
    }

}