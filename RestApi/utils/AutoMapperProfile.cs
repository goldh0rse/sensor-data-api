using AutoMapper;
using RestApi.DTO.Moisture;
using RestApi.DTO.Temperature;
using RestApi.Models;

namespace RestApi
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Temperature, GetTemperatureDTO>();
            CreateMap<AddTemperatureDTO, Temperature>();
            CreateMap<Moisture, GetMoistureDTO>();
            CreateMap<AddMoistureDTO, Moisture>();
        }
    }
}