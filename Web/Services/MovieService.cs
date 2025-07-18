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
        public Task<T> GetAllMoviesAsync<T>() => SendAsync<T>(new APIRequest()
        {
            ApiType = Constant.ApiType.GET,
            Url = _baseUrl + "/api/Movie/get-all-Movies"
        });
        public Task<T> GetMovieByIdAsync<T>(int id)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = Constant.ApiType.GET,
                Url = _baseUrl + $"/api/Movie/{id}"
            });
        }
        public Task<T> CreateMovieAsync<T>(MovieCreateDTO Movie, string? token = null)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = Constant.ApiType.POST,
                Data = Movie,
                Token = token,
                Url = _baseUrl + "/api/Movie/Create"
            });
        }
        public Task<T> DeleteMovieAsync<T>(int id, string? token = null)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = Constant.ApiType.DELETE,
                Url = _baseUrl + $"/api/Movie/Delete/{id}",
                Token = token
            });
        }

        public Task<T> UpdateMovieAsync<T>(MovieUpdateDTO movie, string token)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = Constant.ApiType.PUT,
                Data = movie,
                Token = token,
                Url = _baseUrl + "/api/Movie/Update/" + movie.Id
            });
        }

        public Task<T> GetMoviesForHomeAsync<T>()
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = Constant.ApiType.GET,
                Url = _baseUrl + "/api/Movie/get-movies-for-home"
            });
        }

        public Task<T> GetShowtimesByMovieIdAsync<T>(int movieId)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = Constant.ApiType.GET,
                Url = _baseUrl + $"/api/Movie/{movieId}/showtimes"
            });
        }
        public Task<T> GetAllMoviesWithPaginationAsync<T>(int pageNumber, int pageSize, string status)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = Constant.ApiType.GET,
                Url = _baseUrl + $"/api/Movie/get-all-movies-with-pagination?pageNumber={pageNumber}&pageSize={pageSize}&status={status}"
            });
        }
    }
}
