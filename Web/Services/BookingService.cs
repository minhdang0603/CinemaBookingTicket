using Web.Models;
using Web.Models.DTOs.Request;
using Web.Models.DTOs.Response;
using Web.Services.IServices;
using Utility;

namespace Web.Services
{
    public class BookingService : BaseService, IBookingService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private string _baseUrl = string.Empty;

        public BookingService(IHttpClientFactory httpClientFactory, IConfiguration configuration) : base(httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            _baseUrl = configuration.GetValue<string>("ServiceUrls:CinemaBookingTicketAPI") ?? string.Empty;
        }

        public Task<T> GetBookingByIdAsync<T>(int bookingId, string? token = null)
        {
            return SendAsync<T>(new APIRequest
            {
                ApiType = Constant.ApiType.GET,
                Url = $"{_baseUrl}/api/booking/{bookingId}",
                Token = token ?? string.Empty
            });
        }

        public Task<T> GetAllBookingsAsync<T>(string? token = null)
        {
            return SendAsync<T>(new APIRequest
            {
                ApiType = Constant.ApiType.GET,
                Url = $"{_baseUrl}/api/booking",
                Token = token ?? string.Empty
            });
        }

        public Task<T> GetMyBookingsAsync<T>(string? token = null)
        {
            return SendAsync<T>(new APIRequest
            {
                ApiType = Constant.ApiType.GET,
                Url = $"{_baseUrl}/api/booking/my-bookings",
                Token = token ?? string.Empty
            });
        }

        public Task<T> DeleteBookingAsync<T>(int bookingId, string token)
        {
            return SendAsync<T>(new APIRequest
            {
                ApiType = Constant.ApiType.DELETE,
                Url = $"{_baseUrl}/api/booking/{bookingId}",
                Token = token
            });
        }

        public Task CancelBookingAsync(int bookingId, string? token = null)
        {
            return SendAsync<object>(new APIRequest
            {
                ApiType = Constant.ApiType.POST,
                Url = $"{_baseUrl}/api/booking/{bookingId}/cancel",
                Token = token ?? string.Empty
            });
        }

        public Task<T> CreateBookingAsync<T>(BookingCreateDTO bookingCreateDTO, string token)
        {
            return SendAsync<T>(new APIRequest
            {
                ApiType = Constant.ApiType.POST,
                Data = bookingCreateDTO,
                Url = $"{_baseUrl}/api/booking",
                Token = token
            });
        }

        public Task<T> UpdateBookingAsync<T>(BookingUpdateDTO bookingUpdateDTO, string token)
        {
            return SendAsync<T>(new Models.APIRequest()
            {
                ApiType = Constant.ApiType.PUT,
                Data = bookingUpdateDTO,
                Url = _baseUrl + "/api/Booking/" + bookingUpdateDTO.BookingId,
                Token = token
            });
        }
    }
}
