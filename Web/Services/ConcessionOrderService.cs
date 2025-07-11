using Web.Models;
using Web.Models.DTOs.Request;
using Web.Models.DTOs.Response;
using Web.Services.IServices;
using Utility;

namespace Web.Services
{
    public class ConcessionOrderService : BaseService, IConcessionOrderService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private string _baseUrl = string.Empty;

        public ConcessionOrderService(IHttpClientFactory httpClientFactory, IConfiguration configuration) : base(httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            _baseUrl = configuration.GetValue<string>("ServiceUrls:CinemaBookingTicketAPI") ?? string.Empty;
        }

        public Task<T> CreateConcessionOrderAsync<T>(ConcessionOrderCreateDTO concessionOrderCreateDTO, string? token = null)
        {
            return SendAsync<T>(new APIRequest
            {
                ApiType = Constant.ApiType.POST,
                Data = concessionOrderCreateDTO,
                Url = $"{_baseUrl}/api/concessionorder",
                Token = token ?? string.Empty
            });
        }

        public Task<T> GetConcessionOrderByIdAsync<T>(int concessionOrderId, string? token = null)
        {
            return SendAsync<T>(new APIRequest
            {
                ApiType = Constant.ApiType.GET,
                Url = $"{_baseUrl}/api/concessionorder/{concessionOrderId}",
                Token = token ?? string.Empty
            });
        }

        public Task<T> GetAllConcessionOrdersAsync<T>(string? token = null)
        {
            return SendAsync<T>(new APIRequest
            {
                ApiType = Constant.ApiType.GET,
                Url = $"{_baseUrl}/api/concessionorder",
                Token = token ?? string.Empty
            });
        }

        public Task<T> GetConcessionOrdersByBookingIdAsync<T>(int bookingId, string? token = null)
        {
            return SendAsync<T>(new APIRequest
            {
                ApiType = Constant.ApiType.GET,
                Url = $"{_baseUrl}/api/concessionorder/by-booking/{bookingId}",
                Token = token ?? string.Empty
            });
        }

        public Task<T> DeleteConcessionOrderAsync<T>(int concessionOrderId, string? token = null)
        {
            return SendAsync<T>(new APIRequest
            {
                ApiType = Constant.ApiType.DELETE,
                Url = $"{_baseUrl}/api/concessionorder/{concessionOrderId}",
                Token = token ?? string.Empty
            });
        }
    }
}
