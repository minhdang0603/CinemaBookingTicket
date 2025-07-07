using API.DTOs.Response;
using API.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using API.DTOs;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShowTimeSeatController : ControllerBase
    {
        private readonly IShowTimeService _showTimeService;
        private readonly ILogger<ShowTimeSeatController> _logger;

        public ShowTimeSeatController(IShowTimeService showTimeService, ILogger<ShowTimeSeatController> logger)
        {
            _showTimeService = showTimeService;
            _logger = logger;
        }

        [HttpGet("{showTimeId}")]
        public async Task<ActionResult<APIResponse<ShowTimeSeatStatusDTO>>> GetShowTimeSeatStatus(int showTimeId)
        {
            _logger.LogInformation($"Fetching seat status for showtime ID: {showTimeId}");

            var seatStatus = await _showTimeService.GetShowTimeSeatStatusAsync(showTimeId);

            return Ok(APIResponse<ShowTimeSeatStatusDTO>.Builder()
                .WithStatusCode(HttpStatusCode.OK)
                .WithSuccess(true)
                .WithResult(seatStatus)
                .Build());
        }
    }
}
