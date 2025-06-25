using Web.Models;
using Utility;
using Web.Models.DTOs.Request;
using Web.Services.IServices;

namespace Web.Services
{
    public class AuthService : BaseService, IAuthService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private string _baseUrl = string.Empty;

        public AuthService(IHttpClientFactory httpClientFactory, IConfiguration configuration) : base(httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            _baseUrl = configuration.GetValue<string>("ServiceUrls:CinemaBookingTicketAPI") ?? string.Empty;

        }

        public Task<T> LoginAsync<T>(LoginRequestDTO loginRequest)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = Constant.ApiType.POST,
                Data = loginRequest,
                Url = _baseUrl + "/api/Auth/Login"
            });
        }

        public Task LogoutAsync()
        {
            throw new NotImplementedException();
        }

        public Task<T> RegisterAsync<T>(UserCreateDTO registerRequest)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = Constant.ApiType.POST,
                Data = registerRequest,
                Url = _baseUrl + "/api/Auth/Register"
            });
        }
    }
}