using API.Data;
using API.Data.Models;
using API.DTOs.Request;
using API.DTOs.Response;
using API.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories
{
    public class ShowTimeRepository : Repository<ShowTime>, IShowTimeRepository
    {
        private readonly ApplicationDbContext _context;

        public ShowTimeRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<ShowTimeDTO>> GetAllShowTimesAsync()
        {
            return await _context.ShowTimes
                .Select(s => new ShowTimeDTO
                {
                    Id = s.Id,
                    MovieId = s.MovieId,
                    ScreenId = s.ScreenId,
                    ShowDate = s.ShowDate,
                    StartTime = s.StartTime,
                    EndTime = s.EndTime,
                    BasePrice = s.BasePrice,
                    IsActive = s.IsActive
                }).ToListAsync();
        }

        public async Task<(List<ShowTimeDTO> Added, List<string> Errors)> AddShowTimesAsync(List<ShowTimeCreateDTO> newShowTimes)
        {
            var addedShowTimes = new List<ShowTime>();
            var errors = new List<string>();

            foreach (var dto in newShowTimes)
            {
                if (dto.StartTime >= dto.EndTime)
                {
                    errors.Add($"Giờ bắt đầu phải nhỏ hơn giờ kết thúc: {dto.StartTime} - {dto.EndTime}");
                    continue;
                }

                var conflict = await _context.ShowTimes
                    .Where(s => s.ScreenId == dto.ScreenId && s.ShowDate == dto.ShowDate && s.IsActive)
                    .ToListAsync();

                bool isOverlapping = conflict.Any(s =>
                    dto.StartTime < s.EndTime && dto.EndTime > s.StartTime
                );

                if (isOverlapping)
                {
                    errors.Add($"Trùng lịch chiếu: ScreenId={dto.ScreenId}, Ngày={dto.ShowDate}, Giờ={dto.StartTime}-{dto.EndTime}");
                    continue;
                }

                bool isTooClose = conflict.Any(s =>
                    Math.Abs((dto.StartTime - s.EndTime).TotalMinutes) < 30 ||
                    Math.Abs((dto.EndTime - s.StartTime).TotalMinutes) < 30
                );

                if (isTooClose)
                {
                    errors.Add($"Khoảng cách giữa các suất chiếu phải cách nhau ít nhất 30 phút: ScreenId={dto.ScreenId}, Ngày={dto.ShowDate}, Giờ={dto.StartTime}-{dto.EndTime}");
                    continue;
                }

                var showTime = new ShowTime
                {
                    MovieId = dto.MovieId,
                    ScreenId = dto.ScreenId,
                    ShowDate = dto.ShowDate,
                    StartTime = dto.StartTime,
                    EndTime = dto.EndTime,
                    BasePrice = dto.BasePrice,
                    IsActive = dto.IsActive,
                    CreatedAt = DateTime.Now,
                    LastUpdatedAt = DateTime.Now
                };

                addedShowTimes.Add(showTime);
            }

            if (addedShowTimes.Any())
            {
                await _context.ShowTimes.AddRangeAsync(addedShowTimes);
                await _context.SaveChangesAsync();
            }

            var result = addedShowTimes.Select(s => new ShowTimeDTO
            {
                Id = s.Id,
                MovieId = s.MovieId,
                ScreenId = s.ScreenId,
                ShowDate = s.ShowDate,
                StartTime = s.StartTime,
                EndTime = s.EndTime,
                BasePrice = s.BasePrice,
                IsActive = s.IsActive
            }).ToList();

            return (result, errors);
        }

        public async Task<(ShowTimeDTO? Updated, string? Error)> UpdateShowTimeAsync(int id, ShowTimeUpdateDTO dto)
        {
            var existing = await _context.ShowTimes.FindAsync(id);

            if (existing == null)
            {
                return (null, $"Không tìm thấy suất chiếu với ID = {id}");
            }

            if (dto.StartTime >= dto.EndTime)
            {
                return (null, $"Giờ bắt đầu phải nhỏ hơn giờ kết thúc: {dto.StartTime} - {dto.EndTime}");
            }

            var conflict = await _context.ShowTimes
                .Where(s => s.ScreenId == dto.ScreenId && s.ShowDate == dto.ShowDate && s.Id != id && s.IsActive)
                .ToListAsync();

            bool isOverlapping = conflict.Any(s =>
                dto.StartTime < s.EndTime && dto.EndTime > s.StartTime
            );

            if (isOverlapping)
            {
                return (null, $"Trùng lịch chiếu với suất khác: ScreenId={dto.ScreenId}, Ngày={dto.ShowDate}, Giờ={dto.StartTime}-{dto.EndTime}");
            }

            bool isTooClose = conflict.Any(s =>
                Math.Abs((dto.StartTime - s.EndTime).TotalMinutes) < 30 ||
                Math.Abs((dto.EndTime - s.StartTime).TotalMinutes) < 30
            );

            if (isTooClose)
            {
                return (null, $"Khoảng cách giữa các suất chiếu phải cách nhau ít nhất 30 phút: ScreenId={dto.ScreenId}, Ngày={dto.ShowDate}, Giờ={dto.StartTime}-{dto.EndTime}");
            }

            existing.MovieId = dto.MovieId;
            existing.ScreenId = dto.ScreenId;
            existing.ShowDate = dto.ShowDate;
            existing.StartTime = dto.StartTime;
            existing.EndTime = dto.EndTime;
            existing.BasePrice = dto.BasePrice;
            existing.IsActive = dto.IsActive;
            existing.LastUpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            var updatedDTO = new ShowTimeDTO
            {
                Id = existing.Id,
                MovieId = existing.MovieId,
                ScreenId = existing.ScreenId,
                ShowDate = existing.ShowDate,
                StartTime = existing.StartTime,
                EndTime = existing.EndTime,
                BasePrice = existing.BasePrice,
                IsActive = existing.IsActive
            };

            return (updatedDTO, null);
        }

        public async Task<bool> DeleteShowTimeAsync(int id)
        {
            var showTime = await _context.ShowTimes.FirstOrDefaultAsync(s => s.Id == id);

            if (showTime == null)
            {
                return false;
            }

            showTime.IsActive = false;
            showTime.LastUpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return true;
        }
    }
}
