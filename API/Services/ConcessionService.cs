using API.Data.Models;
using API.DTOs.Request;
using API.DTOs.Response;
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
        var concessions = await _unitOfWork.Concession.GetAllAsync(c => c.IsActive);
        if (concessions == null || !concessions.Any())
        {
            _logger.LogWarning("No active concessions found.");
            return new List<ConcessionDTO>();
        }
        return _mapper.Map<List<ConcessionDTO>>(concessions);
    }
    public async Task<List<ConcessionDTO>> GetAllConcessionsWithPaginationAsync(int pageNumber, int pageSize, bool? isActive = true)
    {
        var concessions = await _unitOfWork.Concession.GetAllAsync(c => c.IsActive == isActive, pageSize: pageSize, pageNumber: pageNumber);
        if (concessions == null || !concessions.Any())
        {
            _logger.LogWarning("No concessions found with the specified criteria.");
            return new List<ConcessionDTO>();
        }
        return _mapper.Map<List<ConcessionDTO>>(concessions);
    }
    public async Task<ConcessionDTO> CreateConcessionAsync(ConcessionCreateDTO concessionCreateDTO)
    {
        if (concessionCreateDTO == null)
        {
            _logger.LogError("ConcessionCreateDTO is null");
            throw new ArgumentNullException(nameof(concessionCreateDTO));
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

        var concession = await _unitOfWork.Concession.GetAsync(c => c.Id == id);
        if (concession == null)
        {
            _logger.LogError($"Concession with ID {id} not found");
            throw new KeyNotFoundException($"Concession with ID {id} not found");
        }

        _mapper.Map(concessionUpdateDTO, concession);
        concession.LastUpdatedAt = DateTime.Now;

        await _unitOfWork.Concession.UpdateAsync(concession);
        await _unitOfWork.SaveAsync();

        _logger.LogInformation($"Concession {concession.Name} updated successfully with ID {concession.Id}");
        return _mapper.Map<ConcessionDTO>(concession);
    }
    public async Task<ConcessionDTO> DeleteConcessionAsync(int id)
    {
        var concession = await _unitOfWork.Concession.GetAsync(c => c.Id == id);
        if (concession == null)
        {
            _logger.LogError($"Concession with ID {id} not found");
            throw new KeyNotFoundException($"Concession with ID {id} not found");
        }

        concession.IsActive = false;
        concession.LastUpdatedAt = DateTime.Now;

        await _unitOfWork.Concession.UpdateAsync(concession);
        await _unitOfWork.SaveAsync();

        _logger.LogInformation($"Concession {concession.Name} deleted successfully with ID {concession.Id}");
        return _mapper.Map<ConcessionDTO>(concession);
    }
    public async Task<ConcessionDTO> GetConcessionByIdAsync(int id)
    {
        var concession = await _unitOfWork.Concession.GetAsync(c => c.Id == id && c.IsActive);
        if (concession == null)
        {
            _logger.LogError($"Concession with ID {id} not found");
            throw new KeyNotFoundException($"Concession with ID {id} not found");
        }
        return _mapper.Map<ConcessionDTO>(concession);
    }
    public async Task<List<ConcessionDTO>> GetConcessionsByCategoryIdAsync(int categoryId)
    {
        var concessions = await _unitOfWork.Concession.GetAllAsync(c => c.CategoryId == categoryId && c.IsActive);
        if (concessions == null || !concessions.Any())
        {
            _logger.LogWarning($"No active concessions found for category ID {categoryId}");
            return new List<ConcessionDTO>();
        }
        return _mapper.Map<List<ConcessionDTO>>(concessions);
    }
    public async Task<List<ConcessionDTO>> SearchConcessionsByNameAsync(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            _logger.LogWarning("Search name is null or empty");
            return new List<ConcessionDTO>();
        }

        var concessions = await _unitOfWork.Concession.GetAllAsync(c => c.Name.Contains(name) && c.IsActive);
        if (concessions == null || !concessions.Any())
        {
            _logger.LogWarning($"No active concessions found matching the name '{name}'");
            return new List<ConcessionDTO>();
        }
        return _mapper.Map<List<ConcessionDTO>>(concessions);
    }
}