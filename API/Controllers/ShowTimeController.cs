using API.DTOs.Request;
using API.DTOs.Response;
using API.Services.IServices;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShowTimeController : ControllerBase
    {
        private readonly IShowTimeService _showTimeService;

        public ShowTimeController(IShowTimeService showTimeService)
        {
            _showTimeService = showTimeService;
        }

        [HttpGet("get-all-showtimes")]
        public async Task<ActionResult<List<ShowTimeDTO>>> GetAllShowTimesAsync()
        {
            var showTimes = await _showTimeService.GetAllShowTimesAsync();

            if (showTimes == null || !showTimes.Any())
            {
                return NotFound("Không có suất chiếu nào.");
            }

            return Ok(showTimes);
        }

        [HttpPost("add-showtimes")]
        public async Task<IActionResult> AddShowTimes([FromBody] List<ShowTimeCreateDTO> newShowTimes)
        {
            var (added, errors) = await _showTimeService.AddShowTimesAsync(newShowTimes);

            return Ok(new
            {
                Success = added.Any(),
                AddedCount = added.Count,
                Errors = errors
            });
        }

        [HttpPut("update-showtime/{id}")]
        public async Task<IActionResult> UpdateShowTime([FromRoute] int id, [FromBody] ShowTimeUpdateDTO dto)
        {
            var (updated, error) = await _showTimeService.UpdateShowTimeAsync(id, dto);

            if (error != null)
            {
                return BadRequest(new { Success = false, Message = error });
            }

            return Ok(new { Success = true, Updated = updated });
        }

        [HttpPut("delete-showtime/{id}")]
        public async Task<IActionResult> DeleteShowTime([FromRoute] int id)
        {
            var deleted = await _showTimeService.DeleteShowTimeAsync(id);

            if (!deleted)
            {
                return NotFound($"Không tìm thấy suất chiếu với Id = {id}");
            }

            return Ok($"Đã xoá (ẩn) suất chiếu Id = {id} thành công.");
        }

    }
}
