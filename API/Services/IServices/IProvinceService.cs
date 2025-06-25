using API.DTOs;
using API.DTOs.Request;
using API.DTOs.Response;

namespace API.Services.IServices
{
    public interface IProvinceService
    {
        Task<List<ProvinceDTO>> GetAllProvincesAsync(bool? isActive = true);
        Task<ProvinceDetailDTO> GetProvinceByIdAsync(int id, bool? isActive = true);
        Task<List<ProvinceDTO>> GetAllProvincesWithPaginationAsync(int pageNumber, int pageSize, bool? isActive = true);
        Task CreateProvinceAsync(ProvinceCreateDTO provinceCreateDTO);
        Task UpdateProvinceAsync(int id, ProvinceUpdateDTO provinceUpdateDTO);
        Task DeleteProvinceAsync(int id);
        Task<List<ProvinceDTO>> SearchProvincesAsync(string name, bool? isActive = true);
    }
}
