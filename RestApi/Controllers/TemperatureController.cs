using Microsoft.AspNetCore.Mvc;
using RestApi.DTO.Temperature;
using RestApi.Models;
using RestApi.Services.TemperatureService;

namespace RestApi.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class TemperatureController : ControllerBase
    {
        private readonly ITemperatureService _temperatureService;

        public TemperatureController(ITemperatureService temperatureService)
        {
            _temperatureService = temperatureService;
        }

        [HttpGet]
        public async Task<ActionResult<ServiceResponse<List<GetTemperatureDTO>>>> Get(
            int page = 1,
            int pageSize = 10,
            string sortBy = "DateTime",
            bool ascending = true)
        {
            var result = await _temperatureService.GetAllTemperatures(page, pageSize, sortBy, ascending);
            if (!result.Success)
            {
                return BadRequest(new { message = "Bad Request" });
            }
            else if (result.Data is null)
            {
                return NotFound(new { message = "No temperatures found" });
            }
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceResponse<GetTemperatureDTO>>> GetSingle(int id)
        {
            var result = await _temperatureService.GetTemperatureById(id);
            if (result.Data is null)
            {
                return NotFound(new { Message = "Data not found" });
            }
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<ServiceResponse<GetTemperatureDTO>>> CreateTemperature(AddTemperatureDTO newTemperature)
        {
            var result = await _temperatureService.AddTemperature(newTemperature);
            if (!result.Success)
            {
                return BadRequest(new { message = "Bad Request" });
            }

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ServiceResponse<GetTemperatureDTO>>> DeleteTemperature(int id)
        {
            var response = await _temperatureService.DeleteTemperatureById(id);
            if (!response.Success)
            {
                return BadRequest(new { Message = "Failed to delete" });
            }
            else if (response.Data is null)
            {
                return NotFound(new { message = "Resource not found" });
            }
            return Ok(response);
        }
    }
}
