using Utility;
using Web.Models;
using Web.Models.DTOs.Request;
using Web.Services.IServices;

namespace Web.Services;

public class ConcessionService : BaseService, IConcessionService
{

    private readonly IHttpClientFactory _httpClientFactory;
    private string _baseUrl = string.Empty;
    public ConcessionService(IHttpClientFactory httpClientFactory, IConfiguration configuration) : base(httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
        _baseUrl = configuration.GetValue<string>("ServiceUrls:CinemaBookingTicketAPI") ?? string.Empty;
    }
    public Task<T> GetAllConcessionsAsync<T>() => SendAsync<T>(new APIRequest()
    {
        ApiType = Constant.ApiType.GET,
        Url = _baseUrl + "/api/Concession/get-all-Concessions"
    });
    public Task<T> GetConcessionByIdAsync<T>(int id)
    {
        return SendAsync<T>(new APIRequest()
        {
            ApiType = Constant.ApiType.GET,
            Url = _baseUrl + $"/api/Concession/{id}"
        });
    }
    public Task<T> CreateConcessionAsync<T>(ConcessionCreateDTO concessionCreateDTO, string token)
    {
        return SendAsync<T>(new APIRequest()
        {
            ApiType = Constant.ApiType.POST,
            Data = concessionCreateDTO,
            Token = token,
            Url = _baseUrl + "/api/Concession/Create"
        });
    }
    public Task<T> UpdateConcessionAsync<T>(ConcessionUpdateDTO concessionUpdateDTO, string token)
    {
        return SendAsync<T>(new APIRequest()
        {
            ApiType = Constant.ApiType.PUT,
            Data = concessionUpdateDTO,
            Token = token,
            Url = _baseUrl + "/api/Concession/Update/" + concessionUpdateDTO.Id
        });
    }
    public Task<T> DeleteConcessionAsync<T>(int id, string token)
    {
        return SendAsync<T>(new APIRequest()
        {
            ApiType = Constant.ApiType.DELETE,
            Url = _baseUrl + $"/api/Concession/Delete/{id}",
            Token = token
        });
    }

}