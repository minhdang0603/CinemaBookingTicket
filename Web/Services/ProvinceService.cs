using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Utility;
using Web.Models;
using Web.Models.DTOs.Request;
using Web.Models.DTOs.Response;
using Web.Services.IServices;

namespace Web.Services
{
    public class ProvinceService : BaseService, IProvinceService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _baseUrl;

        public ProvinceService(IHttpClientFactory httpClientFactory, IConfiguration configuration) : base(httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            _baseUrl = configuration.GetValue<string>("ServiceUrls:CinemaBookingTicketAPI") ?? string.Empty;
        }

        public Task<T> GetAllProvincesAsync<T>()
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = Constant.ApiType.GET,
                Url = _baseUrl + "/api/Province/get-all-provinces"
            });
        }
    }
}