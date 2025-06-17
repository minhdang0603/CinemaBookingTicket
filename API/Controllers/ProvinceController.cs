using API.DTOs;
using API.DTOs.Request;
using API.DTOs.Response;
using API.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Net;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProvinceController : ControllerBase
    {
        private readonly IProvinceService _provinceService;

        public ProvinceController(IProvinceService provinceService)
        {
            _provinceService = provinceService;
        }

        [HttpGet("get-all-provinces")]
        public async Task<ActionResult<APIResponse<List<ProvinceDTO>>>> GetAllProvincesAsync()
        {
            var provinces = await _provinceService.GetAllProvincesAsync();

            return Ok(APIResponse<List<ProvinceDTO>>.Builder()
                .WithResult(provinces)
                .WithStatusCode(HttpStatusCode.OK)
                .Build());
        }

        [HttpGet("get-province-by-id")]
        public async Task<ActionResult<APIResponse<ProvinceDetailDTO>>> GetProvinceByIdAsync(int id)
        {
            if (id == 0)
            {
                return BadRequest(APIResponse<string>.Builder()
                    .WithErrorMessages(new List<string> { "Province Id is null." })
                    .WithStatusCode(HttpStatusCode.BadRequest)
                    .Build());
            }

            var province = await _provinceService.GetProvinceByIdAsync(id);
            return Ok(APIResponse<ProvinceDetailDTO>.Builder()
                .WithResult(province)
                .WithStatusCode(HttpStatusCode.OK)
                .Build());
        }

        [HttpPost("create-province")]
        public async Task<ActionResult<APIResponse<string>>> CreateProvinceAsync([FromBody] ProvinceCreateDTO provinceCreateDTO)
        {
            await _provinceService.CreateProvinceAsync(provinceCreateDTO);
            return Ok(APIResponse<string>.Builder()
                .WithResult($"Province {provinceCreateDTO.Name} created successfully.")
                .WithStatusCode(HttpStatusCode.OK)
                .Build());
        }

        [HttpPut("update-province")]
        public async Task<ActionResult<APIResponse<string>>> UpdateProvinceAsync(int id, [FromBody] ProvinceUpdateDTO provinceUpdateDTO)
        {
            if (id == 0 || provinceUpdateDTO == null)
            {
                return BadRequest(APIResponse<string>.Builder()
                    .WithErrorMessages(new List<string> { "Province Id or Update DTO is null." })
                    .WithStatusCode(HttpStatusCode.BadRequest)
                    .Build());
            }

            await _provinceService.UpdateProvinceAsync(id, provinceUpdateDTO);
            return Ok(APIResponse<string>.Builder()
                .WithResult($"Province {provinceUpdateDTO.Name} updated successfully.")
                .WithStatusCode(HttpStatusCode.OK)
                .Build());
        }

        [HttpDelete("delete-province")]
        public async Task<ActionResult<APIResponse<string>>> DeleteProvinceAsync(int id)
        {
            if (id == 0)
            {
                return BadRequest(APIResponse<string>.Builder()
                    .WithErrorMessages(new List<string> { "Province Id is null." })
                    .WithStatusCode(HttpStatusCode.BadRequest)
                    .Build());
            }

            await _provinceService.DeleteProvinceAsync(id);
            return Ok(APIResponse<string>.Builder()
                .WithResult($"Province {id} deleted successfully.")
                .WithStatusCode(HttpStatusCode.OK)
                .Build());
        }

        [HttpGet("search-province-by-name")]
        public async Task<ActionResult<APIResponse<List<ProvinceDTO>>>> SearchProvincesAsync(string name)
        {
            if (name.IsNullOrEmpty())
            {
                return BadRequest(APIResponse<List<ProvinceDTO>>.Builder()
                    .WithErrorMessages(new List<string> { "Input is null or empty." })
                    .WithStatusCode(HttpStatusCode.BadRequest)
                    .Build());
            }

            var provinces = await _provinceService.SearchProvincesAsync(name);
            return Ok(APIResponse<List<ProvinceDTO>>.Builder()
                .WithResult(provinces)
                .WithStatusCode(HttpStatusCode.OK)
                .Build());
        }
    }
}
