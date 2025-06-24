using Utility;
using Web.Models;
using Web.Models.DTOs.Request;
using Web.Services.IServices;

namespace Web.Services
{
    public class MovieService : BaseService, IMovieService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private string _baseUrl = string.Empty;

        public MovieService(IHttpClientFactory httpClientFactory, IConfiguration configuration) : base(httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            _baseUrl = configuration.GetValue<string>("ServiceUrls:CinemaBookingTicketAPI") ?? string.Empty;
        }

        public Task<T> CreateMovieAsync<T>(MovieCreateDTO movie, string token)
        {
            throw new NotImplementedException();
        }

        public Task<T> DeleteMovieAsync<T>(int movieId, string token)
        {
            throw new NotImplementedException();
        }

        public Task<T> GetAllMoviesAsync<T>()
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = Constant.ApiType.GET,
                Url = _baseUrl + "/api/Movie/get-all-movies"
            });
        }

        public Task<T> GetMovieByIdAsync<T>(int movieId)
        {
            throw new NotImplementedException();
        }

        public Task<T> UpdateMovieAsync<T>(int movieId, MovieUpdateDTO movie, string token)
        {
            throw new NotImplementedException();
        }
    }
}
