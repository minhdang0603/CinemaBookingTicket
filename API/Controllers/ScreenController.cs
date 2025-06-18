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

        [HttpPost("add-room-admin")]
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

            try
            {
                await _screenService.AddScreenAsync(screenCreateDTO);
                return Ok(APIResponse<string>.Builder()
                    .WithResult($"Room {screenCreateDTO.Name} created successfully.")
                    .WithStatusCode(HttpStatusCode.OK)
                    .Build());
            }
            catch (Exception ex)
            {
                return StatusCode(500, APIResponse<string>.Builder()
                    .WithErrorMessages(new List<string> { $"Error creating room: {ex.Message}" })
                    .WithStatusCode(HttpStatusCode.InternalServerError)
                    .WithSuccess(false)
                    .Build());
            }
        }

        [HttpPut("update-room-admin")]
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

            try
            {
                await _screenService.UpdateScreenAsync(id, screenUpdateDTO);
                return Ok(APIResponse<string>.Builder()
                    .WithResult($"Room {screenUpdateDTO.Name} and {screenUpdateDTO.Seats?.Count ?? 0} seats updated successfully.")
                    .WithStatusCode(HttpStatusCode.OK)
                    .Build());
            }
            catch (Exception ex)
            {
                return StatusCode(500, APIResponse<string>.Builder()
                    .WithErrorMessages(new List<string> { $"Error updating room: {ex.Message}" })
                    .WithStatusCode(HttpStatusCode.InternalServerError)
                    .WithSuccess(false)
                    .Build());
            }
        }

        [HttpDelete("delete-room-admin")]
        [Authorize(Roles = Constant.Role_Admin)]
        public async Task<ActionResult<APIResponse<string>>> DeleteRoomAsync(int id)
        {
            if (id == 0)
            {
                return BadRequest(APIResponse<string>.Builder()
                    .WithErrorMessages(new List<string> { "Room Id is null." })
                    .WithStatusCode(HttpStatusCode.BadRequest)
                    .WithSuccess(false)
                    .Build());
            }

            try
            {
                await _screenService.DeleteScreenAsync(id);
                return Ok(APIResponse<string>.Builder()
                    .WithResult($"Room {id} deleted successfully.")
                    .WithStatusCode(HttpStatusCode.OK)
                    .Build());
            }
            catch (Exception ex)
            {
                return StatusCode(500, APIResponse<string>.Builder()
                    .WithErrorMessages(new List<string> { $"Error deleting room: {ex.Message}" })
                    .WithStatusCode(HttpStatusCode.InternalServerError)
                    .WithSuccess(false)
                    .Build());
            }
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
    }
}
