using Web.Models.DTOs.Request;
using Web.Models.DTOs.Response;

namespace Web.Services.IServices
{
    public interface IProvinceService
    {
        Task<T> GetAllProvincesAsync<T>();
        Task<T> GetProvinceByIdAsync<T>(int id);
        Task<T> CreateProvinceAsync<T>(ProvinceCreateDTO dto, string token);
        Task<T> UpdateProvinceAsync<T>(int id, ProvinceUpdateDTO dto, string token);
        Task<T> DeleteProvinceAsync<T>(int id, string token);
        Task<T> SearchProvincesAsync<T>(string name);
    }
}
