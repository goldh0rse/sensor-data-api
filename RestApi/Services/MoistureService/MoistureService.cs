using RestApi.DTO.Moisture;
using RestApi.Models;

namespace RestApi.Services.MoistureService
{
    public class MoistureService : IMoistureService
    {
        public MoistureService()
        {
        }

        public Task<ServiceResponse<GetMoistureDTO>> AddMoistureLvl(AddMoistureDTO newMoistureLvl)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<GetMoistureDTO>> DeleteMoistureById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<List<GetMoistureDTO>>> GetAllMoistureLvls(int page, int pageSize, string sortBy, bool ascending)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<List<GetMoistureDTO>>> GetMoistureByDatetimeSpan(DateTime from, DateTime to)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<GetMoistureDTO>> GetMoistureLvlById(int id)
        {
            throw new NotImplementedException();
        }
    }
}