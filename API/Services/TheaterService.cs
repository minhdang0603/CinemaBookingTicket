using API.Data;
using API.Data.Models;
using API.DTOs.Request;
using API.DTOs.Response;
using API.Exceptions;
using API.Repositories.IRepositories;
using API.Services.IServices;
using AutoMapper;
using System;
using System.Threading.Tasks;

namespace API.Services
{
    public class TheaterService : ITheaterService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TheaterService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task AddTheaterAsync(TheaterCreateDTO dto)
        {

            // Kiểm tra xem theater đã tồn tại hay chưa
            var existingTheater = await _unitOfWork.Theater.GetAsync(t => t.Name == dto.Name && t.IsActive);

            if (existingTheater != null)
                throw new AppException(ErrorCodes.TheaterAlreadyExists(dto.Name));

            var theater = _mapper.Map<Theater>(dto);

            await _unitOfWork.Theater.CreateAsync(theater);
            await _unitOfWork.SaveAsync();
        }

        public async Task UpdateTheaterAsync(int id, TheaterUpdateDTO dto)
        {
            var theater = await _unitOfWork.Theater.GetAsync(t => t.Id == id);
            if (theater == null)
                throw new Exception($"Theater with ID {id} not found");

            _mapper.Map(dto, theater);

            await _unitOfWork.Theater.UpdateAsync(theater);
            await _unitOfWork.SaveAsync();
        }

        public async Task DeleteTheaterAsync(int id)
        {
            var theater = await _unitOfWork.Theater.GetAsync(t => t.Id == id);
            if (theater == null)
                throw new Exception($"Theater with ID {id} not found");

            theater.IsActive = false; // Đánh dấu là không hoạt động
            theater.LastUpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Theater.UpdateAsync(theater);
            await _unitOfWork.SaveAsync();
        }

        public async Task<List<TheaterDTO>> GetAllTheatersAsync(bool? isActive = true)
        {
            var theaters = await _unitOfWork.Theater.GetAllAsync(t => t.IsActive == isActive, includeProperties: "Province");
            return _mapper.Map<List<TheaterDTO>>(theaters);
        }

        public async Task<TheaterDetailDTO> GetTheaterByIdAsync(int id, bool? isActive = true)
        {
            var theater = await _unitOfWork.Theater.GetAsync(t => t.Id == id && t.IsActive == isActive);

            if (theater == null)
                throw new AppException(ErrorCodes.TheaterNotFound(id));

            return _mapper.Map<TheaterDetailDTO>(theater);
        }

        public async Task<List<TheaterDTO>> GetAllTheatersWithPaginationAsync(int pageNumber, int pageSize, bool? isActive = true)
        {
            var theaters = await _unitOfWork.Theater.GetAllAsync(
                t => t.IsActive == isActive,
                pageNumber: pageNumber,
                pageSize: pageSize);

            return _mapper.Map<List<TheaterDTO>>(theaters);
        }
    }
}
