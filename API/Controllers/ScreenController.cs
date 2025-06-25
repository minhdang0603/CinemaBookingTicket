using API.DTOs.Request;
using API.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using API.DTOs;
using Microsoft.AspNetCore.Authorization;
using Utility;
using API.DTOs.Response;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScreenController : ControllerBase
    {
        private readonly IScreenService _screenService;

        public ScreenController(IScreenService screenService)
        {
            _screenService = screenService;
        }

        [HttpGet("get-all-screens")]
        public async Task<ActionResult<APIResponse<List<ScreenDTO>>>> GetAllScreensAsync()
        {
            var screens = await _screenService.GetAllScreensAsync();
            return Ok(APIResponse<List<ScreenDTO>>.Builder()
                .WithResult(screens)
                .WithStatusCode(HttpStatusCode.OK)
                .Build());
        }

        [HttpGet("get-all-screens-with-pagination")]
        public async Task<ActionResult<APIResponse<List<ScreenDTO>>>> GetAllScreensWithPaginationAsync(int pageNumber = 1, int pageSize = 10)
        {
            var screens = await _screenService.GetAllScreensWithPaginationAsync(pageNumber, pageSize);
            return Ok(APIResponse<List<ScreenDTO>>.Builder()
                .WithResult(screens)
                .WithStatusCode(HttpStatusCode.OK)
                .Build());
        }

        [HttpPost("create")]
        [Authorize(Roles = Constant.Role_Admin)]
        public async Task<ActionResult<APIResponse<string>>> AddRoomAsync([FromBody] ScreenCreateDTO screenCreateDTO)
        {
            if (screenCreateDTO == null)
            {
                return BadRequest(APIResponse<string>.Builder()
                    .WithErrorMessages(new List<string> { "Room data is null." })
                    .WithStatusCode(HttpStatusCode.BadRequest)
                    .WithSuccess(false)
                    .Build());
            }

            await _screenService.AddScreenAsync(screenCreateDTO);
            return Ok(APIResponse<string>.Builder()
                .WithResult($"Room {screenCreateDTO.Name} created successfully.")
                .WithStatusCode(HttpStatusCode.OK)
                .Build());
        }

        [HttpPut("update/{id:int}")]
        [Authorize(Roles = Constant.Role_Admin)]
        public async Task<ActionResult<APIResponse<string>>> UpdateRoomAsync(int id, [FromBody] ScreenUpdateDTO screenUpdateDTO)
        {
            if (id == 0 || screenUpdateDTO == null)
            {
                return BadRequest(APIResponse<string>.Builder()
                    .WithErrorMessages(new List<string> { "Room Id or update data is null." })
                    .WithStatusCode(HttpStatusCode.BadRequest)
                    .WithSuccess(false)
                    .Build());
            }

            await _screenService.UpdateScreenAsync(id, screenUpdateDTO);
            return Ok(APIResponse<string>.Builder()
                .WithResult($"Room {screenUpdateDTO.Name} and {screenUpdateDTO.Seats?.Count ?? 0} seats updated successfully.")
                .WithStatusCode(HttpStatusCode.OK)
                .Build());
        }

        [HttpDelete("delete/{id:int}")]
        [Authorize(Roles = Constant.Role_Admin)]
        public async Task<ActionResult<APIResponse<string>>> DeleteRoomAsync([FromRoute] int id)
        {
            if (id == 0)
            {
                return BadRequest(APIResponse<string>.Builder()
                    .WithErrorMessages(new List<string> { "Room Id is null." })
                    .WithStatusCode(HttpStatusCode.BadRequest)
                    .WithSuccess(false)
                    .Build());
            }

            await _screenService.DeleteScreenAsync(id);
            return Ok(APIResponse<string>.Builder()
                .WithResult($"Room {id} deleted successfully.")
                .WithStatusCode(HttpStatusCode.OK)
                .Build());

        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<APIResponse<ScreenDetailDTO>>> GetScreenByIdAsync(int id)
        {
            if (id <= 0)
            {
                return BadRequest(APIResponse<ScreenDetailDTO>.Builder()
                    .WithErrorMessages(new List<string> { "Invalid screen ID." })
                    .WithStatusCode(HttpStatusCode.BadRequest)
                    .Build());
            }

            var screen = await _screenService.GetScreenByIdAsync(id);

            return Ok(APIResponse<ScreenDetailDTO>.Builder()
                .WithResult(screen)
                .WithStatusCode(HttpStatusCode.OK)
                .Build());
        }

        [HttpGet("get-all-seat-types")]
        public async Task<ActionResult<APIResponse<List<SeatTypeDTO>>>> GetAllSeatTypesAsync()
        {
            var seatTypes = await _screenService.GetAllSeatTypesAsync();
            return Ok(APIResponse<List<SeatTypeDTO>>.Builder()
                .WithResult(seatTypes)
                .WithStatusCode(HttpStatusCode.OK)
                .Build());
        }

        [HttpGet("get-seats-by-screen/{id:int}")]
        public async Task<ActionResult<APIResponse<List<SeatDTO>>>> GetSeatsByScreenAsync(int id)
        {
            if (id <= 0)
            {
                return BadRequest(APIResponse<List<SeatDTO>>.Builder()
                    .WithErrorMessages(new List<string> { "Invalid screen ID." })
                    .WithStatusCode(HttpStatusCode.BadRequest)
                    .Build());
            }

            var seats = await _screenService.GetSeatsByScreenIdAsync(id);
            return Ok(APIResponse<List<SeatDTO>>.Builder()
                .WithResult(seats)
                .WithStatusCode(HttpStatusCode.OK)
                .Build());
        }
    }
}
