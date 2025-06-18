using System.Net;
using API.DTOs;
using API.DTOs.Request;
using API.DTOs.Response;
using API.Services.IServices;
using brevo_csharp.Client;
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
        public async Task<ActionResult<APIResponse<List<ShowTimeDTO>>>> GetAllShowTimesAsync()
        {
            var showTimes = await _showTimeService.GetAllShowTimesAsync();
            if (showTimes == null || !showTimes.Any())
            {
                return NotFound(APIResponse<List<ShowTimeDTO>>.Builder()
                    .WithStatusCode(HttpStatusCode.NotFound)
                    .WithSuccess(false)
                    .WithErrorMessages(new List<string> { "Showtimes is null or empty." })
                    .Build());
            }

            return Ok(APIResponse<List<ShowTimeDTO>>.Builder()
                .WithStatusCode(HttpStatusCode.OK)
                .WithSuccess(true)
                .WithResult(showTimes)
                .Build());
        }
        [HttpPost("add-showtimes")]
        public async Task<ActionResult<APIResponse<ShowTimeBulkResultDTO>>> AddShowTimes([FromBody] List<ShowTimeCreateDTO> newShowTimes)
        {
            if (newShowTimes == null || !newShowTimes.Any())
            {
                return BadRequest(APIResponse<ShowTimeBulkResultDTO>.Builder()
                    .WithStatusCode(HttpStatusCode.BadRequest)
                    .WithSuccess(false)
                    .WithErrorMessages(new List<string> { "Showtimes is null or empty." })
                    .Build());
            }

            var result = await _showTimeService.AddShowTimesAsync(newShowTimes);

            // Nếu có cả thành công và thất bại, trả về mã 207 Multi-Status
            if (result.SuccessfulShowTimes.Any() && result.FailedShowTimes.Any())
            {
                var message = $"Added {result.SuccessfulShowTimes.Count} showtimes successfully, {result.FailedShowTimes.Count} showtimes failed.";
                return StatusCode(207, APIResponse<ShowTimeBulkResultDTO>.Builder()
                    .WithStatusCode((HttpStatusCode)207) // Multi-Status
                    .WithSuccess(true)
                    .WithResult(result)
                    .WithErrorMessages(new List<string> { message })
                    .Build());
            }
            // Nếu tất cả đều thành công
            else if (result.SuccessfulShowTimes.Any() && !result.FailedShowTimes.Any())
            {
                var message = $"Added all {result.SuccessfulShowTimes.Count} showtimes successfully.";
                return Ok(APIResponse<ShowTimeBulkResultDTO>.Builder()
                    .WithStatusCode(HttpStatusCode.OK)
                    .WithSuccess(true)
                    .WithResult(result)
                    .Build());
            }
            // Nếu tất cả đều thất bại
            else
            {
                var failureMessages = result.FailedShowTimes.Select(f => f.ErrorMessage).ToList();
                return BadRequest(APIResponse<ShowTimeBulkResultDTO>.Builder()
                    .WithStatusCode(HttpStatusCode.BadRequest)
                    .WithSuccess(false)
                    .WithResult(result)
                    .WithErrorMessages(failureMessages)
                    .Build());
            }
        }

        [HttpPut("update-showtime/{id}")]
        public async Task<ActionResult<APIResponse<ShowTimeDTO>>> UpdateShowTime([FromRoute] int id, [FromBody] ShowTimeUpdateDTO dto)
        {
            if (dto == null || id <= 0)
            {
                return BadRequest(APIResponse<ShowTimeDTO>.Builder()
                    .WithStatusCode(HttpStatusCode.BadRequest)
                    .WithSuccess(false)
                    .WithErrorMessages(new List<string> { "Invalid showtime update data or ID." })
                    .Build());
            }

            var updatedShowtime = await _showTimeService.UpdateShowTimeAsync(id, dto);

            return Ok(APIResponse<ShowTimeDTO>.Builder()
                .WithStatusCode(HttpStatusCode.OK)
                .WithSuccess(true)
                .WithResult(updatedShowtime)
                .Build());
        }

        [HttpPut("delete-showtime/{id}")]
        public async Task<ActionResult<APIResponse<ShowTimeDTO>>> DeleteShowTime([FromRoute] int id)
        {
            if (id <= 0)
            {
                return BadRequest(APIResponse<ShowTimeDTO>.Builder()
                    .WithStatusCode(HttpStatusCode.BadRequest)
                    .WithSuccess(false)
                    .WithErrorMessages(new List<string> { "Invalid showtime ID." })
                    .Build());
            }
            var deletedShowtime = await _showTimeService.DeleteShowTimeAsync(id);

            return Ok(APIResponse<ShowTimeDTO>.Builder()
                .WithStatusCode(HttpStatusCode.OK)
                .WithSuccess(true)
                .WithResult(deletedShowtime)
                .Build());
        }
        [HttpGet("get-all-showtimes-with-pagination")]
        public async Task<ActionResult<APIResponse<List<ShowTimeDTO>>>> GetAllShowTimes(int pageNumber, int pageSize, bool? isActive = true)
        {
            if (pageNumber <= 0 || pageSize <= 0)
            {
                return BadRequest(APIResponse<List<ShowTimeDTO>>.Builder()
                    .WithStatusCode(HttpStatusCode.BadRequest)
                    .WithSuccess(false)
                    .WithErrorMessages(new List<string> { "Invalid pagination parameters." })
                    .Build());
            }

            var showTimes = await _showTimeService.GetAllShowTimesWithPaginationAsync(pageNumber, pageSize, isActive);
            if (showTimes == null || !showTimes.Any())
            {
                return NotFound(APIResponse<List<ShowTimeDTO>>.Builder()
                    .WithStatusCode(HttpStatusCode.NotFound)
                    .WithSuccess(false)
                    .WithErrorMessages(new List<string> { "Showtimes is empty." })
                    .Build());
            }

            return Ok(APIResponse<List<ShowTimeDTO>>.Builder()
                .WithStatusCode(HttpStatusCode.OK)
                .WithSuccess(true)
                .WithResult(showTimes)
                .Build());
        }
    }
}
