using API.Data.Models;
using API.DTOs;
using API.DTOs.Request;
using API.DTOs.Response;
using API.Services;
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
                return NotFound(APIResponse<List<MovieDTO>>.Builder()
                    .WithErrorMessages(new List<string> { "No movies found" })
                    .WithStatusCode(HttpStatusCode.NotFound)
                    .WithSuccess(false)
                    .Build());
            }
            return Ok(APIResponse<List<MovieDTO>>.Builder().WithResult(movies).WithStatusCode(HttpStatusCode.OK).Build());
        }

        [HttpGet("get-all-movies-with-pagination")]
        public async Task<ActionResult<APIResponse<List<MovieDTO>>>> GetAllMoviesWithPaginationAsync(int pageNumber, int pageSize, bool? isActive = true)
        {
            var movies = await _movieService.GetAllMoviesWithPaginationAsync(pageNumber, pageSize, isActive);
            if (movies.Count == 0)
            {
                return NotFound(APIResponse<List<MovieDTO>>.Builder()
                    .WithErrorMessages(new List<string> { "No movies found" })
                    .WithStatusCode(HttpStatusCode.NotFound)
                    .WithSuccess(false)
                    .Build());
            }
            return Ok(APIResponse<List<MovieDTO>>.Builder().WithResult(movies).WithStatusCode(HttpStatusCode.OK).Build());
        }


        [HttpPost("create")]
        [Authorize(Roles = Constant.Role_Admin)]
        public async Task<ActionResult<APIResponse<MovieDTO>>> CreateMovieAsync([FromBody] MovieCreateDTO movieCreateDTO)
        {
            if (movieCreateDTO == null)
            {
                return BadRequest(APIResponse<MovieDTO>.Builder()
                    .WithErrorMessages(new List<string> { "Movie data is null" })
                    .WithStatusCode(HttpStatusCode.BadRequest)
                    .WithSuccess(false)
                    .Build());
            }

            MovieDTO movieDTO = await _movieService.CreateMovieAsync(movieCreateDTO);
            return Ok(APIResponse<MovieDTO>.Builder().WithResult(movieDTO).WithStatusCode(HttpStatusCode.OK).Build());
        }

        [HttpPut("update/{id}")]
        [Authorize(Roles = Constant.Role_Admin)]
        public async Task<ActionResult<APIResponse<MovieDTO>>> UpdateMovieAsync([FromRoute] int id, [FromBody] MovieUpdateDTO movieUpdateDTO)
        {
            if (id == 0 || movieUpdateDTO == null)
            {
                return BadRequest(APIResponse<MovieDTO>.Builder()
                    .WithErrorMessages(new List<string> { "Movie Id or Update Movie is null" })
                    .WithStatusCode(HttpStatusCode.BadRequest)
                    .WithSuccess(false)
                    .Build());
            }

            MovieDTO movieDTO = await _movieService.UpdateMovieAsync(id, movieUpdateDTO);
            if (movieDTO == null)
            {
                return NotFound(APIResponse<MovieDTO>.Builder()
                    .WithErrorMessages(new List<string> { $"Movie with ID {id} not found" })
                    .WithStatusCode(HttpStatusCode.NotFound)
                    .WithSuccess(false)
                    .Build());
            }

            return Ok(APIResponse<MovieDTO>.Builder().WithResult(movieDTO).WithStatusCode(HttpStatusCode.OK).Build());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<APIResponse<MovieDetailDTO>>> GetMovieByIdAsync([FromRoute] int id)
        {
            if (id == 0)
            {
                return BadRequest(APIResponse<MovieDetailDTO>.Builder()
                    .WithErrorMessages(new List<string> { "Movie Id is null" })
                    .WithStatusCode(HttpStatusCode.BadRequest)
                    .WithSuccess(false)
                    .Build());
            }

            var movieDTO = await _movieService.GetMovieByIdAsync(id);
            if (movieDTO == null)
            {
                return NotFound(APIResponse<MovieDetailDTO>.Builder()
                    .WithErrorMessages(new List<string> { $"Movie with ID {id} not found" })
                    .WithStatusCode(HttpStatusCode.NotFound)
                    .WithSuccess(false)
                    .Build());
            }

            return Ok(APIResponse<MovieDetailDTO>.Builder().WithResult(movieDTO).WithStatusCode(HttpStatusCode.OK).Build());
        }

        [HttpDelete("delete/{id}")]
        [Authorize(Roles = Constant.Role_Admin)]
        public async Task<ActionResult<APIResponse<object>>> DeleteMovieAsync([FromRoute] int id)
        {
            if (id == 0)
            {
                return BadRequest(APIResponse<MovieDTO>.Builder()
                    .WithErrorMessages(new List<string> { "Movie Id is null" })
                    .WithStatusCode(HttpStatusCode.BadRequest)
                    .WithSuccess(false)
                    .Build());
            }

            await _movieService.DeleteMovieAsync(id);

            return Ok(APIResponse<MovieDTO>.Builder().WithStatusCode(HttpStatusCode.OK).Build());
        }

        [HttpGet("search-movie-by-name")]
        public async Task<ActionResult<APIResponse<List<MovieDTO>>>> SearchMoviesByNameAsync(string name)
        {
            if (name.IsNullOrEmpty())
            {
                return BadRequest(APIResponse<List<MovieDTO>>.Builder()
                    .WithErrorMessages(new List<string> { "Input is null or empty" })
                    .WithStatusCode(HttpStatusCode.BadRequest)
                    .WithSuccess(false)
                    .Build());
            }

            var movies = await _movieService.SearchMoviesAsync(name);
            if (movies.Count == 0)
            {
                return NotFound(APIResponse<List<MovieDTO>>.Builder()
                    .WithErrorMessages(new List<string> { "No movies found matching the search criteria" })
                    .WithStatusCode(HttpStatusCode.NotFound)
                    .WithSuccess(false)
                    .Build());
            }

            return Ok(APIResponse<List<MovieDTO>>.Builder().WithResult(movies).WithStatusCode(HttpStatusCode.OK).Build());
        }
    }
}
