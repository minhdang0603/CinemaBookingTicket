using System;
using Utility;
using Web.Models;
using Web.Models.DTOs.Request;
using Web.Models.DTOs.Response;
using Web.Services.IServices;

namespace Web.Services;

public class ScreenService : BaseService, IScreenService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private string _baseUrl = string.Empty;

    public ScreenService(IHttpClientFactory httpClientFactory, IConfiguration configuration) : base(httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
        _baseUrl = configuration.GetValue<string>("ServiceUrls:CinemaBookingTicketAPI") ?? string.Empty;
    }

    public Task<T> CreateScreenAsync<T>(ScreenCreateDTO screen, string token)
    {
        return SendAsync<T>(new APIRequest()
        {
            ApiType = Constant.ApiType.POST,
            Data = screen,
            Token = token,
            Url = _baseUrl + "/api/Screen/Create"
        });
    }

    public Task<T> DeleteScreenAsync<T>(int id, string token)
    {
        return SendAsync<T>(new APIRequest()
        {
            ApiType = Constant.ApiType.DELETE,
            Url = _baseUrl + $"/api/Screen/Delete/{id}",
            Token = token
        });
    }

    public Task<T> GetAllScreensAsync<T>()
    {
        return SendAsync<T>(new APIRequest()
        {
            ApiType = Constant.ApiType.GET,
            Url = _baseUrl + "/api/Screen/get-all-screens"
        });
    }

    public Task<List<T>> GetAllScreensWithPaginationAsync<T>(int pageNumber, int pageSize)
    {
        return SendAsync<List<T>>(new APIRequest()
        {
            ApiType = Constant.ApiType.GET,
            Url = _baseUrl + $"/api/Screen/get-all-screens-with-pagination?pageNumber={pageNumber}&pageSize={pageSize}"
        });
    }

    public Task<T> GetScreenByIdAsync<T>(int id)
    {
        return SendAsync<T>(new APIRequest()
        {
            ApiType = Constant.ApiType.GET,
            Url = _baseUrl + $"/api/Screen/{id}"
        });
    }

    public Task<T> UpdateScreenAsync<T>(ScreenUpdateDTO screen, string token)
    {
        return SendAsync<T>(new APIRequest()
        {
            ApiType = Constant.ApiType.PUT,
            Data = screen,
            Token = token,
            Url = _baseUrl + $"/api/Screen/Update/{screen.Id}"
        });
    }
}
