using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RestApi.Data;
using RestApi.DTO.Light;
using RestApi.Models;

namespace RestApi.Services.LightService {
    public class LightService : ILightService {

        private readonly IMapper _mapper;
        private readonly DataContext _context;

        public LightService(IMapper mapper, DataContext context){
            this._mapper = mapper;
            this._context = context;
        }
        public async Task<ServiceResponse<GetLightDTO>> AddLightReading(AddLightDTO newLightReading) {
            ServiceResponse<GetLightDTO>? serviceResponse = new ServiceResponse<GetLightDTO>();

            // Validate the incoming data
            if (newLightReading == null) {
                serviceResponse.Success = false;
                serviceResponse.Message = "Invalid temperature data.";
                return serviceResponse;
            }

            // Begin a new transaction
            using var transaction = await _context.Database.BeginTransactionAsync();

            try {
                // Map the DTO to the database model
                Light? light = _mapper.Map<Light>(source: newLightReading) ?? throw new Exception(message: "Mapping failed.");

                // Add the Temperature to the DbContext
                await _context.LightReadings.AddAsync(entity: light);

                // Save the changes to the database
                await _context.SaveChangesAsync();

                // Commit the transaction
                await transaction.CommitAsync();

                // Fetch the uploaded temperature
                Light? dbLightReading = await _context.LightReadings.FirstOrDefaultAsync(predicate: m => m.Id == light.Id);

                // Map the database models to DTOs
                serviceResponse.Data = _mapper.Map<GetLightDTO>(source: dbLightReading);

                // Validate the returned data
                if (serviceResponse.Data == null) {
                    throw new Exception(message: "Fetching saved data failed.");
                }
            } catch (Exception ex) {
                // Rollback transaction
                await transaction.RollbackAsync();
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }
            return serviceResponse;
        }

        public Task<ServiceResponse<GetLightDTO>> DeleteLightReadingById(int id) {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<List<GetLightDTO>>> GetAllLightReadings(int page, int pageSize, string sortBy, bool ascending) {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<GetLightDTO>> GetLightReadingById(int id) {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<List<GetLightDTO>>> GetSoilReadingByDatetimeSpan(DateTime from, DateTime to) {
            throw new NotImplementedException();
        }
    }
}