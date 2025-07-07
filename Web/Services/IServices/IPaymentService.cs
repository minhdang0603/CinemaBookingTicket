using Web.Models.DTOs.Request;
using Web.Models.DTOs.Response;

namespace Web.Services.IServices
{
    public interface IPaymentService
    {
        Task<string> CreateVNPayPaymentAsync(VNPayRequestDTO request, string token);
        Task<VNPayResponseDTO> VNPayCheckAsync(string queryString);
        Task<PaymentDTO> GetPaymentByBookingIdAsync(int bookingId, string token);
        Task<PaymentDTO> GetPaymentStatusAsync(string paymentId);
    }
}
