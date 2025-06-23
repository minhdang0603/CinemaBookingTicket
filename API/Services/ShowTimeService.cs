using API.Data.Models;
using API.DTOs.Request;
using API.DTOs.Response;
using API.Exceptions;
using API.Repositories.IRepositories;
using API.Services.IServices;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

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
        public async Task<ShowTimeBulkResultDTO> AddShowTimesAsync(List<ShowTimeCreateDTO> newShowTimes)
        {
            if (newShowTimes == null || newShowTimes.Count == 0)
            {
                _logger.LogError("ShowTimeCreates is null");
                throw new ArgumentNullException(nameof(newShowTimes));
            }

            var successfulShowTimes = new List<ShowTimeCreateDTO>();
            var failedShowTimes = new List<ShowTimeFailureDTO>();

            // Check movie and screen have valid ids and exist in the database
            foreach (var showTime in newShowTimes)
            {
                try
                {
                    // Kiểm tra movie và screen có tồn tại không
                    var movie = await _unitOfWork.Movie.GetAsync(m => m.Id == showTime.MovieId);
                    var screen = await _unitOfWork.Screen.GetAsync(s => s.Id == showTime.ScreenId);

                    if (movie == null)
                    {
                        _logger.LogError($"Movie with ID {showTime.MovieId} does not exist.");
                        failedShowTimes.Add(new ShowTimeFailureDTO
                        {
                            ShowTime = showTime,
                            ErrorMessage = $"Movie with ID {showTime.MovieId} does not exist."
                        });
                        continue;
                    }

                    if (screen == null)
                    {
                        _logger.LogError($"Screen with ID {showTime.ScreenId} does not exist.");
                        failedShowTimes.Add(new ShowTimeFailureDTO
                        {
                            ShowTime = showTime,
                            ErrorMessage = $"Screen with ID {showTime.ScreenId} does not exist."
                        });
                        continue;
                    }

                    if (showTime.StartTime >= showTime.EndTime)
                    {
                        _logger.LogError($"Invalid showtime range for Movie ID {showTime.MovieId} and Screen ID {showTime.ScreenId}.");
                        failedShowTimes.Add(new ShowTimeFailureDTO
                        {
                            ShowTime = showTime,
                            ErrorMessage = $"Invalid showtime range: Start time must be before end time."
                        });
                        continue;
                    }

                    // Lấy các suất chiếu đã có trên cùng màn hình
                    var existingShowTimes = await _unitOfWork.ShowTime.GetAllAsync(
                        s => s.ScreenId == showTime.ScreenId && s.ShowDate == showTime.ShowDate && s.IsActive == true);

                    bool isOverlapping = existingShowTimes.Any(s =>
                        showTime.StartTime < s.EndTime && showTime.EndTime > s.StartTime
                    );

                    if (isOverlapping)
                    {
                        _logger.LogError($"Overlapping showtime for Screen ID {showTime.ScreenId}.");
                        failedShowTimes.Add(new ShowTimeFailureDTO
                        {
                            ShowTime = showTime,
                            ErrorMessage = $"Overlapping showtime: ScreenId={showTime.ScreenId}, Date={showTime.ShowDate}, Time={showTime.StartTime}-{showTime.EndTime}"
                        });
                        continue;
                    }

                    bool isTooClose = existingShowTimes.Any(s =>
                        Math.Abs((showTime.StartTime - s.EndTime).TotalMinutes) < 30 ||
                        Math.Abs((showTime.EndTime - s.StartTime).TotalMinutes) < 30
                    );

                    if (isTooClose)
                    {
                        _logger.LogError($"Showtimes too close for Screen ID {showTime.ScreenId}.");
                        failedShowTimes.Add(new ShowTimeFailureDTO
                        {
                            ShowTime = showTime,
                            ErrorMessage = $"Showtimes must be at least 30 minutes apart: ScreenId={showTime.ScreenId}, Date={showTime.ShowDate}, Time={showTime.StartTime}-{showTime.EndTime}"
                        });
                        continue;
                    }

                    // Nếu vượt qua tất cả các kiểm tra, thêm vào danh sách thành công
                    successfulShowTimes.Add(showTime);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error processing showtime: {ex.Message}");
                    failedShowTimes.Add(new ShowTimeFailureDTO
                    {
                        ShowTime = showTime,
                        ErrorMessage = $"Unexpected error: {ex.Message}"
                    });
                }
            }

            List<ShowTimeDTO> createdShowTimes = new List<ShowTimeDTO>();

            if (successfulShowTimes.Any())
            {
                // Map DTOs to ShowTime entities for successful entries only
                var showTimes = _mapper.Map<List<ShowTime>>(successfulShowTimes);
                await _unitOfWork.ShowTime.AddRangeAsync(showTimes);
                await _unitOfWork.SaveAsync();
                _logger.LogInformation($"{successfulShowTimes.Count} ShowTimes added successfully.");

                // Get the added showtimes and map them to DTOs
                var addedShowTimes = await _unitOfWork.ShowTime.GetAllAsync(
                    s => showTimes.Select(st => st.Id).Contains(s.Id),
                    include: q => q.Include(x => x.Movie)
                                   .Include(x => x.Screen)
                                       .ThenInclude(s => s.Theater)
                );

                createdShowTimes = _mapper.Map<List<ShowTimeDTO>>(addedShowTimes);
            }

            return new ShowTimeBulkResultDTO
            {
                SuccessfulShowTimes = createdShowTimes,
                FailedShowTimes = failedShowTimes
            };
        }

        public async Task<ShowTimeDTO> DeleteShowTimeAsync(int id)
        {
            var showTime = await _unitOfWork.ShowTime.GetAsync(
                s => s.Id == id && s.IsActive == true,
                include: q => q.Include(x => x.Movie)
                               .Include(x => x.Screen)
                                   .ThenInclude(s => s.Theater)
            );
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
                include: q => q.Include(x => x.Movie)
                               .Include(x => x.Screen)
                                   .ThenInclude(s => s.Theater)
            );
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
                include: q => q.Include(x => x.Movie)
                               .Include(x => x.Screen)
                                   .ThenInclude(s => s.Theater),
                pageNumber: pageNumber,
                pageSize: pageSize
            );

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

            var showTime = await _unitOfWork.ShowTime.GetAsync(
                s => s.Id == id && s.IsActive == true,
                include: q => q.Include(x => x.Movie)
                               .Include(x => x.Screen)
                                   .ThenInclude(s => s.Theater)
            );
            if (showTime == null)
            {
                _logger.LogError($"ShowTime with ID {id} not found.");
                throw new AppException(ErrorCodes.ShowTimeNotFound(id));
            }

            showTime = _mapper.Map(dto, showTime);

            await _unitOfWork.ShowTime.UpdateAsync(showTime);
            await _unitOfWork.SaveAsync();
            _logger.LogInformation($"ShowTime with ID {id} updated successfully.");
            var updatedShowTime = await _unitOfWork.ShowTime.GetAsync(
                s => s.Id == id && s.IsActive == true,
                include: q => q.Include(x => x.Movie)
                               .Include(x => x.Screen)
                                   .ThenInclude(s => s.Theater)
            );
            return _mapper.Map<ShowTimeDTO>(updatedShowTime);
        }

    }
}
