using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RestApi.Data;
using RestApi.DTO.Light;
using RestApi.Extensions;
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
                Light? dbLightReading = await _context.LightReadings.FirstOrDefaultAsync(predicate: l => l.Id == light.Id);

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

        public async Task<ServiceResponse<GetLightDTO>> DeleteLightReadingById(int id) {
            var serviceResponse = new ServiceResponse<GetLightDTO>();

            // Validate the incoming ID
            if (id <= 0) {
                serviceResponse.Success = false;
                serviceResponse.Message = "Invalid ID.";
                return serviceResponse;
            }

            // Begin a new transaction
            using var transaction = await _context.Database.BeginTransactionAsync();

            try {
                // Fetch the temperature from the database
                var dbLightReading = await _context.LightReadings.FirstOrDefaultAsync(predicate: l => l.Id == id);

                // If the temperature doesn't exist, throw an exception
                if (dbLightReading is null) {
                    throw new Exception($"Record with id '{id}' was not found");
                }

                // Remove the Temperature from the DbContext
                _context.LightReadings.Remove(dbLightReading);

                // Save changes to the database
                await _context.SaveChangesAsync();

                // Commit the transaction
                await transaction.CommitAsync();

                // Map the deleted database entity to the DTO
                serviceResponse.Data = _mapper.Map<GetLightDTO>(dbLightReading);
            } catch (Exception ex) {
                // Rollback the transaction
                await transaction.RollbackAsync();
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetLightDTO>>> GetAllLightReadings(int page, int pageSize, string sortBy, bool ascending) {
            ServiceResponse<List<GetLightDTO>> serviceResponse = new ServiceResponse<List<GetLightDTO>>();

            try {
                // Error handling: Ensure the sort column exists
                var hasProperty = typeof(Light).GetProperty(sortBy) != null;
                if (!hasProperty) {
                    serviceResponse.Success = false;
                    serviceResponse.Message = "Invalid sort column.";
                    return serviceResponse;
                }

                // Pagination and sorting
                var lightReadings = _context.LightReadings
                    .OrderByPropertyName(sortBy, ascending)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize);

                // Fetch from database
                var dbLightReadings = await lightReadings.ToListAsync();

                // Check for empty data
                if (!dbLightReadings.Any()) {
                    serviceResponse.Message = "LightReading records found.";
                }

                // Map and set data
                serviceResponse.Data = dbLightReadings.Select(t => _mapper.Map<GetLightDTO>(t)).ToList();
                serviceResponse.Success = true;
            } catch (Exception ex) {
                // Log error (implement your logging mechanism)

                serviceResponse.Success = false;
                serviceResponse.Message = $"An error occurred: {ex.Message}";
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetLightDTO>> GetLightReadingById(int id) {
            var serviceResponse = new ServiceResponse<GetLightDTO>();

            try {
                // Fetch the temperature by ID
                var dbLightReading = await _context.LightReadings.FindAsync(id);

                // Check if the temperature exists
                if (dbLightReading == null) {
                    serviceResponse.Success = false;
                    serviceResponse.Message = $"LightReading with ID {id} not found.";
                    return serviceResponse;
                }

                // Map the database entity to the DTO
                serviceResponse.Data = _mapper.Map<GetLightDTO>(dbLightReading);
                serviceResponse.Success = true;
            } catch (Exception ex) {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetLightDTO>>> GetLightReadingByDatetimeSpan(DateTime from, DateTime to) {
            ServiceResponse<List<GetLightDTO>>? serviceResponse = new();

            // Validate input DateTime range
            if (from > to) {
                serviceResponse.Success = false;
                serviceResponse.Message = "Invalid date range: 'from' date cannot be later than 'to' date.";
                return serviceResponse;
            }

            try {
                // Fetch temperatures within the DateTime span
                List<Light>? dbLightReadings = await _context.LightReadings
                    .Where(l => l.CreatedAt >= from && l.CreatedAt <= to)
                    .ToListAsync();

                if (dbLightReadings == null || !dbLightReadings.Any()) {
                    serviceResponse.Data = new List<GetLightDTO>();  // Return empty list instead of throwing exception
                    serviceResponse.Message = "No LightRedings found for the given time span.";
                } else {
                    serviceResponse.Data = dbLightReadings.Select(selector: _mapper.Map<GetLightDTO>).ToList();
                }
            } catch (Exception ex) {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }
            return serviceResponse;
        }
    }
}