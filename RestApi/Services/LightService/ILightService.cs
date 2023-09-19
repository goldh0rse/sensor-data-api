using RestApi.DTO.Light;
using RestApi.Models;

namespace RestApi.Services.LightService {
    public interface ILightService {
        Task<ServiceResponse<List<GetLightDTO>>> GetAllLightReadings(
            int page,
            int pageSize,
            string sortBy,
            bool ascending
        );
        Task<ServiceResponse<GetLightDTO>> GetLightReadingById(int id);
        Task<ServiceResponse<GetLightDTO>> AddLightReading(AddLightDTO newLightReading);
        Task<ServiceResponse<GetLightDTO>> DeleteLightReadingById(int id);
        Task<ServiceResponse<List<GetLightDTO>>> GetSoilReadingByDatetimeSpan(DateTime from, DateTime to);
    }
}