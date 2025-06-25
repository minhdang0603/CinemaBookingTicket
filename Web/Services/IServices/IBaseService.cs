using Web.Models;

namespace Web.Services.IServices
{
    public interface IBaseService
    {
        Task<T> SendAsync<T>(APIRequest apiRequest);
    }
}
