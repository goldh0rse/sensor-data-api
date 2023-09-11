using Microsoft.AspNetCore.Mvc;
using Prometheus;
using RestApi.DTO.Moisture;
using RestApi.Models;
using RestApi.Services.MoistureService;

namespace RestApi.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class MoistureController : ControllerBase
    {
        private readonly IMoistureService _moistureService;
        private static readonly Gauge _moistureGauge =
                    Metrics.CreateGauge("app_moisture", "Moisture readings");
        public MoistureController(IMoistureService moistureService)
        {
            this._moistureService = moistureService;
        }

        [HttpGet]
        public async Task<ActionResult<ServiceResponse<List<GetMoistureDTO>>>> Get(
            int page = 1,
            int pageSize = 10,
            string sortBy = "createdAt",
            bool ascending = true
        )
        {
            var result = await _moistureService.GetAllMoistureLvls(page, pageSize, sortBy, ascending);
            if (!result.Success)
            {
                return BadRequest(new { message = "Bad Request" });
            }
            else if (result.Data is null)
            {
                return NotFound(new { message = "No Moisture levels found" });
            }
            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<ServiceResponse<GetMoistureDTO>>> GetMoistureById(int id)
        {
            var result = await _moistureService.GetMoistureLvlById(id);
            if (!result.Success)
            {
                return BadRequest(new { message = "Bad Request" });
            }
            else if (result.Data is null)
            {
                return NotFound(new { message = "No Moisture level found" });
            }
            return Ok(result);
        }

        public async Task<ActionResult<ServiceResponse<GetMoistureDTO>>> AddMoisture(AddMoistureDTO newMoisture)
        {
            var result = await _moistureService.AddMoistureLvl(newMoisture);
            if (!result.Success)
            {
                return BadRequest(new { message = "Bad Request" });
            }

            if (result.Data is not null)
            {
                _moistureGauge.Set(result.Data.MoistureLvl);
            }


            return Ok(result);
        }

        public async Task<ActionResult<ServiceResponse<GetMoistureDTO>>> DeleteMoistureById(int id)
        {
            var result = await _moistureService.DeleteMoistureById(id);

            return Ok(result);
        }
    }
}