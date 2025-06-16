using API.Data;
using API.Data.Models;
using API.DTOs.Request;
using API.Repositories.IRepositories;
using API.Services.IServices;
using AutoMapper;
using System;
using System.Threading.Tasks;

namespace API.Services
{
    public class ScreenService : IScreenService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _context;

        public ScreenService(IUnitOfWork unitOfWork, IMapper mapper, ApplicationDbContext context)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _context = context;
        }

        public async Task AddScreenAsync(ScreenCreateDTO dto)
        {
            var screen = _mapper.Map<Screen>(dto);
            screen.CreatedAt = DateTime.UtcNow;
            screen.LastUpdatedAt = DateTime.UtcNow;
            screen.IsActive = true;

            await _unitOfWork.Screen.AddAsync(screen);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateScreenAsync(int id, ScreenUpdateDTO dto)
        {
            var screen = await _unitOfWork.Screen.GetAsync(id);
            if (screen == null)
                throw new Exception($"Screen with ID {id} not found");

            _mapper.Map(dto, screen);
            screen.LastUpdatedAt = DateTime.UtcNow;

            _unitOfWork.Screen.Update(screen);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteScreenAsync(int id)
        {
            var screen = await _unitOfWork.Screen.GetAsync(id);
            if (screen == null)
                throw new Exception($"Screen with ID {id} not found");

            _unitOfWork.Screen.Remove(screen);
            await _context.SaveChangesAsync();
        }
    }
}
