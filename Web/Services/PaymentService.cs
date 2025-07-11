using System.Net;
using Microsoft.Extensions.Configuration;
using Web.Models;
using Web.Models.DTOs.Request;
using Web.Models.DTOs.Response;
using Web.Services.IServices;
using Utility;

namespace Web.Services
{
    public class PaymentService : BaseService, IPaymentService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _baseUrl;

        public PaymentService(IHttpClientFactory httpClientFactory, IConfiguration configuration) : base(httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            _baseUrl = configuration.GetValue<string>("ServiceUrls:CinemaBookingTicketAPI") ?? string.Empty;
        }

        // POST /api/payment/create-vnpay-payment (Authorize)
        public Task<T> CreateVNPayPaymentAsync<T>(VNPayRequestDTO request, string token)
        {
            return SendAsync<T>(new APIRequest
            {
                ApiType = Constant.ApiType.POST,
                Url = $"{_baseUrl}/api/payment/create-vnpay-payment",
                Data = request,
                Token = token
            });
        }

        // GET /api/payment/vnpay-check (AllowAnonymous)
        public Task<T> VNPayCheckAsync<T>(string queryString)
        {
			return SendAsync<T>(new APIRequest
            {
                ApiType = Constant.ApiType.GET,
                Url = $"{_baseUrl}/api/payment/vnpay-check{queryString}",
            });
        }

        // GET /api/payment/by-booking/{bookingId} (Authorize)
        public Task<T> GetPaymentByBookingIdAsync<T>(int bookingId, string token)
        {
            return SendAsync<T>(new APIRequest
            {
                ApiType = Constant.ApiType.GET,
                Url = $"{_baseUrl}/api/payment/by-booking/{bookingId}",
                Token = token
            });
        }

        // GET /api/payment/payment-status/{paymentId} (AllowAnonymous)
        public Task<T> GetPaymentStatusAsync<T>(string paymentId)
        {
            return SendAsync<T>(new APIRequest
            {
                ApiType = Constant.ApiType.GET,
                Url = $"{_baseUrl}/api/payment/payment-status/{paymentId}"
            });
        }
    }
}
