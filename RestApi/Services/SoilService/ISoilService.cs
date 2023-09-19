using RestApi.DTO.Soil;
using RestApi.Models;

namespace RestApi.Services.SoilService {
    public interface ILightService {
        Task<ServiceResponse<List<GetSoilDTO>>> GetAllSoilReadings(
            int page,
            int pageSize,
            string sortBy,
            bool ascending
        );
        Task<ServiceResponse<GetSoilDTO>> GetSoilReadingById(int id);
        Task<ServiceResponse<GetSoilDTO>> AddSoilReading(AddSoilDTO newMoistureLvl);
        Task<ServiceResponse<GetSoilDTO>> DeleteSoilReadingById(int id);
        Task<ServiceResponse<List<GetSoilDTO>>> GetSoilReadingByDatetimeSpan(DateTime from, DateTime to);
    }
}