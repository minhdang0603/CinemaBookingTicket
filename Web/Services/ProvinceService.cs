using Utility;
using Web.Models;
using Web.Models.DTOs.Request;
using Web.Services.IServices;

namespace Web.Services
{
    public class ProvinceService : BaseService, IProvinceService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _baseUrl;

        public ProvinceService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
            : base(httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            _baseUrl = configuration.GetValue<string>("ServiceUrls:CinemaBookingTicketAPI") ?? string.Empty;
        }

        public Task<T> GetAllProvincesAsync<T>()
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = Constant.ApiType.GET,
                Url = $"{_baseUrl}/api/Province/get-all-provinces"
            });
        }

        public Task<T> GetProvinceByIdAsync<T>(int id)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = Constant.ApiType.GET,
                Url = $"{_baseUrl}/api/Province/get-province-by-id?id={id}"
            });
        }

        public Task<T> SearchProvincesAsync<T>(string name)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = Constant.ApiType.GET,
                Url = $"{_baseUrl}/api/Province/search-province-by-name?name={name}"
            });
        }

        public Task<T> CreateProvinceAsync<T>(ProvinceCreateDTO dto, string token)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = Constant.ApiType.POST,
                Data = dto,
                Url = $"{_baseUrl}/api/Province/create-province",
                Token = token
            });
        }

        public Task<T> UpdateProvinceAsync<T>(int id, ProvinceUpdateDTO dto, string token)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = Constant.ApiType.PUT,
                Data = dto,
                Url = $"{_baseUrl}/api/Province/update-province?id={id}",
                Token = token
            });
        }

        public Task<T> DeleteProvinceAsync<T>(int id, string token)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = Constant.ApiType.DELETE,
                Url = $"{_baseUrl}/api/Province/delete/{id}",
                Token = token
            });
        }
    }
}
