using AutoMapper;
using RestApi.DTO.Character;
using RestApi.DTO.Temperature;
using RestApi.Models;

namespace RestApi
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Character, GetCharacterDTO>();
            CreateMap<AddCharacterDTO, Character>();
            CreateMap<UpdateCharacterDTO, Character>();
            CreateMap<Temperature, GetTemperatureDTO>();
            CreateMap<AddTemperatureDTO, Temperature>();
        }
    }
}