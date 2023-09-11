using RestApi.DTO.Moisture;
using RestApi.Models;

namespace RestApi.Services.MoistureService
{
    public interface IMoistureService
    {
        Task<ServiceResponse<List<GetMoistureDTO>>> GetAllMoistureLvls(
            int page,
            int pageSize,
            string sortBy,
            bool ascending
        );
        Task<ServiceResponse<GetMoistureDTO>> GetMoistureLvlById(int id);
        Task<ServiceResponse<GetMoistureDTO>> AddMoistureLvl(AddMoistureDTO newMoistureLvl);
        Task<ServiceResponse<GetMoistureDTO>> DeleteMoistureById(int id);
        Task<ServiceResponse<List<GetMoistureDTO>>> GetMoistureByDatetimeSpan(DateTime from, DateTime to);
    }
}