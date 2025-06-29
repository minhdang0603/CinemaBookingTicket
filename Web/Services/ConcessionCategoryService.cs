using Utility;
using Web.Models;
using Web.Models.DTOs.Request;
using Web.Services.IServices;

namespace Web.Services;

public class ConcessionCategoryService : BaseService, IConcessionCategoryService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private string _baseUrl = string.Empty;
    public ConcessionCategoryService(IHttpClientFactory httpClientFactory, IConfiguration configuration) : base(httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
        _baseUrl = configuration.GetValue<string>("ServiceUrls:CinemaBookingTicketAPI") ?? string.Empty;
    }
    public Task<T> GetAllConcessionCategoriesAsync<T>() => SendAsync<T>(new APIRequest()
    {
        ApiType = Constant.ApiType.GET,
        Url = _baseUrl + "/api/ConcessionCategory/get-all-concession-categories"
    });
    public Task<T> GetConcessionCategoryByIdAsync<T>(int id)
    {
        return SendAsync<T>(new APIRequest()
        {
            ApiType = Constant.ApiType.GET,
            Url = _baseUrl + $"/api/ConcessionCategory/{id}"
        });
    }
    public Task<T> CreateConcessionCategoryAsync<T>(ConcessionCategoryCreateDTO concessionCategoryCreateDTO, string token)
    {
        return SendAsync<T>(new APIRequest()
        {
            ApiType = Constant.ApiType.POST,
            Data = concessionCategoryCreateDTO,
            Token = token,
            Url = _baseUrl + "/api/ConcessionCategory/create"
        });
    }
    public Task<T> UpdateConcessionCategoryAsync<T>(ConcessionCategoryUpdateDTO concessionCategoryUpdateDTO, string token)
    {
        return SendAsync<T>(new APIRequest()
        {
            ApiType = Constant.ApiType.PUT,
            Data = concessionCategoryUpdateDTO,
            Token = token,
            Url = _baseUrl + "/api/ConcessionCategory/update/" + concessionCategoryUpdateDTO.Id
        });
    }
    public Task<T> DeleteConcessionCategoryAsync<T>(int id, string token)
    {
        return SendAsync<T>(new APIRequest()
        {
            ApiType = Constant.ApiType.DELETE,
            Url = _baseUrl + $"/api/ConcessionCategory/delete/{id}",
            Token = token
        });
    }
}