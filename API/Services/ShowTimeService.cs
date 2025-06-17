using API.Data.Models;
using API.DTOs.Request;
using API.DTOs.Response;
using API.Exceptions;
using API.Repositories.IRepositories;
using API.Services.IServices;
using AutoMapper;

namespace API.Services
{
    public class ShowTimeService : IShowTimeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<MovieService> _logger;

        public ShowTimeService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<MovieService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<List<ShowTimeDTO>> AddShowTimesAsync(List<ShowTimeCreateDTO> newShowTimes)
        {
            if (newShowTimes == null || newShowTimes.Count == 0)
            {
                _logger.LogError("ShowTimeCreates is null");
                throw new ArgumentNullException(nameof(newShowTimes));
            }

            // check movie and screen have valid ids and exist in the database
            foreach (var showTime in newShowTimes)
            {
                var movieExists = _unitOfWork.Movie.GetAsync(m => m.Id == showTime.MovieId);
                var screenExists = _unitOfWork.Screen.GetAsync(s => s.Id == showTime.ScreenId);

                if (movieExists == null)
                {
                    _logger.LogError($"Movie with ID {showTime.MovieId} does not exist.");
                    throw new AppException(ErrorCodes.MovieIdNotFound(showTime.MovieId));
                }
                if (screenExists == null)
                {
                    _logger.LogError($"Screen with ID {showTime.ScreenId} does not exist.");
                    throw new AppException(ErrorCodes.ScreenIdNotFound(showTime.ScreenId));
                }
            }
            // Map DTOs to ShowTime entities
            var showTimes = _mapper.Map<List<ShowTime>>(newShowTimes);
            await _unitOfWork.ShowTime.AddRangeAsync(showTimes);
            await _unitOfWork.SaveAsync();
            _logger.LogInformation("ShowTimes added successfully.");
            // Get the added showtimes and map them to DTOs
            var addedShowTimes = await _unitOfWork.ShowTime.GetAllAsync(
                s => newShowTimes.Select(nt => nt.MovieId).Contains(s.MovieId) &&
                     newShowTimes.Select(nt => nt.ScreenId).Contains(s.ScreenId),
                includeProperties: "Movie,Screen");
            return _mapper.Map<List<ShowTimeDTO>>(showTimes);
        }

        public async Task<ShowTimeDTO> DeleteShowTimeAsync(int id)
        {
            var showTime = await _unitOfWork.ShowTime.GetAsync(s => s.Id == id && s.IsActive == true);
            if (showTime == null)
            {
                _logger.LogError($"ShowTime with ID {id} not found.");
                throw new AppException(ErrorCodes.ShowTimeNotFound(id));
            }

            showTime.IsActive = false;

            await _unitOfWork.ShowTime.UpdateAsync(showTime);
            await _unitOfWork.SaveAsync();
            _logger.LogInformation($"ShowTime with ID {id} deleted successfully.");
            return _mapper.Map<ShowTimeDTO>(showTime);
        }

        public async Task<List<ShowTimeDTO>> GetAllShowTimesAsync(bool? isActive = true)
        {
            var showTimes = await _unitOfWork.ShowTime.GetAllAsync(
                m => m.IsActive == isActive,
                includeProperties: "Movie,Screen");
            if (showTimes == null || !showTimes.Any())
            {
                _logger.LogWarning("No showtimes found.");
                return new List<ShowTimeDTO>();
            }

            _logger.LogInformation("Retrieved all showtimes successfully.");
            return _mapper.Map<List<ShowTimeDTO>>(showTimes);
        }

        public async Task<List<ShowTimeDTO>> GetAllShowTimesWithPaginationAsync(int pageNumber, int pageSize, bool? isActive = true)
        {
            if (pageNumber <= 0 || pageSize <= 0)
            {
                _logger.LogError("Invalid pagination parameters.");
                throw new ArgumentException("Page number and size must be greater than zero.");
            }

            var showTimes = await _unitOfWork.ShowTime.GetAllAsync(
                m => m.IsActive == isActive,
                pageNumber: pageNumber,
                pageSize: pageSize,
                includeProperties: "Movie,Screen");

            if (showTimes == null || !showTimes.Any())
            {
                _logger.LogWarning("No showtimes found for the given pagination parameters.");
                return new List<ShowTimeDTO>();
            }

            _logger.LogInformation("Retrieved paginated showtimes successfully.");
            return _mapper.Map<List<ShowTimeDTO>>(showTimes);
        }

        public async Task<ShowTimeDTO> UpdateShowTimeAsync(int id, ShowTimeUpdateDTO dto)
        {
            if (dto == null)
            {
                _logger.LogError("ShowTimeUpdateDTO is null");
                throw new ArgumentNullException(nameof(dto));
            }

            var showTime = await _unitOfWork.ShowTime.GetAsync(s => s.Id == id && s.IsActive == true, includeProperties:"Movie,Screen");
            if (showTime == null)
            {
                _logger.LogError($"ShowTime with ID {id} not found.");
                throw new AppException(ErrorCodes.ShowTimeNotFound(id));
            }

            showTime = _mapper.Map(dto, showTime);

            await _unitOfWork.ShowTime.UpdateAsync(showTime);
            await _unitOfWork.SaveAsync();
            _logger.LogInformation($"ShowTime with ID {id} updated successfully.");
            var updatedShowTime = await _unitOfWork.ShowTime.GetAsync(s => s.Id == id && s.IsActive == true, includeProperties: "Movie,Screen");
            return _mapper.Map<ShowTimeDTO>(updatedShowTime);
        }

    }
}
