using Microsoft.AspNetCore.Mvc;
using Prometheus;
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

        // Assuming the gauge metric is globally accessible.
        // You could also inject this via the constructor if it's registered in your DI container.
        private static readonly Gauge _temperatureGauge =
            Metrics.CreateGauge("app_temperature", "Temperature readings");

        public TemperatureController(ITemperatureService temperatureService)
        {
            _temperatureService = temperatureService;
        }

        [HttpGet]
        public async Task<ActionResult<ServiceResponse<List<GetTemperatureDTO>>>> Get(
            int page = 1,
            int pageSize = 10,
            string sortBy = "CreatedAt",
            bool ascending = true)
        {
            var result = await _temperatureService.GetAllTemperatures(page, pageSize, sortBy, ascending);
            if (!result.Success)
            {
                System.Console.WriteLine(result.Message);
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

        [HttpGet("Span")]
        public async Task<ActionResult<ServiceResponse<List<GetTemperatureDTO>>>> GetRecordsBetweenDates(
            [FromQuery] DateTime startDate, 
            [FromQuery] DateTime endDate)
        {
            var serviceResponse = await _temperatureService.GetTemperaturesByDatetimeSpan(startDate, endDate);
            if(!serviceResponse.Success){
                return BadRequest( new { Message = "Bad Request"});
            }
            return Ok(serviceResponse.Data);
        }

        [HttpPost]
        public async Task<ActionResult<ServiceResponse<GetTemperatureDTO>>> CreateTemperature(AddTemperatureDTO newTemperature)
        {
            var result = await _temperatureService.AddTemperature(newTemperature);
            if (!result.Success)
            {
                return BadRequest(new { message = "Bad Request" });
            }

            if (result.Data is not null)
            {
                _temperatureGauge.Set(result.Data.Temp);
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
