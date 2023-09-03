using AutoMapper;
using rest_api.DTO.Character;
using rest_api.DTO.Temperature;
using rest_api.Models;

namespace rest_api
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