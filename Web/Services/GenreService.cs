using Utility;
using Web.Models;
using Web.Models.DTOs.Request;
using Web.Services.IServices;

namespace Web.Services;

public class GenreService : BaseService, IGenreService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private string _baseUrl = string.Empty;

    public GenreService(IHttpClientFactory httpClientFactory, IConfiguration configuration) : base(httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
        _baseUrl = configuration.GetValue<string>("ServiceUrls:CinemaBookingTicketAPI") ?? string.Empty;
    }
    public Task<T> GetAllGenresAsync<T>()
    {
        return SendAsync<T>(new APIRequest()
        {
            ApiType = Constant.ApiType.GET,
            Url = _baseUrl + "/api/Genre/get-all-genres"
        });
    }
    public Task<T> GetGenreByIdAsync<T>(int id)
    {
        return SendAsync<T>(new APIRequest()
        {
            ApiType = Constant.ApiType.GET,
            Url = _baseUrl + $"/api/Genre/{id}"
        });
    }
    public Task<T> CreateGenreAsync<T>(GenreCreateDTO genre, string? token = null)
    {
        return SendAsync<T>(new APIRequest()
        {
            ApiType = Constant.ApiType.POST,
            Data = genre,
            Token = token,
            Url = _baseUrl + "/api/Genre/Create"
        });
    }
    public Task<T> UpdateGenreAsync<T>(GenreUpdateDTO genre, string? token = null)
    {
        return SendAsync<T>(new APIRequest()
        {
            ApiType = Constant.ApiType.PUT,
            Data = genre,
            Token = token,
            Url = _baseUrl + "/api/Genre/Update/" + genre.Id
        });
    }
    public Task<T> DeleteGenreAsync<T>(int id, string? token = null)
    {
        return SendAsync<T>(new APIRequest()
        {
            ApiType = Constant.ApiType.DELETE,
            Url = _baseUrl + $"/api/Genre/Delete/{id}",
            Token = token
        });
    }
}