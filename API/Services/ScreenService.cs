using API.Data;
using API.Data.Models;
using API.DTOs.Request;
using API.DTOs.Response;
using API.Exceptions;
using API.Repositories.IRepositories;
using API.Services.IServices;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Services
{
    public class ScreenService : IScreenService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ScreenService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task AddScreenAsync(ScreenCreateDTO dto)
        {
            var screen = _mapper.Map<Screen>(dto);
            screen.CreatedAt = DateTime.UtcNow;
            screen.LastUpdatedAt = DateTime.UtcNow;
            screen.IsActive = true;

            await _unitOfWork.Screen.CreateAsync(screen);
            await _unitOfWork.SaveAsync();
        }

        public async Task UpdateScreenAsync(int id, ScreenUpdateDTO dto)
        {
            // Bắt đầu transaction để đảm bảo tính nhất quán dữ liệu
            using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                // Lấy screen hiện tại kèm theo danh sách ghế
                var screen = await _unitOfWork.Screen.GetAsync(
                    s => s.Id == id,
                    includeProperties: "Seats");

                if (screen == null)
                    throw new Exception($"Screen with ID {id} not found");

                // Cập nhật thông tin cơ bản của screen
                _mapper.Map(dto, screen);
                screen.LastUpdatedAt = DateTime.UtcNow;

                // Cập nhật screen
                await _unitOfWork.Screen.UpdateAsync(screen);

                // Cập nhật trạng thái của các ghế nếu có
                if (dto.Seats != null && dto.Seats.Any())
                {
                    foreach (var seatDto in dto.Seats)
                    {
                        var seat = screen.Seats.FirstOrDefault(s => s.Id == seatDto.Id);
                        if (seat != null)
                        {
                            // Cập nhật thông tin ghế
                            _mapper.Map(seatDto, seat);
                            seat.LastUpdatedAt = DateTime.UtcNow;
                        }
                    }
                }

                await _unitOfWork.SaveAsync();
                await _unitOfWork.CommitAsync();
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task DeleteScreenAsync(int id)
        {
            var screen = await _unitOfWork.Screen.GetAsync(s => s.Id == id);
            if (screen == null)
                throw new Exception($"Screen with ID {id} not found");

            screen.IsActive = false; // Đánh dấu là không hoạt động thay vì xóa vĩnh viễn
            screen.LastUpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Screen.UpdateAsync(screen);
            await _unitOfWork.SaveAsync();
        }

        public async Task<ScreenDetailDTO> GetScreenByIdAsync(int id, bool? isActive = true)
        {
            var screen = await _unitOfWork.Screen.GetAsync(s => s.Id == id && s.IsActive == isActive, includeProperties: "Seats");

            if (screen == null)
                throw new AppException(ErrorCodes.ScreenNotFound(id));

            return _mapper.Map<ScreenDetailDTO>(screen);
        }
    }
}
