using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RestApi.Data;
using RestApi.DTO.Soil;
using RestApi.Extensions;
using RestApi.Models;

// TODO: Fix moisture labeling

namespace RestApi.Services.SoilService
{
    public class MoistureService : ILightService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;

        public MoistureService(IMapper mapper, DataContext context) {
            this._mapper = mapper;
            this._context = context;
        }

        public async Task<ServiceResponse<GetSoilDTO>> AddSoilReading(AddSoilDTO newMoistureLvl) {
            ServiceResponse<GetSoilDTO>? serviceResponse = new();

            // Validate the incoming data
            if (newMoistureLvl == null) {
                serviceResponse.Success = false;
                serviceResponse.Message = "Invalid temperature data.";
                return serviceResponse;
            }

            // Begin a new transaction
            using var transaction = await _context.Database.BeginTransactionAsync();

            try {
                // Map the DTO to the database model
                Soil? moistureLvl = _mapper.Map<Soil>(source: newMoistureLvl) ?? throw new Exception(message: "Mapping failed.");

                // Add the Temperature to the DbContext
                await _context.SoilReadings.AddAsync(entity: moistureLvl);

                // Save the changes to the database
                await _context.SaveChangesAsync();

                // Commit the transaction
                await transaction.CommitAsync();

                // Fetch the uploaded temperature
                Soil? dbMoistureLvl = await _context.SoilReadings.FirstOrDefaultAsync(predicate: m => m.Id == moistureLvl.Id);

                // Map the database models to DTOs
                serviceResponse.Data = _mapper.Map<GetSoilDTO>(source: dbMoistureLvl);

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

        public async Task<ServiceResponse<GetSoilDTO>> DeleteSoilReadingById(int id) {
            var serviceResponse = new ServiceResponse<GetSoilDTO>();

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
                var dbMoistureLvl = await _context.SoilReadings.FirstOrDefaultAsync(m => m.Id == id);

                // If the temperature doesn't exist, throw an exception
                if (dbMoistureLvl is null) {
                    throw new Exception($"Record with id '{id}' was not found");
                }

                // Remove the Temperature from the DbContext
                _context.SoilReadings.Remove(dbMoistureLvl);

                // Save changes to the database
                await _context.SaveChangesAsync();

                // Commit the transaction
                await transaction.CommitAsync();

                // Map the deleted database entity to the DTO
                serviceResponse.Data = _mapper.Map<GetSoilDTO>(dbMoistureLvl);
            } catch (Exception ex) {
                // Rollback the transaction
                await transaction.RollbackAsync();
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetSoilDTO>>> GetAllSoilReadings(int page, int pageSize, string sortBy, bool ascending) {
            var serviceResponse = new ServiceResponse<List<GetSoilDTO>>();

            try {
                // Error handling: Ensure the sort column exists
                var hasProperty = typeof(Temperature).GetProperty(sortBy) != null;
                if (!hasProperty) {
                    serviceResponse.Success = false;
                    serviceResponse.Message = "Invalid sort column.";
                    return serviceResponse;
                }

                // Pagination and sorting
                var moistureLvls = _context.SoilReadings
                    .OrderByPropertyName(sortBy, ascending)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize);

                // Fetch from database
                var dbMoistureLvls = await moistureLvls.ToListAsync();

                // Check for empty data
                if (!dbMoistureLvls.Any()) {
                    serviceResponse.Message = "Moisture level records found.";
                }

                // Map and set data
                serviceResponse.Data = dbMoistureLvls.Select(t => _mapper.Map<GetSoilDTO>(t)).ToList();
                serviceResponse.Success = true;
            } catch (Exception ex) {
                // Log error (implement your logging mechanism)

                serviceResponse.Success = false;
                serviceResponse.Message = $"An error occurred: {ex.Message}";
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetSoilDTO>>> GetSoilReadingByDatetimeSpan(DateTime from, DateTime to) {
            ServiceResponse<List<GetSoilDTO>>? serviceResponse = new();

            // Validate input DateTime range
            if (from > to) {
                serviceResponse.Success = false;
                serviceResponse.Message = "Invalid date range: 'from' date cannot be later than 'to' date.";
                return serviceResponse;
            }

            try {
                // Fetch temperatures within the DateTime span
                List<Soil>? dbMoistureLvls = await _context.SoilReadings
                    .Where(m => m.CreatedAt >= from && m.CreatedAt <= to)
                    .ToListAsync();

                if (dbMoistureLvls == null || !dbMoistureLvls.Any()) {
                    serviceResponse.Data = new List<GetSoilDTO>();  // Return empty list instead of throwing exception
                    serviceResponse.Message = "No moisture records found for the given time span.";
                } else {
                    serviceResponse.Data = dbMoistureLvls.Select(selector: _mapper.Map<GetSoilDTO>).ToList();
                }
            } catch (Exception ex) {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetSoilDTO>> GetSoilReadingById(int id) {
            var serviceResponse = new ServiceResponse<GetSoilDTO>();

            try {
                // Fetch the temperature by ID
                var dbTemperature = await _context.SoilReadings.FindAsync(id);

                // Check if the temperature exists
                if (dbTemperature == null) {
                    serviceResponse.Success = false;
                    serviceResponse.Message = $"Moisture level with ID {id} not found.";
                    return serviceResponse;
                }

                // Map the database entity to the DTO
                serviceResponse.Data = _mapper.Map<GetSoilDTO>(dbTemperature);
                serviceResponse.Success = true;
            } catch (Exception ex) {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }
            return serviceResponse;
        }
    }
}