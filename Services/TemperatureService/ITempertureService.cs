using rest_api.DTO.Temperature;
using rest_api.Models;

namespace rest_api.Services.TemperatureService
{
    public interface ITemperatureService
    {
        Task<ServiceResponse<List<GetTemperatureDTO>>> GetAllTemperatures();
        Task<ServiceResponse<GetTemperatureDTO>> GetTemperatureById(int id);
        Task<ServiceResponse<GetTemperatureDTO>> AddTemperature(AddTemperatureDTO newTemperature);
        Task<ServiceResponse<GetTemperatureDTO>> DeleteTemperatureById(int id);
        Task<ServiceResponse<List<GetTemperatureDTO>>> GetTemperaturesByDatetimeSpan(DateTime from, DateTime to);
    }
}
