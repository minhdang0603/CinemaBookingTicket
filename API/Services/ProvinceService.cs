using API.Data.Models;
using API.DTOs;
using API.DTOs.Request;
using API.DTOs.Response;
using API.Repositories.IRepositories;
using API.Services.IServices;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
    public class ProvinceService : IProvinceService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProvinceService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<ProvinceDTO>> GetAllProvincesAsync(bool? isActive = true)
        {
            var provinces = await _unitOfWork.Province.GetAllAsync(p => p.IsActive == isActive);

            return _mapper.Map<List<ProvinceDTO>>(provinces);
        }

        public async Task<List<ProvinceDTO>> GetAllProvincesWithPaginationAsync(int pageNumber, int pageSize, bool? isActive = true)
        {
            var provinces = await _unitOfWork.Province.GetAllAsync(
                p => p.IsActive == isActive,
                pageNumber: pageNumber,
                pageSize: pageSize);

            return _mapper.Map<List<ProvinceDTO>>(provinces);
        }

        public async Task<ProvinceDetailDTO> GetProvinceByIdAsync(int id, bool? isActive = true)
        {
            var province = await _unitOfWork.Province.GetAsync(p => p.Id == id && p.IsActive == isActive);
            return _mapper.Map<ProvinceDetailDTO>(province);
        }

        public async Task CreateProvinceAsync(ProvinceCreateDTO provinceCreateDTO)
        {
            var province = _mapper.Map<Province>(provinceCreateDTO);
            await _unitOfWork.Province.CreateAsync(province);
            await _unitOfWork.SaveAsync();
        }

        public async Task UpdateProvinceAsync(int id, ProvinceUpdateDTO provinceUpdateDTO)
        {
            var province = await _unitOfWork.Province.GetAsync(p => p.Id == id);
            if (province != null)
            {
                _mapper.Map(provinceUpdateDTO, province);
                await _unitOfWork.Province.UpdateAsync(province);
                await _unitOfWork.SaveAsync();
            }
        }

        public async Task DeleteProvinceAsync(int id)
        {
            var province = await _unitOfWork.Province.GetAsync(p => p.Id == id);
            if (province == null)
            {
                throw new Exception($"Province with ID {id} not found");
            }

            province.IsActive = false; // Thay vì xóa, đánh dấu là không hoạt động

            await _unitOfWork.Province.UpdateAsync(province);
            await _unitOfWork.SaveAsync();
        }

        public async Task<List<ProvinceDTO>> SearchProvincesAsync(string name, bool? isActive = true)
        {
            var provinces = await _unitOfWork.Province.GetAllAsync(p => p.Name.ToLower().Contains(name.ToLower()) && p.IsActive == isActive);

            return _mapper.Map<List<ProvinceDTO>>(provinces);
        }
    }
}
