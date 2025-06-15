using API.Data;
using API.Data.Models;
using API.DTOs.Request;
using API.Repositories.IRepositories;
using API.Services.IServices;
using System;
using System.Threading.Tasks;

namespace API.Services
{
    public class TheaterService : ITheaterService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _context;

        public TheaterService(IUnitOfWork unitOfWork, ApplicationDbContext context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public async Task AddTheaterAsync(TheaterCreateDTO dto)
        {
            var theater = new Theater
            {
                Name = dto.Name,
                Address = dto.Address,
                Description = dto.Description,
                ProvinceId = dto.ProvinceId,
                IsActive = dto.IsActive,
                CreatedAt = DateTime.UtcNow,
                LastUpdatedAt = DateTime.UtcNow,

                // Convert DateTime? => TimeOnly?
                OpeningTime = dto.OpeningTime.HasValue ? TimeOnly.FromDateTime(dto.OpeningTime.Value) : null,
                ClosingTime = dto.ClosingTime.HasValue ? TimeOnly.FromDateTime(dto.ClosingTime.Value) : null
            };

            await _unitOfWork.Theater.AddAsync(theater);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateTheaterAsync(int id, TheaterUpdateDTO dto)
        {
            var theater = await _unitOfWork.Theater.GetAsync(id);
            if (theater == null)
                throw new Exception($"Theater with ID {id} not found");

            theater.Name = dto.Name;
            theater.Address = dto.Address;
            theater.Description = dto.Description;
            theater.ProvinceId = dto.ProvinceId;
            theater.IsActive = dto.IsActive;
            theater.LastUpdatedAt = DateTime.UtcNow;

            theater.OpeningTime = dto.OpeningTime.HasValue ? TimeOnly.FromDateTime(dto.OpeningTime.Value) : null;
            theater.ClosingTime = dto.ClosingTime.HasValue ? TimeOnly.FromDateTime(dto.ClosingTime.Value) : null;

            _unitOfWork.Theater.Update(theater);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteTheaterAsync(int id)
        {
            var theater = await _unitOfWork.Theater.GetAsync(id);
            if (theater == null)
                throw new Exception($"Theater with ID {id} not found");

            _unitOfWork.Theater.Remove(theater);
            await _context.SaveChangesAsync();
        }
    }
}
