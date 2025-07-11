using Web.Models.DTOs.Request;
using Web.Models.DTOs.Response;

namespace Web.Services.IServices
{
    public interface IConcessionOrderService
    {
        Task<T> CreateConcessionOrderAsync<T>(ConcessionOrderCreateDTO concessionOrderCreateDTO, string? token = null);
        Task<T> GetConcessionOrderByIdAsync<T>(int concessionOrderId, string? token = null);
        Task<T> GetAllConcessionOrdersAsync<T>(string? token = null);
        Task<T> GetConcessionOrdersByBookingIdAsync<T>(int bookingId, string? token = null);
        Task<T> DeleteConcessionOrderAsync<T>(int concessionOrderId, string? token = null);
    }
}
