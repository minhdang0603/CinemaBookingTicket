using API.DTOs.Request;
using API.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using API.DTOs;

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
        public async Task<ActionResult<APIResponse<string>>> AddRoomAsync([FromBody] ScreenCreateDTO screenCreateDTO)
        {
            await _screenService.AddScreenAsync(screenCreateDTO);

            return Ok(APIResponse<string>.Builder()
                .WithResult($"Room {screenCreateDTO.Name} created successfully.")
                .WithStatusCode(HttpStatusCode.OK)
                .Build());
        }

        [HttpPut("update-room-admin")]
        public async Task<ActionResult<APIResponse<string>>> UpdateRoomAsync(int id, [FromBody] ScreenUpdateDTO screenUpdateDTO)
        {
            if (id == 0 || screenUpdateDTO == null)
            {
                return BadRequest(APIResponse<string>.Builder()
                    .WithErrorMessages("Room Id or update data is null.")
                    .WithStatusCode(HttpStatusCode.BadRequest)
                    .Build());
            }

            await _screenService.UpdateScreenAsync(id, screenUpdateDTO);
            return Ok(APIResponse<string>.Builder()
                .WithResult($"Room {screenUpdateDTO.Name} updated successfully.")
                .WithStatusCode(HttpStatusCode.OK)
                .Build());
        }

        [HttpDelete("delete-room-admin")]
        public async Task<ActionResult<APIResponse<string>>> DeleteRoomAsync(int id)
        {
            if (id == 0)
            {
                return BadRequest(APIResponse<string>.Builder()
                    .WithErrorMessages("Room Id is null.")
                    .WithStatusCode(HttpStatusCode.BadRequest)
                    .Build());
            }

            await _screenService.DeleteScreenAsync(id);
            return Ok(APIResponse<string>.Builder()
                .WithResult($"Room {id} deleted successfully.")
                .WithStatusCode(HttpStatusCode.OK)
                .Build());
        }
    }
}
