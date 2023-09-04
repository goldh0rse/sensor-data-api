using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
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

        [HttpGet]
        public async Task<ActionResult<ServiceResponse<List<GetTemperatureDTO>>>> Get()
        {
            var result = await _temperatureService.GetAllTemperatures();
            if (!result.Success)
            {
                return BadRequest(new { message = "Bad Request" });
            }
            else if (result.Data is null)
            {
                return NotFound(new { message = "No temperatures found" });
            }
            return Ok(await _temperatureService.GetAllTemperatures());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceResponse<GetTemperatureDTO>>> GetSingle(int id)
        {
            var result = await _temperatureService.GetTemperatureById(id);
            if (result.Data is null)
            {
                return NotFound(new { Message = "Data not found" });
            }
            return Ok(await _temperatureService.GetTemperatureById(id));
        }

        [HttpPost]
        public async Task<ActionResult<ServiceResponse<GetTemperatureDTO>>> CreateTemperature(AddTemperatureDTO newTemperature)
        {
            var result = await _temperatureService.AddTemperature(newTemperature);
            if (!result.Success)
            {
                return BadRequest(new { message = "Bad Request" });
            }

            return Ok(await _temperatureService.AddTemperature(newTemperature));
        }

        [HttpDelete]
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