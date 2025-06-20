using System.Net;
using API.DTOs;
using API.DTOs.Request;
using API.DTOs.Response;
using API.Services.IServices;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

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
    public async Task<ActionResult<APIResponse<List<ConcessionCategoryDTO>>>> GetAllConcessionCategories()
    {
        var categories = await _concessionCategoryService.GetAllConcessionCategoriesAsync();
        if (categories == null || categories.Count == 0)
        {
            return NotFound(APIResponse<List<ConcessionCategoryDTO>>.Builder().WithStatusCode(HttpStatusCode.NotFound).WithErrorMessages(new List<string> { "ConcessionCategory not found" }).Build());
        }
        return Ok(APIResponse<List<ConcessionCategoryDTO>>.Builder().WithResult(categories).WithStatusCode(HttpStatusCode.OK).Build());
    }

    [HttpGet("get-concession-category-by-id/{id}")]
    public async Task<ActionResult<APIResponse<ConcessionCategoryDTO>>> GetConcessionCategoryById([FromRoute] int id)
    {
        var category = await _concessionCategoryService.GetConcessionCategoryByIdAsync(id);
        if (category == null)
        {
            return NotFound(APIResponse<ConcessionCategoryDTO>.Builder().WithStatusCode(HttpStatusCode.NotFound).WithErrorMessages(new List<string> { "ConcessionCategory not found" }).Build());
        }
        return Ok(APIResponse<ConcessionCategoryDTO>.Builder().WithResult(category).WithStatusCode(HttpStatusCode.OK).Build());
    }
    [HttpPost("create-concession-category")]
    public async Task<ActionResult<APIResponse<ConcessionCategoryDTO>>> CreateConcessionCategory([FromBody] ConcessionCategoryCreateDTO concessionCategory)
    {
        if (concessionCategory == null)
        {
            return BadRequest(APIResponse<ConcessionCategoryDTO>.Builder().WithStatusCode(HttpStatusCode.BadRequest).WithErrorMessages(new List<string> { "Invalid concession category data" }).Build());
        }

        var createdCategory = await _concessionCategoryService.CreateConcessionCategoryAsync(concessionCategory);
        if (createdCategory == null)
        {
            return BadRequest(APIResponse<ConcessionCategoryDTO>.Builder().WithStatusCode(HttpStatusCode.BadRequest).WithErrorMessages(new List<string> { "Failed to create concession category" }).Build());
        }
        return Ok(APIResponse<ConcessionCategoryDTO>.Builder().WithResult(createdCategory).WithStatusCode(HttpStatusCode.Created).Build());
    }
    [HttpPut("update-concession-category/{id}")]
    public async Task<ActionResult<APIResponse<ConcessionCategoryDTO>>> UpdateConcessionCategory([FromRoute] int id, [FromBody] ConcessionCategoryUpdateDTO concessionCategory)
    {
        if (concessionCategory == null)
        {
            return BadRequest(APIResponse<ConcessionCategoryDTO>.Builder().WithStatusCode(HttpStatusCode.BadRequest).WithErrorMessages(new List<string> { "Invalid concession category data" }).Build());
        }

        var updatedCategory = await _concessionCategoryService.UpdateConcessionCategoryAsync(id, concessionCategory);
        if (updatedCategory == null)
        {
            return NotFound(APIResponse<ConcessionCategoryDTO>.Builder().WithStatusCode(HttpStatusCode.NotFound).WithErrorMessages(new List<string> { "ConcessionCategory not found" }).Build());
        }
        return Ok(APIResponse<ConcessionCategoryDTO>.Builder().WithResult(updatedCategory).WithStatusCode(HttpStatusCode.OK).Build());
    }
    [HttpDelete("delete-concession-category/{id}")]
    public async Task<ActionResult<APIResponse<ConcessionCategoryDTO>>> DeleteConcessionCategory([FromRoute] int id)
    {
        var deletedCategory = await _concessionCategoryService.DeleteConcessionCategoryAsync(id);
        if (deletedCategory == null)
        {
            return NotFound(APIResponse<ConcessionCategoryDTO>.Builder().WithStatusCode(HttpStatusCode.NotFound).WithErrorMessages(new List<string> { "ConcessionCategory not found" }).Build());
        }
        return Ok(APIResponse<ConcessionCategoryDTO>.Builder().WithResult(deletedCategory).WithStatusCode(HttpStatusCode.OK).Build());
    }
}