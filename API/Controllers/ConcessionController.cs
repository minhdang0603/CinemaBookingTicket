using System.Net;
using API.DTOs;
using API.DTOs.Request;
using API.DTOs.Response;
using API.Services.IServices;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

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
    public async Task<ActionResult<APIResponse<List<ConcessionDTO>>>> GetAllConcessions()
    {
        var concessions = await _concessionService.GetAllConcessionsAsync();
        if (concessions == null || concessions.Count == 0)
        {
            return NotFound(APIResponse<List<ConcessionDTO>>.Builder()
                .WithErrorMessages(new List<string> { "No active concessions found." })
                .WithStatusCode(HttpStatusCode.NotFound)
                .Build());
        }
        return Ok(APIResponse<List<ConcessionDTO>>.Builder()
            .WithResult(concessions)
            .WithStatusCode(HttpStatusCode.OK)
            .Build());
    }
    [HttpGet("get-all-concessions-with-pagination")]
    public async Task<ActionResult<APIResponse<List<ConcessionDTO>>>> GetAllConcessionsWithPagination([FromQuery] int pageNumber, [FromQuery] int pageSize, [FromQuery] bool? isActive = true)
    {
        var concessions = await _concessionService.GetAllConcessionsWithPaginationAsync(pageNumber, pageSize, isActive);
        if (concessions == null || concessions.Count == 0)
        {
            return NotFound(APIResponse<List<ConcessionDTO>>.Builder()
                .WithErrorMessages(new List<string> { "No concessions found with the specified criteria." })
                .WithStatusCode(HttpStatusCode.NotFound)
                .Build());
        }
        return Ok(APIResponse<List<ConcessionDTO>>.Builder()
            .WithResult(concessions)
            .WithStatusCode(HttpStatusCode.OK)
            .Build());
    }
    [HttpPost("create-concession")]
    public async Task<ActionResult<APIResponse<ConcessionDTO>>> CreateConcession([FromBody] ConcessionCreateDTO concessionCreateDTO)
    {
        if (concessionCreateDTO == null)
        {
            return BadRequest(APIResponse<ConcessionDTO>.Builder()
                .WithErrorMessages(new List<string> { "Concession data is required." })
                .WithStatusCode(HttpStatusCode.BadRequest)
                .Build());
        }

        var concession = await _concessionService.CreateConcessionAsync(concessionCreateDTO);
        return CreatedAtAction(nameof(GetAllConcessions), new { id = concession.Id }, APIResponse<ConcessionDTO>.Builder()
            .WithResult(concession)
            .WithStatusCode(HttpStatusCode.Created)
            .Build());
    }
    [HttpPut("update-concession/{id}")]
    public async Task<ActionResult<APIResponse<ConcessionDTO>>> UpdateConcession(int id, [FromBody] ConcessionUpdateDTO concessionUpdateDTO)
    {
        if (concessionUpdateDTO == null)
        {
            return BadRequest(APIResponse<ConcessionDTO>.Builder()
                .WithErrorMessages(new List<string> { "Concession data is required." })
                .WithStatusCode(HttpStatusCode.BadRequest)
                .Build());
        }

        var concession = await _concessionService.UpdateConcessionAsync(id, concessionUpdateDTO);
        if (concession == null)
        {
            return NotFound(APIResponse<ConcessionDTO>.Builder()
                .WithErrorMessages(new List<string> { "Concession not found." })
                .WithStatusCode(HttpStatusCode.NotFound)
                .Build());
        }
        return Ok(APIResponse<ConcessionDTO>.Builder()
            .WithResult(concession)
            .WithStatusCode(HttpStatusCode.OK)
            .Build());
    }
    [HttpDelete("delete-concession/{id}")]
    public async Task<ActionResult<APIResponse<ConcessionDTO>>> DeleteConcession(int id)
    {
        var concession = await _concessionService.DeleteConcessionAsync(id);
        if (concession == null)
        {
            return NotFound(APIResponse<ConcessionDTO>.Builder()
                .WithErrorMessages(new List<string> { "Concession not found." })
                .WithStatusCode(HttpStatusCode.NotFound)
                .Build());
        }
        return Ok(APIResponse<ConcessionDTO>.Builder()
            .WithResult(concession)
            .WithStatusCode(HttpStatusCode.OK)
            .Build());
    }
    [HttpGet("get-concession-by-id/{id}")]
    public async Task<ActionResult<APIResponse<ConcessionDTO>>> GetConcessionById(int id)
    {
        var concession = await _concessionService.GetConcessionByIdAsync(id);
        if (concession == null)
        {
            return NotFound(APIResponse<ConcessionDTO>.Builder()
                .WithErrorMessages(new List<string> { "Concession not found." })
                .WithStatusCode(HttpStatusCode.NotFound)
                .Build());
        }
        return Ok(APIResponse<ConcessionDTO>.Builder()
            .WithResult(concession)
            .WithStatusCode(HttpStatusCode.OK)
            .Build());
    }
    [HttpGet("get-concessions-by-category-id/{categoryId}")]
    public async Task<ActionResult<APIResponse<List<ConcessionDTO>>>> GetConcessionsByCategoryId(int categoryId)
    {
        var concessions = await _concessionService.GetConcessionsByCategoryIdAsync(categoryId);
        if (concessions == null || concessions.Count == 0)
        {
            return NotFound(APIResponse<List<ConcessionDTO>>.Builder()
                .WithErrorMessages(new List<string> { "No concessions found for the specified category." })
                .WithStatusCode(HttpStatusCode.NotFound)
                .Build());
        }
        return Ok(APIResponse<List<ConcessionDTO>>.Builder()
            .WithResult(concessions)
            .WithStatusCode(HttpStatusCode.OK)
            .Build());
    }
    [HttpGet("search-concessions-by-name")]
    public async Task<ActionResult<APIResponse<List<ConcessionDTO>>>> SearchConcessionsByName([FromQuery] string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return BadRequest(APIResponse<List<ConcessionDTO>>.Builder()
                .WithErrorMessages(new List<string> { "Search term is required." })
                .WithStatusCode(HttpStatusCode.BadRequest)
                .Build());
        }

        var concessions = await _concessionService.SearchConcessionsByNameAsync(name);
        if (concessions == null || concessions.Count == 0)
        {
            return NotFound(APIResponse<List<ConcessionDTO>>.Builder()
                .WithErrorMessages(new List<string> { "No concessions found matching the search term." })
                .WithStatusCode(HttpStatusCode.NotFound)
                .Build());
        }
        return Ok(APIResponse<List<ConcessionDTO>>.Builder()
            .WithResult(concessions)
            .WithStatusCode(HttpStatusCode.OK)
            .Build());
    }

}