using API.Data.Models;
using API.DTOs;
using API.DTOs.Request;
using API.DTOs.Response;
using API.Services;
using API.Services.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Net;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovieController : ControllerBase
    {
        private readonly IMovieService _movieService;
        public MovieController(IMovieService movieService)
        {
            _movieService = movieService;
        }

        [HttpGet("get-all-movies")]
        public async Task<ActionResult<APIResponse<List<MovieDTO>>>> GetAllMoviesAsync()
        {
            var movies = await _movieService.GetAllMoviesAsync();
            if (movies.Count == 0)
            {
                return NotFound(APIResponse<List<MovieDTO>>.Builder().WithErrorMessages("No movies found").WithStatusCode(HttpStatusCode.NotFound).Build());
            }
            return Ok(APIResponse<List<MovieDTO>>.Builder().WithResult(movies).WithStatusCode(HttpStatusCode.OK).Build());
        }

        [HttpGet("get-all-movies-with-pagination")]
        public async Task<ActionResult<APIResponse<List<MovieDTO>>>> GetAllMoviesWithPaginationAsync(int pageNumber, int pageSize, bool? isActive = true)
        {
            var movies = await _movieService.GetAllMoviesWithPaginationAsync(pageNumber, pageSize, isActive);
            if (movies.Count == 0)
            {
                return NotFound(APIResponse<List<MovieDTO>>.Builder().WithErrorMessages("No movies found").WithStatusCode(HttpStatusCode.NotFound).Build());
            }
            return Ok(APIResponse<List<MovieDTO>>.Builder().WithResult(movies).WithStatusCode(HttpStatusCode.OK).Build());
        }


        [HttpPost("create-movie")]
        public async Task<ActionResult<APIResponse<string>>> CreateGenreAsync([FromBody] MovieCreateDTO movieCreateDTO)
        {
            await _movieService.CreateMovieAsync(movieCreateDTO);

            return Ok(APIResponse<string>.Builder().WithResult($"Movie {movieCreateDTO.Title} created successfully.").WithStatusCode(HttpStatusCode.OK).Build());
        }

        [HttpPut("update-movie")]
        public async Task<ActionResult<APIResponse<string>>> UpdateGenreAsync(int id, [FromBody] MovieUpdateDTO movieUpdateDTO)
        {
            if (id == 0 || movieUpdateDTO == null)
            {
                return BadRequest(APIResponse<string>.Builder().WithErrorMessages("Movie Id or Update Genre is null.").WithStatusCode(HttpStatusCode.BadRequest).Build());
            }
            await _movieService.UpdateMovieAsync(id, movieUpdateDTO);

            return Ok(APIResponse<string>.Builder().WithResult($"Movie {movieUpdateDTO.Title} updated successfully.").WithStatusCode(HttpStatusCode.OK).Build());
        }

        [HttpGet("get-movie-by-id")]
        public async Task<ActionResult<APIResponse<MovieDTO>>> GetGenreByIdAsync(int id)
        {
            if (id == 0)
            {
                return BadRequest(APIResponse<string>.Builder().WithErrorMessages("Movie Id is null.").WithStatusCode(HttpStatusCode.BadRequest).Build());
            }
            var movieDTO = await _movieService.GetMovieByIdAsync(id);
            return Ok(APIResponse<MovieDTO>.Builder().WithResult(movieDTO).WithStatusCode(HttpStatusCode.OK).Build());
        }

        [HttpDelete("delete-movie")]
        public async Task<ActionResult<APIResponse<string>>> DeleteGenreAsync(int id)
        {
            if (id == 0)
            {
                return BadRequest(APIResponse<string>.Builder().WithErrorMessages("Movie Id is null.").WithStatusCode(HttpStatusCode.BadRequest).Build());
            }
            await _movieService.DeleteMovieAsync(id);
            return Ok(APIResponse<string>.Builder().WithResult($"Movie deleted successfully.").WithStatusCode(HttpStatusCode.OK).Build());
        }

        [HttpGet("search-movie-by-name")]
        public async Task<ActionResult<APIResponse<List<MovieDTO>>>> SearchGenresAsync(string name)
        {
            if (name.IsNullOrEmpty())
            {
                return BadRequest(APIResponse<List<MovieDTO>>.Builder().WithErrorMessages("Input is null or empty.").WithStatusCode(HttpStatusCode.BadRequest).Build());
            }
            var movies = await _movieService.SearchMoviesAsync(name);
            return Ok(APIResponse<List<MovieDTO>>.Builder().WithResult(movies).WithStatusCode(HttpStatusCode.OK).Build());
        }
    }
}
