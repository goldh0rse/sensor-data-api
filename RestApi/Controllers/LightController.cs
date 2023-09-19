using Microsoft.AspNetCore.Mvc;
using Prometheus;
using RestApi.DTO.Light;
using RestApi.Models;
using RestApi.Services.LightService;

namespace RestApi.Controllers {
    [ApiController]
    [Route("api/v1/[controller]")]
    public class LightController : ControllerBase {

        private readonly ILightService _lightService;
        private static readonly Gauge _lightGauge =
                    Metrics.CreateGauge(name: "app_light", help: "Light readings");

        public LightController(ILightService soilService) {
            this._lightService = soilService;
        }

        [HttpGet]
        public async Task<ActionResult<ServiceResponse<List<GetLightDTO>>>> Get(
            int page = 1,
            int pageSize = 10,
            string sortBy = "createdAt",
            bool ascending = true
        ){
            ServiceResponse<List<GetLightDTO>>? result = await _lightService.GetAllLightReadings(page, pageSize, sortBy, ascending);
            if (!result.Success) {
                return BadRequest(error: new { message = "Bad Request" });
            } else if (result.Data is null) {
                return NotFound(value: new { message = "No LightReadings found" });
            }
            return Ok(value: result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceResponse<GetLightDTO>>> GetLightReadingById(int id) {
            ServiceResponse<GetLightDTO>? result = await _lightService.GetLightReadingById(id);
            if (!result.Success){
                return BadRequest(error: new { message = "Bad Request" });
            } else if (result.Data is null) {
                return NotFound(value: new { message = "No Soil Reading found" });
            }
            return Ok(value: result);
        }

        [HttpGet("Span")]
        public async Task<ActionResult<ServiceResponse<List<GetLightDTO>>>> GetRecordsBetweenDates(
            [FromQuery] DateTime startDate, 
            [FromQuery] DateTime endDate)
        {
            var serviceResponse = await _lightService.GetLightReadingByDatetimeSpan(startDate, endDate);
            if(!serviceResponse.Success){
                return BadRequest( new { Message = "Bad Request"});
            }
            return Ok(serviceResponse.Data);
        }

        [HttpPost]
        public async Task<ActionResult<ServiceResponse<GetLightDTO>>> AddMoisture(AddLightDTO newLightReading) {
            ServiceResponse<GetLightDTO>? result = await _lightService.AddLightReading(newLightReading: newLightReading);
            if (!result.Success) {
                return BadRequest(error: new { message = "Bad Request" });
            }

            if (result.Data is not null) {
                _lightGauge.Set(val: result.Data.Lux);
            }


            return Ok(value: result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ServiceResponse<GetLightDTO>>> DeleteMoistureById(int id) {
            ServiceResponse<GetLightDTO>? result = await _lightService.DeleteLightReadingById(id);

            return Ok(value: result);
        }
    }
}