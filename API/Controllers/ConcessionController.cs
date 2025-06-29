using API.DTOs;
using API.DTOs.Request;
using API.DTOs.Response;
using API.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Utility;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConcessionController : ControllerBase
    {
        private readonly IConcessionService _concessionService;
        public ConcessionController(IConcessionService concessionService)
        {
            _concessionService = concessionService;
        }

        [HttpGet("get-all-concessions")]
        public async Task<ActionResult<APIResponse<List<ConcessionDTO>>>> GetAllConcessionsAsync()
        {
            var concessions = await _concessionService.GetAllConcessionsAsync();
            if (concessions.Count == 0)
            {
                return NotFound(APIResponse<List<ConcessionDTO>>.Builder()
                    .WithErrorMessages(new List<string> { "No concessions found" })
                    .WithStatusCode(HttpStatusCode.NotFound)
                    .WithSuccess(false)
                    .Build());
            }
            return Ok(APIResponse<List<ConcessionDTO>>.Builder().WithResult(concessions).WithStatusCode(HttpStatusCode.OK).Build());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<APIResponse<ConcessionDTO>>> GetConcessionByIdAsync([FromRoute] int id)
        {
            if (id == 0)
            {
                return BadRequest(APIResponse<ConcessionDTO>.Builder()
                    .WithErrorMessages(new List<string> { "Concession Id is null" })
                    .WithStatusCode(HttpStatusCode.BadRequest)
                    .WithSuccess(false)
                    .Build());
            }

            try
            {
                var concession = await _concessionService.GetConcessionByIdAsync(id);
                return Ok(APIResponse<ConcessionDTO>.Builder().WithResult(concession).WithStatusCode(HttpStatusCode.OK).Build());
            }
            catch (Exception ex)
            {
                return NotFound(APIResponse<ConcessionDTO>.Builder()
                    .WithErrorMessages(new List<string> { ex.Message })
                    .WithStatusCode(HttpStatusCode.NotFound)
                    .WithSuccess(false)
                    .Build());
            }
        }

        [HttpPost("create")]
        [Authorize(Roles = Constant.Role_Admin)]
        public async Task<ActionResult<APIResponse<ConcessionDTO>>> CreateConcessionAsync([FromBody] ConcessionCreateDTO concessionCreateDTO)
        {
            if (concessionCreateDTO == null)
            {
                return BadRequest(APIResponse<ConcessionDTO>.Builder()
                    .WithErrorMessages(new List<string> { "Concession data is null" })
                    .WithStatusCode(HttpStatusCode.BadRequest)
                    .WithSuccess(false)
                    .Build());
            }

            try
            {
                ConcessionDTO concessionDTO = await _concessionService.CreateConcessionAsync(concessionCreateDTO);
                return Ok(APIResponse<ConcessionDTO>.Builder().WithResult(concessionDTO).WithStatusCode(HttpStatusCode.OK).Build());
            }
            catch (Exception ex)
            {
                return BadRequest(APIResponse<ConcessionDTO>.Builder()
                    .WithErrorMessages(new List<string> { ex.Message })
                    .WithStatusCode(HttpStatusCode.BadRequest)
                    .WithSuccess(false)
                    .Build());
            }
        }

        [HttpPut("update/{id}")]
        [Authorize(Roles = Constant.Role_Admin)]
        public async Task<ActionResult<APIResponse<ConcessionDTO>>> UpdateConcessionAsync([FromRoute] int id, [FromBody] ConcessionUpdateDTO concessionUpdateDTO)
        {
            if (id == 0 || concessionUpdateDTO == null)
            {
                return BadRequest(APIResponse<ConcessionDTO>.Builder()
                    .WithErrorMessages(new List<string> { "Concession Id or Update Concession is null" })
                    .WithStatusCode(HttpStatusCode.BadRequest)
                    .WithSuccess(false)
                    .Build());
            }

            try
            {
                ConcessionDTO concessionDTO = await _concessionService.UpdateConcessionAsync(id, concessionUpdateDTO);
                return Ok(APIResponse<ConcessionDTO>.Builder().WithResult(concessionDTO).WithStatusCode(HttpStatusCode.OK).Build());
            }
            catch (Exception ex)
            {
                return NotFound(APIResponse<ConcessionDTO>.Builder()
                    .WithErrorMessages(new List<string> { ex.Message })
                    .WithStatusCode(HttpStatusCode.NotFound)
                    .WithSuccess(false)
                    .Build());
            }
        }

        [HttpDelete("delete/{id}")]
        [Authorize(Roles = Constant.Role_Admin)]
        public async Task<ActionResult<APIResponse<ConcessionDTO>>> DeleteConcessionAsync([FromRoute] int id)
        {
            if (id == 0)
            {
                return BadRequest(APIResponse<ConcessionDTO>.Builder()
                    .WithErrorMessages(new List<string> { "Concession Id is null" })
                    .WithStatusCode(HttpStatusCode.BadRequest)
                    .WithSuccess(false)
                    .Build());
            }

            try
            {
                ConcessionDTO concessionDTO = await _concessionService.DeleteConcessionAsync(id);
                return Ok(APIResponse<ConcessionDTO>.Builder().WithResult(concessionDTO).WithStatusCode(HttpStatusCode.OK).Build());
            }
            catch (Exception ex)
            {
                return NotFound(APIResponse<ConcessionDTO>.Builder()
                    .WithErrorMessages(new List<string> { ex.Message })
                    .WithStatusCode(HttpStatusCode.NotFound)
                    .WithSuccess(false)
                    .Build());
            }
        }

        [HttpGet("get-by-category/{categoryId}")]
        public async Task<ActionResult<APIResponse<List<ConcessionDTO>>>> GetAllConcessionsByCategoryIdAsync([FromRoute] int categoryId)
        {
            if (categoryId == 0)
            {
                return BadRequest(APIResponse<List<ConcessionDTO>>.Builder()
                    .WithErrorMessages(new List<string> { "Category Id is null" })
                    .WithStatusCode(HttpStatusCode.BadRequest)
                    .WithSuccess(false)
                    .Build());
            }

            var concessions = await _concessionService.GetAllConcessionsByCategoryIdAsync(categoryId);
            if (concessions.Count == 0)
            {
                return NotFound(APIResponse<List<ConcessionDTO>>.Builder()
                    .WithErrorMessages(new List<string> { $"No concessions found for category ID {categoryId}" })
                    .WithStatusCode(HttpStatusCode.NotFound)
                    .WithSuccess(false)
                    .Build());
            }

            return Ok(APIResponse<List<ConcessionDTO>>.Builder().WithResult(concessions).WithStatusCode(HttpStatusCode.OK).Build());
        }
    }
}