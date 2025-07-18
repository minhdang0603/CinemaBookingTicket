using Web.Models.DTOs.Request;
using Web.Models.DTOs.Response;

namespace Web.Services.IServices
{
	public interface IBookingService
	{
		Task<T> CreateBookingAsync<T>(BookingCreateDTO bookingCreateDTO, string token);
		Task<T> UpdateBookingAsync<T>(BookingUpdateDTO bookingUpdateDTO, string token);
		Task<T> DeleteBookingAsync<T>(int bookingId, string token);
		Task<T> GetMyBookingsAsync<T>(string? token = null);
		Task<T> GetBookingByIdAsync<T>(int bookingId, string? token = null);
		Task<T> GetAllBookingsAsync<T>(string? token = null);
		Task<T> CancelBookingAsync<T>(int bookingId, string? token = null);
	}
}
