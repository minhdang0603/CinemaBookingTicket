using Utility;
using Web.Models;
using Web.Models.DTOs.Request;
using Web.Services.IServices;

namespace Web.Services
{
    public class ShowtimeService : BaseService, IShowtimeService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private string _baseUrl = string.Empty;

        public ShowtimeService(IHttpClientFactory httpClient, IConfiguration configuration) : base(httpClient)
        {
            _httpClientFactory = httpClient;
            _baseUrl = configuration.GetValue<string>("ServiceUrls:CinemaBookingTicketAPI") ?? string.Empty;
        }

        public Task<T> AddShowTimeAsync<T>(ShowTimeCreateDTO dto, string? token = null)
        {
            return SendAsync<T>(new APIRequest
            {
                ApiType = Constant.ApiType.POST,
                Data = dto,
                Url = $"{_baseUrl}/api/showtime",
                Token = token
            });
        }

        public Task<T> AddShowTimesAsync<T>(List<ShowTimeCreateDTO> newShowTimes, string? token = null)
        {
            return SendAsync<T>(new APIRequest
            {
                ApiType = Constant.ApiType.POST,
                Data = newShowTimes,
                Url = $"{_baseUrl}/api/showtime/add-showtimes",
                Token = token
            });
        }

        public Task<T> DeleteShowTimeAsync<T>(int showTimeId, string? token = null)
        {
            return SendAsync<T>(new APIRequest
            {
                ApiType = Constant.ApiType.DELETE,
                Url = $"{_baseUrl}/api/showtime/{showTimeId}",
                Token = token
            });
        }

        public Task<T> GetAllShowTimesAsync<T>()
        {
            return SendAsync<T>(new APIRequest
            {
                ApiType = Constant.ApiType.GET,
                Url = $"{_baseUrl}/api/showtime"
            });
        }

        public Task<T> GetShowTimeByIdAsync<T>(int showTimeId)
        {
            return SendAsync<T>(new APIRequest
            {
                ApiType = Constant.ApiType.GET,
                Url = $"{_baseUrl}/api/showtime/{showTimeId}"
            });
        }

        public Task<T> GetShowTimeSeatStatusAsync<T>(int showTimeId, string? token = null)
        {
            return SendAsync<T>(new APIRequest
            {
                ApiType = Constant.ApiType.GET,
                Url = $"{_baseUrl}/api/showtime/{showTimeId}/seats",
                Token = token
            });
        }

        public Task<T> UpdateShowTimeAsync<T>(int showTimeId, ShowTimeUpdateDTO updatedShowTime, string? token = null)
        {
            return SendAsync<T>(new APIRequest
            {
                ApiType = Constant.ApiType.PUT,
                Data = updatedShowTime,
                Url = $"{_baseUrl}/api/showtime/{showTimeId}",
                Token = token
            });
        }
    }
}
