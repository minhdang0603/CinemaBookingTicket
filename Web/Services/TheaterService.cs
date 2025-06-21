using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Utility;
using Web.Models;
using Web.Models.DTOs.Request;
using Web.Models.DTOs.Response;
using Web.Services.IServices;

namespace Web.Services;

public class TheaterService : BaseService, ITheaterService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _baseUrl;

    public TheaterService(IHttpClientFactory httpClientFactory, IConfiguration configuration) : base(httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
        _baseUrl = configuration.GetValue<string>("ServiceUrls:CinemaBookingTicketAPI") ?? string.Empty;
    }

    public Task<T> GetTheaterByIdAsync<T>(int id)
    {
        return SendAsync<T>(new APIRequest()
        {
            ApiType = Constant.ApiType.GET,
            Url = _baseUrl + $"/api/Theater/{id}"
        });
    }

    public Task<T> CreateTheaterAsync<T>(TheaterCreateDTO theater, string token)
    {
        return SendAsync<T>(new APIRequest()
        {
            ApiType = Constant.ApiType.POST,
            Data = theater,
            Token = token,
            Url = _baseUrl + "/api/Theater/add-theater-admin"
        });
    }

    public Task<T> UpdateTheaterAsync<T>(int id, TheaterUpdateDTO theater, string token)
    {
        return SendAsync<T>(new APIRequest()
        {
            ApiType = Constant.ApiType.PUT,
            Data = theater,
            Token = token,
            Url = _baseUrl + $"/api/Theater/update-theater-admin?id={id}"
        });
    }

    public Task<T> DeleteTheaterAsync<T>(int id, string token)
    {
        return SendAsync<T>(new APIRequest()
        {
            ApiType = Constant.ApiType.DELETE,
            Token = token,
            Url = _baseUrl + $"/api/Theater/delete-theater-admin?id={id}"
        });
    }

    public Task<T> GetAllTheatersAsync<T>()
    {
        return SendAsync<T>(new APIRequest()
        {
            ApiType = Constant.ApiType.GET,
            Url = _baseUrl + "/api/Theater"
        });
    }
}
