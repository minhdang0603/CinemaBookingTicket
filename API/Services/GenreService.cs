using API.Data.Models;
using API.DTOs.Request;
using API.DTOs.Response;
using API.Exceptions;
using API.Repositories.IRepositories;
using API.Services.IServices;
using AutoMapper;

namespace API.Services
{
    public class GenreService : IGenreService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<MovieService> _logger;

        public GenreService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<MovieService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }
        public async Task CreateGenreAsync(GenreCreateDTO genreCreateDTO)
        {
            if (genreCreateDTO == null)
            {
                _logger.LogError("GenreCreateDTO is null");
                throw new ArgumentNullException(nameof(genreCreateDTO));
            }

            if (await _unitOfWork.Genre.GetAsync(m => m.Name == genreCreateDTO.Name) != null)
            {
                _logger.LogError($"Genre with Name {genreCreateDTO.Name} already exists");
                throw new AppException(ErrorCodes.GenreAlreadyExists(genreCreateDTO.Name));
            }
            var genre = _mapper.Map<Genre>(genreCreateDTO);

            genre.CreatedAt = DateTime.Now;
            genre.LastUpdatedAt = DateTime.Now;

            await _unitOfWork.Genre.CreateAsync(genre);
            await _unitOfWork.SaveAsync();

            _logger.LogInformation($"Genre {genre.Name} created successfully with ID {genre.Id}");
        }

        public async Task DeleteGenreAsync(int id)
        {
            var genre = await _unitOfWork.Genre.GetAsync(m => m.Id == id);
            if (genre == null)
            {
                _logger.LogError($"Genre with ID {id} not found");
                throw new AppException(ErrorCodes.MovieNotFound(id));
            }
            genre.IsActive = false;
            genre.LastUpdatedAt = DateTime.Now;
            await _unitOfWork.Genre.UpdateAsync(genre);
            await _unitOfWork.SaveAsync();
            _logger.LogInformation($"Genre {genre.Name} deleted successfully with ID {genre.Id}");
        }

        public async Task<List<GenreDTO>> GetAllGenresAsync(bool? isActive = true)
        {
            var genres = await _unitOfWork.Genre.GetAllAsync(
            m => m.IsActive == isActive);
            if (genres == null || !genres.Any())
            {
                _logger.LogInformation("No genres found");
                return new List<GenreDTO>();
            }

            return _mapper.Map<List<GenreDTO>>(genres);
        }

        public async Task<List<GenreDTO>> GetAllGenresWithPaginationAsync(int pageNumber, int pageSize, bool? isActive = true)
        {
            var genres = await _unitOfWork.Genre.GetAllAsync(
            m => m.IsActive == isActive,
            pageSize: pageSize,
            pageNumber: pageNumber);
            if (genres == null || !genres.Any())
            {
                _logger.LogInformation("No genres found");
                return new List<GenreDTO>();
            }
            return _mapper.Map<List<GenreDTO>>(genres);
        }

        public async Task<GenreDTO> GetGenreByIdAsync(int id, bool? isActive = true)
        {
            var genre = await _unitOfWork.Genre.GetAsync(
            m => m.Id == id && m.IsActive == isActive);
            if (genre == null)
            {
                _logger.LogError($"Genre with ID {id} not found");
                throw new AppException(ErrorCodes.GenreNotFound(id));
            }
            return _mapper.Map<GenreDTO>(genre);
        }

        public async Task<List<GenreDTO>> SearchGenresAsync(string searchTerm, bool? isActive = true)
        {
            var genres = await _unitOfWork.Genre.GetAllAsync(
            m => m.Name.Contains(searchTerm) && m.IsActive == isActive);
            return _mapper.Map<List<GenreDTO>>(genres);
        }

        public async Task UpdateGenreAsync(int id, GenreUpdateDTO genreUpdateDTO)
        {
            var genre = await _unitOfWork.Genre.GetAsync(m => m.Id == id && m.IsActive == true);
            if (genre == null)
            {
                _logger.LogError($"Genre with ID {id} not found");
                throw new AppException(ErrorCodes.GenreNotFound(id));
            }
            genre = _mapper.Map(genreUpdateDTO, genre);
            await _unitOfWork.Genre.UpdateAsync(genre);
            await _unitOfWork.SaveAsync();
        }
    }
}
