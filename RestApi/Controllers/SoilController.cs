using Microsoft.AspNetCore.Mvc;
using Prometheus;
using RestApi.DTO.Soil;
using RestApi.Models;
using RestApi.Services.SoilService;

namespace RestApi.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class SoilController : ControllerBase
    {
        private readonly ISoilService _soilService;
        private static readonly Gauge _soilMoistureGauge =
                    Metrics.CreateGauge(name: "app_soil_moisture", help: "Soil Moisture readings");
        private static readonly Gauge _soilTemperatureGauge =
                        Metrics.CreateGauge(name: "app_soil_temperature", help: "Soil Temperature readings");
        public SoilController(ISoilService soilService)
        {
            this._soilService = soilService;
        }

        [HttpGet]
        public async Task<ActionResult<ServiceResponse<List<GetSoilDTO>>>> Get(
            int page = 1,
            int pageSize = 10,
            string sortBy = "createdAt",
            bool ascending = true
        ){
            ServiceResponse<List<GetSoilDTO>>? result = await _soilService.GetAllSoilReadings(page, pageSize, sortBy, ascending);
            if (!result.Success)
            {
                return BadRequest(error: new { message = "Bad Request" });
            }
            else if (result.Data is null)
            {
                return NotFound(value: new { message = "No Moisture levels found" });
            }
            return Ok(value: result);
        }

        [HttpGet]
        public async Task<ActionResult<ServiceResponse<GetSoilDTO>>> GetMoistureById(int id){
            ServiceResponse<GetSoilDTO>? result = await _soilService.GetSoilReadingById(id);
            if (!result.Success)
            {
                return BadRequest(error: new { message = "Bad Request" });
            }
            else if (result.Data is null)
            {
                return NotFound(value: new { message = "No Soil Reading found" });
            }
            return Ok(value: result);
        }

        public async Task<ActionResult<ServiceResponse<GetSoilDTO>>> AddMoisture(AddSoilDTO newMoisture){
            ServiceResponse<GetSoilDTO>? result = await _soilService.AddSoilReading(newMoistureLvl: newMoisture);
            if (!result.Success)
            {
                return BadRequest(error: new { message = "Bad Request" });
            }

            if (result.Data is not null)
            {
                _soilMoistureGauge.Set(val: result.Data.Moisture);
                _soilTemperatureGauge.Set(val: result.Data.Temperature);
            }


            return Ok(value: result);
        }

        public async Task<ActionResult<ServiceResponse<GetSoilDTO>>> DeleteMoistureById(int id)
        {
            ServiceResponse<GetSoilDTO>? result = await _soilService.DeleteSoilReadingById(id);

            return Ok(value: result);
        }
    }
}