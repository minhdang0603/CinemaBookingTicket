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
using Utility;

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

            await _unitOfWork.Screen.CreateAsync(screen);
            await _unitOfWork.SaveAsync();

            // Thêm ghế dựa trên Seats và SeatPerRow
            for (int row = 1; row <= dto.Rows; row++)
            {
                for (int seatNumber = 1; seatNumber <= screen.SeatsPerRow; seatNumber++)
                {
                    // Format ghế là "A1", "A2", ..., "B1", "B2", ...
                    // Giả sử hàng ghế được đánh số từ A, B, C, ...
                    var seatRow = (char)('A' + row - 1); // Chuyển đổi số hàng thành chữ cái
                    var seat = new Seat
                    {
                        ScreenId = screen.Id,
                        SeatRow = seatRow.ToString(),
                        SeatNumber = seatNumber,
                        IsActive = true,
                        CreatedAt = DateTime.Now,
                        LastUpdatedAt = DateTime.Now,
                        SeatType = await _unitOfWork.SeatType.GetAsync(st => st.Name == Constant.Seat_Type_Standard)
                    };

                    await _unitOfWork.Seat.CreateAsync(seat);
                }
            }
            await _unitOfWork.SaveAsync();
        }

        public async Task UpdateScreenAsync(int id, ScreenUpdateDTO dto)
        {

            // Lấy screen hiện tại kèm theo danh sách ghế
            var screen = await _unitOfWork.Screen.GetAsync(
                s => s.Id == id,
                includeProperties: "Seats");

            if (screen == null)
                throw new Exception($"Screen with ID {id} not found");

            // Cập nhật thông tin cơ bản của screen
            _mapper.Map(dto, screen);

            // Cập nhật screen 
            await _unitOfWork.Screen.UpdateAsync(screen);
            await _unitOfWork.SaveAsync();
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
            var screen = await _unitOfWork.Screen.GetAsync(s => s.Id == id && s.IsActive == isActive, includeProperties: "Seats,Theater");

            if (screen == null)
                throw new AppException(ErrorCodes.EntityNotFound("Screen", id));

            return _mapper.Map<ScreenDetailDTO>(screen);
        }

        public async Task<List<ScreenDTO>> GetAllScreensAsync(bool? isActive = true)
        {
            var screens = await _unitOfWork.Screen.GetAllAsync(
                filter: s => s.IsActive == isActive,
                includeProperties: "Theater");

            return _mapper.Map<List<ScreenDTO>>(screens);
        }

        public async Task<List<ScreenDTO>> GetAllScreensWithPaginationAsync(int pageNumber, int pageSize, bool? isActive = true)
        {
            var screens = await _unitOfWork.Screen.GetAllAsync(
                filter: s => s.IsActive == isActive,
                includeProperties: "Theater, Seats",
                pageNumber: pageNumber,
                pageSize: pageSize);

            return _mapper.Map<List<ScreenDTO>>(screens);
        }

        public async Task<List<SeatTypeDTO>> GetAllSeatTypesAsync()
        {
            var seatTypes = await _unitOfWork.SeatType.GetAllAsync();
            return _mapper.Map<List<SeatTypeDTO>>(seatTypes);
        }

        public async Task<List<SeatDTO>> GetSeatsByScreenIdAsync(int screenId)
        {
            var seats = await _unitOfWork.Seat.GetAllAsync(
                filter: s => s.ScreenId == screenId,
                includeProperties: "SeatType");

            return _mapper.Map<List<SeatDTO>>(seats);
        }
    }
}
