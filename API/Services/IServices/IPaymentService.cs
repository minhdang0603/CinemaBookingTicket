using API.Data.Models;
using API.DTOs.Request;
using API.DTOs.Response;

namespace API.Services.IServices;

public interface IPaymentService
{
    // VNPay methods
    Task<string> CreateVNPayPaymentUrl(VNPayRequestDTO request);
    Task<VNPayResponseDTO> ProcessVNPayReturnAsync(Dictionary<string, string> queryParams);
    Task<VNPayRefundResponseDTO> Refund(VNPayRefundRequestDTO request);

    // Common methods
    Task<PaymentDTO> GetPaymentsByBookingIdAsync(int bookingId);
    Task<PaymentDTO> UpdatePaymentStatusAsync(int paymentId, string status, string? transactionId = null);

    Task<PaymentDTO> CreatePaymentAsync(int bookingId);
}