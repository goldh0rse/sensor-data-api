using Microsoft.AspNetCore.Mvc;
using rest_api.DTO.Temperature;
using rest_api.Models;
using rest_api.Services.TemperatureService;

namespace rest_api.Controllers
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

        [HttpGet("GetAll")]
        public async Task<ActionResult<ServiceResponse<List<GetTemperatureDTO>>>> Get()
        {
            return Ok(await _temperatureService.GetAllTemperatures());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceResponse<GetTemperatureDTO>>> GetSingle(int id)
        {
            return Ok(await _temperatureService.GetTemperatureById(id));
        }

        [HttpPost]
        public async Task<ActionResult<ServiceResponse<GetTemperatureDTO>>> CreateTemperature(AddTemperatureDTO newTemperature)
        {
            return Ok(await _temperatureService.AddTemperature(newTemperature));
        }

        [HttpDelete]
        public async Task<ActionResult<ServiceResponse<GetTemperatureDTO>>> DeleteTemperature(int id)
        {
            var response = await _temperatureService.DeleteTemperatureById(id);
            if (response.Data is null)
            {
                return NotFound(response);
            }
            else
            {
                return Ok(response);
            }
        }
    }
}