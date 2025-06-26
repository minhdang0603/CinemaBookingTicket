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
    public class GenreController : ControllerBase
    {
        private readonly IGenreService _genreService;
        public GenreController(IGenreService genreService)
        {
            _genreService = genreService;
        }

        [HttpGet("get-all-genres")]
        public async Task<ActionResult<APIResponse<List<GenreDTO>>>> GetAllGenresAsync()
        {
            var genres = await _genreService.GetAllGenresAsync();
            if (genres.Count == 0)
            {
                return NotFound(APIResponse<List<GenreDTO>>.Builder()
                    .WithErrorMessages(new List<string> { "No genres found" })
                    .WithStatusCode(HttpStatusCode.NotFound)
                    .WithSuccess(false)
                    .Build());
            }
            return Ok(APIResponse<List<GenreDTO>>.Builder().WithResult(genres).WithStatusCode(HttpStatusCode.OK).Build());
        }

        [HttpGet("get-all-genre-pagination")]
        public async Task<ActionResult<APIResponse<List<GenreDTO>>>> GetAllGenresWithPaginationAsync(int pageNumber, int pageSize, bool? isActive = true)
        {
            var genres = await _genreService.GetAllGenresWithPaginationAsync(pageNumber, pageSize, isActive);
            if (genres.Count == 0)
            {
                return NotFound(APIResponse<List<GenreDTO>>.Builder()
                    .WithErrorMessages(new List<string> { "No genres found" })
                    .WithStatusCode(HttpStatusCode.NotFound)
                    .WithSuccess(false)
                    .Build());
            }

            return Ok(APIResponse<List<GenreDTO>>.Builder().WithResult(genres).WithStatusCode(HttpStatusCode.OK).Build());
        }

        [HttpPost("create")]
        [Authorize(Roles = Constant.Role_Admin)]
        public async Task<ActionResult<APIResponse<GenreDTO>>> CreateGenreAsync([FromBody] GenreCreateDTO genreCreateDTO)
        {
            if (genreCreateDTO == null)
            {
                return BadRequest(APIResponse<GenreDTO>.Builder()
                    .WithErrorMessages(new List<string> { "Genre data is null" })
                    .WithStatusCode(HttpStatusCode.BadRequest)
                    .WithSuccess(false)
                    .Build());
            }

            GenreDTO genreDTO = await _genreService.CreateGenreAsync(genreCreateDTO);
            return Ok(APIResponse<GenreDTO>.Builder().WithResult(genreDTO).WithStatusCode(HttpStatusCode.OK).Build());
        }

        [HttpPut("update/{id}")]
        [Authorize(Roles = Constant.Role_Admin)]
        public async Task<ActionResult<APIResponse<GenreDTO>>> UpdateGenreAsync([FromRoute] int id, [FromBody] GenreUpdateDTO genreUpdateDTO)
        {
            if (id == 0 || genreUpdateDTO == null)
            {
                return BadRequest(APIResponse<GenreDTO>.Builder()
                    .WithErrorMessages(new List<string> { "Genre Id or Update Genre is null" })
                    .WithStatusCode(HttpStatusCode.BadRequest)
                    .WithSuccess(false)
                    .Build());
            }

            GenreDTO genreDTO = await _genreService.UpdateGenreAsync(id, genreUpdateDTO);
            if (genreDTO == null)
            {
                return NotFound(APIResponse<GenreDTO>.Builder()
                    .WithErrorMessages(new List<string> { $"Genre with ID {id} not found" })
                    .WithStatusCode(HttpStatusCode.NotFound)
                    .WithSuccess(false)
                    .Build());
            }

            return Ok(APIResponse<GenreDTO>.Builder().WithResult(genreDTO).WithStatusCode(HttpStatusCode.OK).Build());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<APIResponse<GenreDTO>>> GetGenreByIdAsync([FromRoute] int id)
        {
            if (id == 0)
            {
                return BadRequest(APIResponse<GenreDTO>.Builder()
                    .WithErrorMessages(new List<string> { "Genre Id is null" })
                    .WithStatusCode(HttpStatusCode.BadRequest)
                    .WithSuccess(false)
                    .Build());
            }

            var genre = await _genreService.GetGenreByIdAsync(id);
            if (genre == null)
            {
                return NotFound(APIResponse<GenreDTO>.Builder()
                    .WithErrorMessages(new List<string> { $"Genre with ID {id} not found" })
                    .WithStatusCode(HttpStatusCode.NotFound)
                    .WithSuccess(false)
                    .Build());
            }

            return Ok(APIResponse<GenreDTO>.Builder().WithResult(genre).WithStatusCode(HttpStatusCode.OK).Build());
        }

        [HttpDelete("delete/{id}")]
        [Authorize(Roles = Constant.Role_Admin)]
        public async Task<ActionResult<APIResponse<GenreDTO>>> DeleteGenreAsync([FromRoute] int id)
        {
            if (id == 0)
            {
                return BadRequest(APIResponse<GenreDTO>.Builder()
                    .WithErrorMessages(new List<string> { "Genre Id is null" })
                    .WithStatusCode(HttpStatusCode.BadRequest)
                    .WithSuccess(false)
                    .Build());
            }

            GenreDTO genreDTO = await _genreService.DeleteGenreAsync(id);
            if (genreDTO == null)
            {
                return NotFound(APIResponse<GenreDTO>.Builder()
                    .WithErrorMessages(new List<string> { $"Genre with ID {id} not found" })
                    .WithStatusCode(HttpStatusCode.NotFound)
                    .WithSuccess(false)
                    .Build());
            }

            return Ok(APIResponse<GenreDTO>.Builder().WithResult(genreDTO).WithStatusCode(HttpStatusCode.OK).Build());
        }

        [HttpGet("search-genre-by-name")]
        public async Task<ActionResult<APIResponse<List<GenreDTO>>>> SearchGenresAsync(string name)
        {
            if (name.IsNullOrEmpty())
            {
                return BadRequest(APIResponse<List<GenreDTO>>.Builder()
                    .WithErrorMessages(new List<string> { "Input is null or empty" })
                    .WithStatusCode(HttpStatusCode.BadRequest)
                    .WithSuccess(false)
                    .Build());
            }

            var genres = await _genreService.SearchGenresAsync(name);
            if (genres.Count == 0)
            {
                return NotFound(APIResponse<List<GenreDTO>>.Builder()
                    .WithErrorMessages(new List<string> { "No genres found matching the search criteria" })
                    .WithStatusCode(HttpStatusCode.NotFound)
                    .WithSuccess(false)
                    .Build());
            }

            return Ok(APIResponse<List<GenreDTO>>.Builder().WithResult(genres).WithStatusCode(HttpStatusCode.OK).Build());
        }
    }
}
