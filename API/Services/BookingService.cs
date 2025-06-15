using API.DTOs.Response;
using API.Exceptions;
using API.Repositories.IRepositories;
using API.Services.IServices;
using AutoMapper;

namespace API.Services
{
    public class BookingService : IBookingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public BookingService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<BookingHistoryDTO>> GetUserBookingHistoryAsync(string userId, int page = 1, int pageSize = 10)
        {
            var bookings = await _unitOfWork.Booking.GetUserBookingHistoryAsync(userId, page, pageSize);
            return _mapper.Map<IEnumerable<BookingHistoryDTO>>(bookings);
        }

        public async Task<BookingDetailDTO> GetBookingDetailAsync(int bookingId, string userId)
        {
            var booking = await _unitOfWork.Booking.GetBookingDetailAsync(bookingId);

            if (booking == null)
                throw new AppException(ErrorCodes.BookingNotFound(bookingId)); 

            // Check ownership
            if (booking.ApplicationUser.Id != userId)
                throw new AppException(ErrorCodes.BookingNotFound(bookingId));

            return _mapper.Map<BookingDetailDTO>(booking);
        }

        public async Task<int> GetUserBookingCountAsync(string userId)
        {
            return await _unitOfWork.Booking.GetUserBookingCountAsync(userId);
        }
    }
}