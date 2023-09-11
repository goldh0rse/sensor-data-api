using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RestApi.Data;
using RestApi.DTO.Moisture;
using RestApi.Extensions;
using RestApi.Models;

namespace RestApi.Services.MoistureService
{
    public class MoistureService : IMoistureService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;

        public MoistureService(IMapper mapper, DataContext context)
        {
            this._mapper = mapper;
            this._context = context;
        }

        public async Task<ServiceResponse<GetMoistureDTO>> AddMoistureLvl(AddMoistureDTO newMoistureLvl)
        {
            var serviceResponse = new ServiceResponse<GetMoistureDTO>();

            // Validate the incoming data
            if (newMoistureLvl == null)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "Invalid temperature data.";
                return serviceResponse;
            }

            // Begin a new transaction
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Map the DTO to the database model
                var moistureLvl = _mapper.Map<Moisture>(newMoistureLvl);

                // Validate the mapped object
                if (moistureLvl == null)
                {
                    throw new Exception("Mapping failed.");
                }

                // Add the Temperature to the DbContext
                await _context.MoistureLvls.AddAsync(moistureLvl);

                // Save the changes to the database
                await _context.SaveChangesAsync();

                // Commit the transaction
                await transaction.CommitAsync();

                // Fetch the uploaded temperature
                var dbMoistureLvl = await _context.MoistureLvls.FirstOrDefaultAsync(m => m.Id == moistureLvl.Id);

                // Map the database models to DTOs
                serviceResponse.Data = _mapper.Map<GetMoistureDTO>(dbMoistureLvl);

                // Validate the returned data
                if (serviceResponse.Data == null)
                {
                    throw new Exception("Fetching saved data failed.");
                }
            }
            catch (Exception ex)
            {
                // Rollback transaction
                await transaction.RollbackAsync();
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetMoistureDTO>> DeleteMoistureById(int id)
        {
            var serviceResponse = new ServiceResponse<GetMoistureDTO>();

            // Validate the incoming ID
            if (id <= 0)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "Invalid temperature ID.";
                return serviceResponse;
            }

            // Begin a new transaction
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Fetch the temperature from the database
                var dbMoistureLvl = await _context.MoistureLvls.FirstOrDefaultAsync(m => m.Id == id);

                // If the temperature doesn't exist, throw an exception
                if (dbMoistureLvl is null)
                {
                    throw new Exception($"Record with id '{id}' was not found");
                }

                // Remove the Temperature from the DbContext
                _context.MoistureLvls.Remove(dbMoistureLvl);

                // Save changes to the database
                await _context.SaveChangesAsync();

                // Commit the transaction
                await transaction.CommitAsync();

                // Map the deleted database entity to the DTO
                serviceResponse.Data = _mapper.Map<GetMoistureDTO>(dbMoistureLvl);
            }
            catch (Exception ex)
            {
                // Rollback the transaction
                await transaction.RollbackAsync();
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetMoistureDTO>>> GetAllMoistureLvls(int page, int pageSize, string sortBy, bool ascending)
        {
            var serviceResponse = new ServiceResponse<List<GetMoistureDTO>>();

            try
            {
                // Error handling: Ensure the sort column exists
                var hasProperty = typeof(Temperature).GetProperty(sortBy) != null;
                if (!hasProperty)
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = "Invalid sort column.";
                    return serviceResponse;
                }

                // Pagination and sorting
                var moistureLvls = _context.MoistureLvls
                    .OrderByPropertyName(sortBy, ascending)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize);

                // Fetch from database
                var dbMoistureLvls = await moistureLvls.ToListAsync();

                // Check for empty data
                if (!dbMoistureLvls.Any())
                {
                    serviceResponse.Message = "Moisture level records found.";
                }

                // Map and set data
                serviceResponse.Data = dbMoistureLvls.Select(t => _mapper.Map<GetMoistureDTO>(t)).ToList();
                serviceResponse.Success = true;
            }
            catch (Exception ex)
            {
                // Log error (implement your logging mechanism)

                serviceResponse.Success = false;
                serviceResponse.Message = $"An error occurred: {ex.Message}";
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetMoistureDTO>>> GetMoistureByDatetimeSpan(DateTime from, DateTime to)
        {
            var serviceResponse = new ServiceResponse<List<GetMoistureDTO>>();

            // Validate input DateTime range
            if (from > to)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "Invalid date range: 'from' date cannot be later than 'to' date.";
                return serviceResponse;
            }

            try
            {
                // Fetch temperatures within the DateTime span
                var dbMoistureLvls = await _context.MoistureLvls
                    .Where(m => m.CreatedAt >= from && m.CreatedAt <= to)
                    .ToListAsync();

                if (dbMoistureLvls == null || !dbMoistureLvls.Any())
                {
                    serviceResponse.Data = new List<GetMoistureDTO>();  // Return empty list instead of throwing exception
                    serviceResponse.Message = "No moisture records found for the given time span.";
                }
                else
                {
                    serviceResponse.Data = dbMoistureLvls.Select(t => _mapper.Map<GetMoistureDTO>(t)).ToList();
                }
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetMoistureDTO>> GetMoistureLvlById(int id)
        {
            var serviceResponse = new ServiceResponse<GetMoistureDTO>();

            try
            {
                // Fetch the temperature by ID
                var dbTemperature = await _context.MoistureLvls.FindAsync(id);

                // Check if the temperature exists
                if (dbTemperature == null)
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = $"Moisture level with ID {id} not found.";
                    return serviceResponse;
                }

                // Map the database entity to the DTO
                serviceResponse.Data = _mapper.Map<GetMoistureDTO>(dbTemperature);
                serviceResponse.Success = true;
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }
            return serviceResponse;
        }
    }
}