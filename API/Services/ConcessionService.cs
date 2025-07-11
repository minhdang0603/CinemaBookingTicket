using API.Data.Models;
using API.DTOs.Request;
using API.DTOs.Response;
using API.Exceptions;
using API.Repositories.IRepositories;
using API.Services.IServices;
using AutoMapper;

namespace API.Services;

public class ConcessionService : IConcessionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<MovieService> _logger;
    public ConcessionService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<MovieService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }
    public async Task<List<ConcessionDTO>> GetAllConcessionsAsync()
    {
        var concessions = await _unitOfWork.Concession.GetAllAsync(c => c.IsActive, includeProperties: "Category");
        foreach (var concession in concessions)
        {
            _logger.LogInformation($"Concession: {concession.Name}, CategoryId: {concession.CategoryId}, Category: {concession.Category?.Name ?? "NULL"}");
        }
        if (concessions == null || concessions.Count == 0)
        {
            _logger.LogWarning("No active concessions found");
            return new List<ConcessionDTO>();
        }
        return _mapper.Map<List<ConcessionDTO>>(concessions);
    }
    public async Task<ConcessionDTO> GetConcessionByIdAsync(int id)
    {
        var concession = await _unitOfWork.Concession.GetAsync(c => c.Id == id && c.IsActive, includeProperties: "Category");
        if (concession == null)
        {
            _logger.LogError($"Concession with ID {id} not found");
            throw new AppException(ErrorCodes.EntityNotFound("Concession", id));
        }
        return _mapper.Map<ConcessionDTO>(concession);
    }
    public async Task<ConcessionDTO> CreateConcessionAsync(ConcessionCreateDTO concessionCreateDTO)
    {
        if (concessionCreateDTO == null)
        {
            _logger.LogError("ConcessionCreateDTO is null");
            throw new ArgumentNullException(nameof(concessionCreateDTO));
        }

        var existingConcession = await _unitOfWork.Concession.GetAsync(c => c.Name == concessionCreateDTO.Name, includeProperties: "Category");
        if (existingConcession != null)
        {
            _logger.LogError($"Concession with Name {concessionCreateDTO.Name} already exists");
            throw new AppException(ErrorCodes.ConcessionAlreadyExists(concessionCreateDTO.Name));
        }

        var concession = _mapper.Map<Concession>(concessionCreateDTO);
        await _unitOfWork.Concession.CreateAsync(concession);
        await _unitOfWork.SaveAsync();

        _logger.LogInformation($"Concession {concession.Name} created successfully with ID {concession.Id}");
        return _mapper.Map<ConcessionDTO>(concession);
    }
    public async Task<ConcessionDTO> UpdateConcessionAsync(int id, ConcessionUpdateDTO concessionUpdateDTO)
    {
        if (concessionUpdateDTO == null)
        {
            _logger.LogError("ConcessionUpdateDTO is null");
            throw new ArgumentNullException(nameof(concessionUpdateDTO));
        }

        var concession = await _unitOfWork.Concession.GetAsync(c => c.Id == id && c.IsActive, includeProperties: "Category");
        if (concession == null)
        {
            _logger.LogError($"Concession with ID {id} not found");
            throw new AppException(ErrorCodes.EntityNotFound("Concession", id));
        }

        _mapper.Map(concessionUpdateDTO, concession);

        await _unitOfWork.Concession.UpdateAsync(concession);
        await _unitOfWork.SaveAsync();

        _logger.LogInformation($"Concession {concession.Name} updated successfully with ID {concession.Id}");
        return _mapper.Map<ConcessionDTO>(concession);
    }
    public async Task<ConcessionDTO> DeleteConcessionAsync(int id)
    {
        var concession = await _unitOfWork.Concession.GetAsync(c => c.Id == id && c.IsActive, includeProperties: "Category");
        if (concession == null)
        {
            _logger.LogError($"Concession with ID {id} not found");
            throw new AppException(ErrorCodes.EntityNotFound("Concession", id));
        }

        concession.IsActive = false;
        await _unitOfWork.Concession.UpdateAsync(concession);
        await _unitOfWork.SaveAsync();

        _logger.LogInformation($"Concession {concession.Name} deleted successfully with ID {concession.Id}");
        return _mapper.Map<ConcessionDTO>(concession);
    }
    public async Task<List<ConcessionDTO>> GetAllConcessionsByCategoryIdAsync(int categoryId)
    {
        var concessions = await _unitOfWork.Concession.GetAllAsync(c => c.CategoryId == categoryId && c.IsActive, includeProperties: "Category");
        if (concessions == null || concessions.Count == 0)
        {
            _logger.LogWarning($"No concessions found for category ID {categoryId}");
            return new List<ConcessionDTO>();
        }
        return _mapper.Map<List<ConcessionDTO>>(concessions);
    }
}