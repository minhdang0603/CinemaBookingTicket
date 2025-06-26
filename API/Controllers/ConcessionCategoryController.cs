using API.DTOs;
using API.DTOs.Request;
using API.DTOs.Response;
using API.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using Utility;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConcessionCategoryController : ControllerBase
    {
        private readonly IConcessionCategoryService _concessionCategoryService;
        public ConcessionCategoryController(IConcessionCategoryService concessionCategoryService)
        {
            _concessionCategoryService = concessionCategoryService;
        }

        [HttpGet("get-all-concession-categories")]
        public async Task<ActionResult<APIResponse<List<ConcessionCategoryDTO>>>> GetAllConcessionCategoriesAsync()
        {
            var concessionCategories = await _concessionCategoryService.GetAllConcessionCategoriesAsync();
            if (concessionCategories.ToList().Count == 0)
            {
                return NotFound(APIResponse<List<ConcessionCategoryDTO>>.Builder()
                    .WithErrorMessages(new List<string> { "No concession categories found" })
                    .WithStatusCode(HttpStatusCode.NotFound)
                    .WithSuccess(false)
                    .Build());
            }
            return Ok(APIResponse<List<ConcessionCategoryDTO>>.Builder().WithResult(concessionCategories.ToList()).WithStatusCode(HttpStatusCode.OK).Build());
        }

        [HttpPost("create")]
        [Authorize(Roles = Constant.Role_Admin)]
        public async Task<ActionResult<APIResponse<ConcessionCategoryDTO>>> CreateConcessionCategoryAsync([FromBody] ConcessionCategoryCreateDTO concessionCategoryCreateDTO)
        {
            if (concessionCategoryCreateDTO == null)
            {
                return BadRequest(APIResponse<ConcessionCategoryDTO>.Builder()
                    .WithErrorMessages(new List<string> { "Concession category data is null" })
                    .WithStatusCode(HttpStatusCode.BadRequest)
                    .WithSuccess(false)
                    .Build());
            }

            ConcessionCategoryDTO concessionCategoryDTO = await _concessionCategoryService.CreateConcessionCategoryAsync(concessionCategoryCreateDTO);
            return Ok(APIResponse<ConcessionCategoryDTO>.Builder().WithResult(concessionCategoryDTO).WithStatusCode(HttpStatusCode.OK).Build());
        }

        [HttpPut("update/{id}")]
        [Authorize(Roles = Constant.Role_Admin)]
        public async Task<ActionResult<APIResponse<ConcessionCategoryDTO>>> UpdateConcessionCategoryAsync([FromRoute] int id, [FromBody] ConcessionCategoryUpdateDTO concessionCategoryUpdateDTO)
        {
            if (id == 0 || concessionCategoryUpdateDTO == null)
            {
                return BadRequest(APIResponse<ConcessionCategoryDTO>.Builder()
                    .WithErrorMessages(new List<string> { "Concession category Id or Update Concession category is null" })
                    .WithStatusCode(HttpStatusCode.BadRequest)
                    .WithSuccess(false)
                    .Build());
            }

            ConcessionCategoryDTO concessionCategoryDTO = await _concessionCategoryService.UpdateConcessionCategoryAsync(id, concessionCategoryUpdateDTO);
            if (concessionCategoryDTO == null)
            {
                return NotFound(APIResponse<ConcessionCategoryDTO>.Builder()
                    .WithErrorMessages(new List<string> { $"Concession category with ID {id} not found" })
                    .WithStatusCode(HttpStatusCode.NotFound)
                    .WithSuccess(false)
                    .Build());
            }

            return Ok(APIResponse<ConcessionCategoryDTO>.Builder().WithResult(concessionCategoryDTO).WithStatusCode(HttpStatusCode.OK).Build());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<APIResponse<ConcessionCategoryDTO>>> GetConcessionCategoryByIdAsync([FromRoute] int id)
        {
            if (id == 0)
            {
                return BadRequest(APIResponse<ConcessionCategoryDTO>.Builder()
                    .WithErrorMessages(new List<string> { "Concession category Id is null" })
                    .WithStatusCode(HttpStatusCode.BadRequest)
                    .WithSuccess(false)
                    .Build());
            }

            var concessionCategory = await _concessionCategoryService.GetConcessionCategoryByIdAsync(id);
            if (concessionCategory == null)
            {
                return NotFound(APIResponse<ConcessionCategoryDTO>.Builder()
                    .WithErrorMessages(new List<string> { $"Concession category with ID {id} not found" })
                    .WithStatusCode(HttpStatusCode.NotFound)
                    .WithSuccess(false)
                    .Build());
            }

            return Ok(APIResponse<ConcessionCategoryDTO>.Builder().WithResult(concessionCategory).WithStatusCode(HttpStatusCode.OK).Build());
        }

        [HttpDelete("delete/{id}")]
        [Authorize(Roles = Constant.Role_Admin)]
        public async Task<ActionResult<APIResponse<ConcessionCategoryDTO>>> DeleteConcessionCategoryAsync([FromRoute] int id)
        {
            if (id == 0)
            {
                return BadRequest(APIResponse<ConcessionCategoryDTO>.Builder()
                    .WithErrorMessages(new List<string> { "Concession category Id is null" })
                    .WithStatusCode(HttpStatusCode.BadRequest)
                    .WithSuccess(false)
                    .Build());
            }

            var concessionCategory = await _concessionCategoryService.DeleteConcessionCategoryAsync(id);
            if (concessionCategory == null)
            {
                return NotFound(APIResponse<ConcessionCategoryDTO>.Builder()
                    .WithErrorMessages(new List<string> { $"Concession category with ID {id} not found" })
                    .WithStatusCode(HttpStatusCode.NotFound)
                    .WithSuccess(false)
                    .Build());
            }

            return Ok(APIResponse<ConcessionCategoryDTO>.Builder().WithResult(concessionCategory).WithStatusCode(HttpStatusCode.OK).Build());
        }
    }
}