using API.Data.Models;
using API.DTOs;
using API.DTOs.Request;
using API.DTOs.Response;
using API.Services.IServices;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
    public class ProvinceService : IProvinceService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ProvinceService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<ProvinceDTO>> GetAllProvincesAsync()
        {
            var provinces = await _context.Provinces.ToListAsync();
            return _mapper.Map<List<ProvinceDTO>>(provinces);
        }

        public async Task<List<ProvinceDTO>> GetAllProvincesWithPaginationAsync(int pageNumber, int pageSize, bool? isActive = true)
        {
            var query = _context.Provinces.AsQueryable();

            if (isActive != null)
                query = query.Where(p => p.IsActive == isActive);

            var provinces = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return _mapper.Map<List<ProvinceDTO>>(provinces);
        }

        public async Task<ProvinceDTO> GetProvinceByIdAsync(int id)
        {
            var province = await _context.Provinces.FindAsync(id);
            return _mapper.Map<ProvinceDTO>(province);
        }

        public async Task CreateProvinceAsync(ProvinceCreateDTO provinceCreateDTO)
        {
            var province = _mapper.Map<Province>(provinceCreateDTO);
            await _context.Provinces.AddAsync(province);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateProvinceAsync(int id, ProvinceUpdateDTO provinceUpdateDTO)
        {
            var province = await _context.Provinces.FindAsync(id);
            if (province != null)
            {
                _mapper.Map(provinceUpdateDTO, province);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteProvinceAsync(int id)
        {
            var province = await _context.Provinces.FindAsync(id);
            if (province != null)
            {
                _context.Provinces.Remove(province);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<ProvinceDTO>> SearchProvincesAsync(string name)
        {
            var provinces = await _context.Provinces
                .Where(p => p.Name.ToLower().Contains(name.ToLower()))
                .ToListAsync();

            return _mapper.Map<List<ProvinceDTO>>(provinces);
        }
    }
}
