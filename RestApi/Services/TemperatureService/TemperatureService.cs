using AutoMapper;
using Microsoft.EntityFrameworkCore;
using rest_api.Data;
using rest_api.DTO.Temperature;
using rest_api.Models;
using RestApi.Extensions;

namespace rest_api.Services.TemperatureService
{
    public class TemperatureService : ITemperatureService
    {
        private readonly IMapper _mapper;
        public readonly DataContext _context;

        public TemperatureService(IMapper mapper, DataContext context)
        {
            this._mapper = mapper;
            this._context = context;
        }

        public async Task<ServiceResponse<List<GetTemperatureDTO>>> GetAllTemperatures(
            int page = 1,
            int pageSize = 10,
            string sortBy = "DateTime",
            bool ascending = true)
        {
            var serviceResponse = new ServiceResponse<List<GetTemperatureDTO>>();

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
                var temperatures = _context.Temperatures
                    .OrderByPropertyName(sortBy, ascending)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize);

                // Fetch from database
                var dbTemperatures = await temperatures.ToListAsync();

                // Check for empty data
                if (!dbTemperatures.Any())
                {
                    serviceResponse.Message = "No temperature records found.";
                }

                // Map and set data
                serviceResponse.Data = dbTemperatures.Select(t => _mapper.Map<GetTemperatureDTO>(t)).ToList();
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


        public async Task<ServiceResponse<GetTemperatureDTO>> AddTemperature(AddTemperatureDTO newTemperature)
        {
            var serviceResponse = new ServiceResponse<GetTemperatureDTO>();

            // Validate the incoming data
            if (newTemperature == null)
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
                var temperature = _mapper.Map<Temperature>(newTemperature);

                // Validate the mapped object
                if (temperature == null)
                {
                    throw new Exception("Mapping failed.");
                }

                // Generate a DateTime variable and add it to the temperature object
                temperature.DateTime = DateTime.UtcNow;

                // Add the Temperature to the DbContext
                await _context.Temperatures.AddAsync(temperature);

                // Save the changes to the database
                await _context.SaveChangesAsync();

                // Commit the transaction
                await transaction.CommitAsync();

                // Fetch the uploaded temperature
                var dbTemperature = await _context.Temperatures.FirstOrDefaultAsync(t => t.Id == temperature.Id);

                // Map the database models to DTOs
                serviceResponse.Data = _mapper.Map<GetTemperatureDTO>(dbTemperature);

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

        public async Task<ServiceResponse<GetTemperatureDTO>> DeleteTemperatureById(int id)
        {
            var serviceResponse = new ServiceResponse<GetTemperatureDTO>();

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
                var dbTemperature = await _context.Temperatures.FirstOrDefaultAsync(t => t.Id == id);

                // If the temperature doesn't exist, throw an exception
                if (dbTemperature is null)
                {
                    throw new Exception($"Temperature with id '{id}' was not found");
                }

                // Remove the Temperature from the DbContext
                _context.Temperatures.Remove(dbTemperature);

                // Save changes to the database
                await _context.SaveChangesAsync();

                // Commit the transaction
                await transaction.CommitAsync();

                // Map the deleted database entity to the DTO
                serviceResponse.Data = _mapper.Map<GetTemperatureDTO>(dbTemperature);
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


        public async Task<ServiceResponse<List<GetTemperatureDTO>>> GetTemperaturesByDatetimeSpan(DateTime from, DateTime to)
        {
            var serviceResponse = new ServiceResponse<List<GetTemperatureDTO>>();

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
                var dbTemperatures = await _context.Temperatures
                    .Where(t => t.DateTime >= from && t.DateTime <= to)
                    .ToListAsync();

                if (dbTemperatures == null || !dbTemperatures.Any())
                {
                    serviceResponse.Data = new List<GetTemperatureDTO>();  // Return empty list instead of throwing exception
                    serviceResponse.Message = "No temperature records found for the given time span.";
                }
                else
                {
                    serviceResponse.Data = dbTemperatures.Select(t => _mapper.Map<GetTemperatureDTO>(t)).ToList();
                }
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }
            return serviceResponse;
        }


        public async Task<ServiceResponse<GetTemperatureDTO>> GetTemperatureById(int id)
        {
            var serviceResponse = new ServiceResponse<GetTemperatureDTO>();

            try
            {
                // Fetch the temperature by ID
                var dbTemperature = await _context.Temperatures.FindAsync(id);

                // Check if the temperature exists
                if (dbTemperature == null)
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = $"Temperature with ID {id} not found.";
                    return serviceResponse;
                }

                // Map the database entity to the DTO
                serviceResponse.Data = _mapper.Map<GetTemperatureDTO>(dbTemperature);
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