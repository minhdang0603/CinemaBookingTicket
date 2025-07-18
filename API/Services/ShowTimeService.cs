using API.Data.Models;
using API.DTOs.Request;
using API.DTOs.Response;
using API.Exceptions;
using API.Repositories.IRepositories;
using API.Services.IServices;
using AutoMapper;
using Utility;

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

		// Kiểm tra movie và screen có tồn tại không
		private async Task<(Movie movie, Screen screen)> ValidateMovieAndScreenAsync(int movieId, int screenId)
		{
			var movie = await _unitOfWork.Movie.GetAsync(m => m.Id == movieId && m.IsActive);
			var screen = await _unitOfWork.Screen.GetAsync(s => s.Id == screenId && s.IsActive);

			if (movie == null)
			{
				_logger.LogError($"Movie with ID {movieId} does not exist.");
				throw new AppException(ErrorCodes.EntityNotFound("Movie", movieId));
			}

			if (screen == null)
			{
				_logger.LogError($"Screen with ID {screenId} does not exist.");
				throw new AppException(ErrorCodes.EntityNotFound("Screen", screenId));
			}

			return (movie, screen);
		}

		// Kiểm tra thời gian chiếu hợp lệ
		private void ValidateShowTimeRange(TimeOnly startTime, TimeOnly endTime, int movieId, int screenId)
		{
			if (startTime >= endTime)
			{
				_logger.LogError($"Invalid showtime range for Movie ID {movieId} and Screen ID {screenId}.");
				throw new AppException(ErrorCodes.InvalidShowTimeRange(movieId, screenId));
			}
		}

		// Kiểm tra suất chiếu có bị chồng chéo với các suất chiếu khác không
		private async Task ValidateShowTimeOverlappingAsync(ShowTimeCreateDTO dto, int? excludeShowTimeId = null)
		{
			// Lấy các suất chiếu đã có trên cùng màn hình
			var existingShowTimes = await _unitOfWork.ShowTime.GetAllAsync(
				s => s.ScreenId == dto.ScreenId &&
					 s.ShowDate == dto.ShowDate &&
					 s.IsActive == true &&
					 (!excludeShowTimeId.HasValue || s.Id != excludeShowTimeId));

			var overlappedShowtime = existingShowTimes.FirstOrDefault(s =>
				dto.StartTime < s.EndTime && dto.EndTime > s.StartTime
			);

			if (overlappedShowtime != null)
			{
				_logger.LogError($"Overlapping showtime for Screen ID {dto.ScreenId}.");
				throw new AppException(ErrorCodes.InvalidStartForShowTime(dto.ScreenId, overlappedShowtime.EndTime, dto.StartTime));
			}

			var tooCloseShowtime = existingShowTimes.FirstOrDefault(s =>
				Math.Abs((dto.StartTime - s.EndTime).TotalMinutes) < 30 ||
				Math.Abs((dto.EndTime - s.StartTime).TotalMinutes) < 30
			);

			if (tooCloseShowtime != null)
			{
				_logger.LogError($"Showtimes too close for Screen ID {dto.ScreenId}.");
				throw new AppException(ErrorCodes.InvalidStartForShowTime(dto.ScreenId, tooCloseShowtime.EndTime, dto.StartTime));
			}
		}

		// Kiểm tra suất chiếu có bị chồng chéo với các suất chiếu khác trong cùng batch không
		private bool IsOverlappingWithNewShowtimes(ShowTimeCreateDTO newShowTime, List<ShowTimeCreateDTO> validatedShowTimes)
		{
			return validatedShowTimes
				.Where(s => s.ScreenId == newShowTime.ScreenId && s.ShowDate == newShowTime.ShowDate)
				.Any(s => newShowTime.StartTime < s.EndTime && newShowTime.EndTime > s.StartTime);
		}

		// Kiểm tra suất chiếu có quá gần với các suất chiếu khác trong cùng batch không
		private bool IsTooCloseToNewShowtimes(ShowTimeCreateDTO newShowTime, List<ShowTimeCreateDTO> validatedShowTimes)
		{
			return validatedShowTimes
				.Where(s => s.ScreenId == newShowTime.ScreenId && s.ShowDate == newShowTime.ShowDate)
				.Any(s =>
					Math.Abs((newShowTime.StartTime - s.EndTime).TotalMinutes) < 30 ||
					Math.Abs((newShowTime.EndTime - s.StartTime).TotalMinutes) < 30
				);
		}

		public async Task<ShowTimeDTO> AddShowTimeAsync(ShowTimeCreateDTO dto)
		{
			// Kiểm tra movie và screen
			var (movie, screen) = await ValidateMovieAndScreenAsync(dto.MovieId, dto.ScreenId);

			// Tính end time dựa trên movie duration và start time
			dto.EndTime = dto.StartTime.AddMinutes(movie.Duration);

			// Kiểm tra thời gian chiếu
			ValidateShowTimeRange(dto.StartTime, dto.EndTime, dto.MovieId, dto.ScreenId);

			// Kiểm tra chồng chéo với các suất chiếu khác
			await ValidateShowTimeOverlappingAsync(dto);

			// Map DTO to ShowTime entity
			var showTime = _mapper.Map<ShowTime>(dto);

			await _unitOfWork.ShowTime.CreateAsync(showTime);
			await _unitOfWork.SaveAsync();
			return _mapper.Map<ShowTimeDTO>(showTime);
		}

		// Kiểm tra và xác thực một showtime trước khi thêm vào batch
		private async Task<(bool isValid, string? errorMessage, ShowTimeCreateDTO? validatedShowTime)> ValidateShowTimeForBatchAsync(
			ShowTimeCreateDTO showTime, List<ShowTimeCreateDTO> validatedShowTimes)
		{
			try
			{
				// Kiểm tra movie và screen
				var (movie, screen) = await ValidateMovieAndScreenAsync(showTime.MovieId, showTime.ScreenId);

				// Tính end time dựa trên movie duration và start time
				showTime.EndTime = showTime.StartTime.AddMinutes(movie.Duration);

				// Kiểm tra thời gian chiếu
				ValidateShowTimeRange(showTime.StartTime, showTime.EndTime, showTime.MovieId, showTime.ScreenId);

				// Lấy các suất chiếu đã có trên cùng màn hình
				var existingShowTimes = await _unitOfWork.ShowTime.GetAllAsync(
					s => s.ScreenId == showTime.ScreenId && s.ShowDate == showTime.ShowDate && s.IsActive == true);

				// Kiểm tra chồng chéo với các suất chiếu hiện có
				bool isOverlapping = existingShowTimes.Any(s =>
					showTime.StartTime < s.EndTime && showTime.EndTime > s.StartTime
				);

				if (isOverlapping)
				{
					_logger.LogError($"Overlapping showtime for Screen ID {showTime.ScreenId}.");
					return (false, $"Overlapping showtime: ScreenId={showTime.ScreenId}, Date={showTime.ShowDate}, Time={showTime.StartTime}-{showTime.EndTime}", null);
				}

				// Kiểm tra khoảng cách thời gian với các suất chiếu hiện có
				bool isTooClose = existingShowTimes.Any(s =>
					Math.Abs((showTime.StartTime - s.EndTime).TotalMinutes) < 30 ||
					Math.Abs((showTime.EndTime - s.StartTime).TotalMinutes) < 30
				);

				if (isTooClose)
				{
					_logger.LogError($"Showtimes too close for Screen ID {showTime.ScreenId}.");
					return (false, $"Showtimes must be at least 30 minutes apart: ScreenId={showTime.ScreenId}, Date={showTime.ShowDate}, Time={showTime.StartTime}-{showTime.EndTime}", null);
				}

				// Kiểm tra chồng chéo với các showtime khác trong cùng lần thêm
				if (IsOverlappingWithNewShowtimes(showTime, validatedShowTimes))
				{
					_logger.LogError($"Overlapping with another showtime in the same batch for Screen ID {showTime.ScreenId}.");
					return (false, $"Overlapping with another showtime in the same batch: ScreenId={showTime.ScreenId}, Date={showTime.ShowDate}, Time={showTime.StartTime}-{showTime.EndTime}", null);
				}

				// Kiểm tra khoảng cách thời gian với các showtime khác trong cùng lần thêm
				if (IsTooCloseToNewShowtimes(showTime, validatedShowTimes))
				{
					_logger.LogError($"Too close to another showtime in the same batch for Screen ID {showTime.ScreenId}.");
					return (false, $"Showtimes must be at least 30 minutes apart from other new showtimes: ScreenId={showTime.ScreenId}, Date={showTime.ShowDate}, Time={showTime.StartTime}-{showTime.EndTime}", null);
				}

				return (true, null, showTime);
			}
			catch (AppException ex)
			{
				return (false, ex.Message, null);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Error validating showtime for movie ID {showTime.MovieId} and screen ID {showTime.ScreenId}");
				return (false, $"Unexpected error validating showtime: {ex.Message}", null);
			}
		}

		public async Task<ShowTimeBulkResultDTO> AddShowTimesAsync(List<ShowTimeCreateDTO> newShowTimes)
		{
			if (newShowTimes == null || newShowTimes.Count == 0)
			{
				_logger.LogError("ShowTimeCreates is null");
				throw new ArgumentNullException(nameof(newShowTimes));
			}

			var successfulShowTimes = new List<ShowTimeCreateDTO>();
			var failedShowTimes = new List<string>();
			var validatedShowTimes = new List<ShowTimeCreateDTO>(); // Danh sách để theo dõi các showtime đã được xác thực

			// Xác thực từng showtime trong batch
			foreach (var showTime in newShowTimes)
			{
				var (isValid, errorMessage, validatedShowTime) = await ValidateShowTimeForBatchAsync(showTime, validatedShowTimes);

				if (isValid && validatedShowTime != null)
				{
					validatedShowTimes.Add(validatedShowTime);
					successfulShowTimes.Add(validatedShowTime);
				}
				else if (!string.IsNullOrEmpty(errorMessage))
				{
					failedShowTimes.Add(errorMessage);
				}
			}

			// Lưu các showtime hợp lệ vào database
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
					includeProperties: "Movie,Screen");

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
			var showTime = await _unitOfWork.ShowTime.GetAsync(s => s.Id == id && s.IsActive == true);
			if (showTime == null)
			{
				_logger.LogError($"ShowTime with ID {id} not found.");
				throw new AppException(ErrorCodes.EntityNotFound("ShowTime", id));
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
				includeProperties: "Movie,Screen,Screen.Theater");
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
				includeProperties: "Movie,Screen,Screen.Theater");

			if (showTimes == null || !showTimes.Any())
			{
				_logger.LogWarning("No showtimes found for the given pagination parameters.");
				return new List<ShowTimeDTO>();
			}

			_logger.LogInformation("Retrieved paginated showtimes successfully.");
			return _mapper.Map<List<ShowTimeDTO>>(showTimes);
		}

		public async Task<ShowTimeDTO> GetShowTimeByIdAsync(int id, bool? isActive = true)
		{
			var showTime = await _unitOfWork.ShowTime.GetAsync(
				s => s.Id == id && s.IsActive == isActive,
				includeProperties: "Movie,Screen,Screen.Theater");
			if (showTime == null)
			{
				_logger.LogError($"ShowTime with ID {id} not found.");
				throw new AppException(ErrorCodes.EntityNotFound("ShowTime", id));
			}
			return _mapper.Map<ShowTimeDTO>(showTime);
		}

		public async Task<ShowTimeDTO> UpdateShowTimeAsync(int id, ShowTimeUpdateDTO dto)
		{
			if (dto == null)
			{
				_logger.LogError("ShowTimeUpdateDTO is null");
				throw new ArgumentNullException(nameof(dto));
			}

			// Kiểm tra showtime tồn tại
			var showTime = await _unitOfWork.ShowTime.GetAsync(s => s.Id == id && s.IsActive == true, includeProperties: "Movie,Screen");
			if (showTime == null)
			{
				_logger.LogError($"ShowTime with ID {id} not found.");
				throw new AppException(ErrorCodes.EntityNotFound("ShowTime", id));
			}

			// Kiểm tra movie và screen
			var (movie, screen) = await ValidateMovieAndScreenAsync(dto.MovieId, dto.ScreenId);

			// Tính end time dựa trên movie duration và start time
			dto.EndTime = dto.StartTime.AddMinutes(movie.Duration);

			// Kiểm tra thời gian chiếu
			ValidateShowTimeRange(dto.StartTime, dto.EndTime, dto.MovieId, dto.ScreenId);

			// Convert DTO để có thể sử dụng cho hàm validation
			var showTimeCreateDTO = new ShowTimeCreateDTO
			{
				MovieId = dto.MovieId,
				ScreenId = dto.ScreenId,
				ShowDate = dto.ShowDate,
				StartTime = dto.StartTime,
				EndTime = dto.EndTime,
				BasePrice = dto.BasePrice
			};

			// Kiểm tra chồng chéo với các suất chiếu khác (loại trừ chính nó)
			await ValidateShowTimeOverlappingAsync(showTimeCreateDTO, id);

			// Update showTime entity
			showTime = _mapper.Map(dto, showTime);

			await _unitOfWork.ShowTime.UpdateAsync(showTime);
			await _unitOfWork.SaveAsync();
			_logger.LogInformation($"ShowTime with ID {id} updated successfully.");
			return _mapper.Map<ShowTimeDTO>(showTime);
		}

		public async Task<ShowTimeSeatStatusDTO> GetShowTimeSeatStatusAsync(int showTimeId)
		{
			_logger.LogInformation($"Getting seat status for showtime ID: {showTimeId}");

			// Lấy showtime và kiểm tra tồn tại
			var showtime = await _unitOfWork.ShowTime.GetAsync(
				filter: st => st.Id == showTimeId && st.IsActive,
				includeProperties: "Movie,Screen,Screen.Theater,Screen.Seats,Screen.Seats.SeatType"
			);

			if (showtime == null)
			{
				_logger.LogError($"ShowTime with ID {showTimeId} not found or inactive");
				throw new AppException(ErrorCodes.EntityNotFound("ShowTime", showTimeId));
			}

			// Lấy tất cả booking details cho showtime này (ghế đã được đặt)
			var bookingDetails = await _unitOfWork.BookingDetail.GetAllAsync(
				filter: bd => bd.Booking.ShowTimeId == showTimeId &&
						 (bd.Booking.BookingStatus.ToLower() == Constant.Booking_Status_Confirmed ||
						  bd.Booking.BookingStatus.ToLower() == Constant.Booking_Status_Pending),
				includeProperties: "Booking,Seat"
			);

			// Tạo set chứa ID của các ghế đã được đặt
			var bookedSeatIds = new HashSet<int>(bookingDetails.Select(bd => bd.SeatId));

			// Map showtime to ShowTimeSeatStatusDTO using AutoMapper
			var result = _mapper.Map<ShowTimeSeatStatusDTO>(showtime);

			// Map seats and add booking status information
			var seatsList = showtime.Screen.Seats
				.Where(s => s.IsActive)
				.Select(seat =>
				{
					var seatDTO = _mapper.Map<SeatBookingStatusDTO>(seat);
					seatDTO.Price = showtime.BasePrice * seat.SeatType.PriceMultiplier;
					seatDTO.IsBooked = bookedSeatIds.Contains(seat.Id);
					return seatDTO;
				})
				.OrderBy(s => s.SeatRow)
				.ThenBy(s => s.SeatNumber)
				.ToList();

			result.Seats = seatsList;

			return result;
		}
		public async Task<List<ShowTimeDTO>> GetShowTimesByMovieIdAsync(int movieId, DateOnly? date = null, int? provinceId = null)
		{
			if (movieId <= 0)
			{
				_logger.LogError($"Invalid movie ID: {movieId}");
				throw new ArgumentException("Invalid movie ID", nameof(movieId));
			}
			var showTimes = await _unitOfWork.ShowTime.GetShowTimesByMovieIdAsync(movieId, date, provinceId);
			if (showTimes == null || !showTimes.Any())
			{
				_logger.LogWarning($"No showtimes found for Movie ID {movieId}.");
				return new List<ShowTimeDTO>();
			}
			_logger.LogInformation($"Retrieved {showTimes.Count} showtimes for Movie ID {movieId}.");
			return _mapper.Map<List<ShowTimeDTO>>(showTimes);
		}
	}
}
