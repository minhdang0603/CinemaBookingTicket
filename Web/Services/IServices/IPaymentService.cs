using Web.Models.DTOs.Request;
using Web.Models.DTOs.Response;

namespace Web.Services.IServices
{
    public interface IPaymentService
    {
        Task<T> CreateVNPayPaymentAsync<T>(VNPayRequestDTO request, string token);
        Task<T> VNPayCheckAsync<T>(string queryString);
        Task<T> GetPaymentByBookingIdAsync<T>(int bookingId, string token);
        Task<T> GetPaymentStatusAsync<T>(string paymentId);
    }
}
