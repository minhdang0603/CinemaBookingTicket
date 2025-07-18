using API.DTOs.Request;
using API.DTOs.Response;

namespace API.Services.IServices;

public interface IConcessionOrderService
{
    Task<ConcessionOrderDTO> CreateConcessionOrderAsync(ConcessionOrderCreateDTO concessionOrderCreateDTO);
    Task<ConcessionOrderDTO> GetConcessionOrderByIdAsync(int concessionOrderId);
    Task<List<ConcessionOrderDTO>> GetAllConcessionOrdersAsync();
    Task<List<ConcessionOrderDTO>> GetConcessionOrdersByBookingIdAsync(int bookingId);
    Task DeleteConcessionOrderAsync(int concessionOrderId);

}
