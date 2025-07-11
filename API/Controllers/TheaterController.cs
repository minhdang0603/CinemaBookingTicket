﻿using API.DTOs;
using API.DTOs.Request;
using API.DTOs.Response;
using API.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using Utility;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TheaterController : ControllerBase
    {
        private readonly ITheaterService _theaterService;

        public TheaterController(ITheaterService theaterService)
        {
            _theaterService = theaterService;
        }

        [HttpPost("add-theater-admin")]
        [Authorize(Roles = Constant.Role_Admin)]
        public async Task<ActionResult<APIResponse<string>>> AddTheaterAsync([FromBody] TheaterCreateDTO theaterCreateDTO)
        {
            await _theaterService.AddTheaterAsync(theaterCreateDTO);
            return Ok(APIResponse<string>.Builder()
                .WithResult($"Theater {theaterCreateDTO.Name} created successfully.")
                .WithStatusCode(HttpStatusCode.OK)
                .Build());
        }

        [HttpPut("update-theater-admin")]
        [Authorize(Roles = Constant.Role_Admin)]
        public async Task<ActionResult<APIResponse<string>>> UpdateTheaterAsync(int id, [FromBody] TheaterUpdateDTO theaterUpdateDTO)
        {
            if (id == 0 || theaterUpdateDTO == null)
            {
                return BadRequest(APIResponse<string>.Builder()
                    .WithErrorMessages(new List<string> { "Theater Id or update data is null." })
                    .WithStatusCode(HttpStatusCode.BadRequest)
                    .Build());
            }

            await _theaterService.UpdateTheaterAsync(id, theaterUpdateDTO);
            return Ok(APIResponse<string>.Builder()
                .WithResult($"Theater {theaterUpdateDTO.Name} updated successfully.")
                .WithStatusCode(HttpStatusCode.OK)
                .Build());
        }

        [HttpDelete("delete-theater-admin")]
        [Authorize(Roles = Constant.Role_Admin)]
        public async Task<ActionResult<APIResponse<string>>> DeleteTheaterAsync(int id)
        {
            if (id == 0)
            {
                return BadRequest(APIResponse<string>.Builder()
                    .WithErrorMessages(new List<string> { "Theater Id is null." })
                    .WithStatusCode(HttpStatusCode.BadRequest)
                    .Build());
            }

            await _theaterService.DeleteTheaterAsync(id);
            return Ok(APIResponse<string>.Builder()
                .WithResult($"Theater {id} deleted successfully.")
                .WithStatusCode(HttpStatusCode.OK)
                .Build());
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<APIResponse<TheaterDetailDTO>>> GetTheaterByIdAsync(int id)
        {
            if (id == 0)
            {
                return BadRequest(APIResponse<string>.Builder()
                    .WithErrorMessages(new List<string> { "Theater Id is null." })
                    .WithStatusCode(HttpStatusCode.BadRequest)
                    .Build());
            }

            var theater = await _theaterService.GetTheaterByIdAsync(id);
            return Ok(APIResponse<TheaterDetailDTO>.Builder()
                .WithResult(theater)
                .WithStatusCode(HttpStatusCode.OK)
                .Build());
        }

        [HttpGet]
        public async Task<ActionResult<APIResponse<List<TheaterDTO>>>> GetAllTheatersAsync()
        {
            var theaters = await _theaterService.GetAllTheatersAsync();
            return Ok(APIResponse<List<TheaterDTO>>.Builder()
                .WithResult(theaters)
                .WithStatusCode(HttpStatusCode.OK)
                .Build());
        }
    }
}
