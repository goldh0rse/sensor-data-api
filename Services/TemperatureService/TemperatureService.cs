using AutoMapper;
using Microsoft.EntityFrameworkCore;
using rest_api.Data;
using rest_api.DTO.Temperature;
using rest_api.Models;

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

        public async Task<ServiceResponse<List<GetTemperatureDTO>>> GetAllTemperatures()
        {
            var dbTemperatures = await _context.Temperatures.ToListAsync();
            var serviceResponse = new ServiceResponse<List<GetTemperatureDTO>>
            {
                Data = dbTemperatures.Select(t => _mapper.Map<GetTemperatureDTO>(t)).ToList()
            };
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetTemperatureDTO>> AddTemperature(AddTemperatureDTO newTemperature)
        {
            var serviceResponse = new ServiceResponse<GetTemperatureDTO>();

            try
            {
                // Map the DTO to the database model
                var temperature = _mapper.Map<Temperature>(newTemperature);

                // Generate a DateTime variable and add it to the temperature object
                temperature.DateTime = DateTime.UtcNow;

                // Add the Temperature to the DbContext
                await _context.Temperatures.AddAsync(temperature);

                // Save the changes to the database
                await _context.SaveChangesAsync();

                // Fetch the uploaded temperature
                var dbTemperature = await _context.Temperatures.FirstOrDefaultAsync(t => t.Id == temperature.Id);

                // Map the database models to DTOs
                serviceResponse.Data = _mapper.Map<GetTemperatureDTO>(dbTemperature);
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<GetTemperatureDTO>> DeleteTemperatureById(int id)
        {
            var serviceResponse = new ServiceResponse<GetTemperatureDTO>();
            try
            {
                // Fetch the temperature from the database
                var dbTemperature = await _context.Temperatures.FirstOrDefaultAsync(t => t.Id == id);

                // If the temperature doesn't exist, throw an exception
                if (dbTemperature is null)
                {
                    throw new Exception($"Temperatures wid id '{id}' was not found");
                };

                // Map the updated fields onto the database entity
                _context.Temperatures.Remove(dbTemperature);

                // Update the temperature in the database
                await _context.SaveChangesAsync();

                // Map the updated database entity to the DTO
                serviceResponse.Data = _mapper.Map<GetTemperatureDTO>(dbTemperature);
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetTemperatureDTO>>> GetTemperaturesByDatetimeSpan(DateTime from, DateTime to)
        {
            var serviceResponse = new ServiceResponse<List<GetTemperatureDTO>>();

            try
            {
                var dbTemperatures = await _context.Temperatures
                    .Where(t => t.DateTime >= from && t.DateTime <= to)
                    .ToListAsync();

                if (dbTemperatures == null || !dbTemperatures.Any())
                {
                    throw new Exception("No temperature records found for the given time span.");
                }

                serviceResponse.Data = dbTemperatures.Select(t => _mapper.Map<GetTemperatureDTO>(t)).ToList();
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
            var dbTemperature = await _context.Temperatures.FirstOrDefaultAsync(t => t.Id == id);
            var serviceResponse = new ServiceResponse<GetTemperatureDTO>
            {
                Data = _mapper.Map<GetTemperatureDTO>(dbTemperature)
            };

            return serviceResponse;
        }
    }
}