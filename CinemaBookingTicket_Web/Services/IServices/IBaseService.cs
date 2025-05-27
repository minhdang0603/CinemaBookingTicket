using CinemaBookingTicket_Web.Models;
using MagicVilla_Web.Models;

namespace CinemaBookingTicket_Web.Services.IServices
{
    public interface IBaseService
    {
        APIResponse responseModel { get; set; }
        Task<T> SendAsync<T>(APIRequest apiRequest);

    }
}
