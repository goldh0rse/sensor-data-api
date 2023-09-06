using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RestApi.Data;
using RestApi.DTO.Character;
using RestApi.Models;

namespace RestApi.Services.CharacterService
{

    public class CharacterService : ICharacterService
    {
        private readonly IMapper _mapper;
        public readonly DataContext _context;

        public CharacterService(IMapper mapper, DataContext context)
        {
            this._context = context;
            this._mapper = mapper;
        }
        public async Task<ServiceResponse<List<GetCharacterDTO>>> AddCharacter(AddCharacterDTO newCharacter)
        {
            var serviceResponse = new ServiceResponse<List<GetCharacterDTO>>();

            // Map the DTO to the database model
            var character = _mapper.Map<Character>(newCharacter);

            // Add the character to the DbContext
            await _context.Characters.AddAsync(character);

            // Save changes to the database
            await _context.SaveChangesAsync();

            // Update the ID from the database
            character.Id = character.Id;  // The Id should be set by the database

            // Fetch updated list of characters from the database
            var dbCharacters = await _context.Characters.ToListAsync();

            // Map the database models to DTOs
            serviceResponse.Data = dbCharacters.Select(c => _mapper.Map<GetCharacterDTO>(c)).ToList();

            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetCharacterDTO>>> GetAllCharacters()
        {
            var dbCharacters = await _context.Characters.ToListAsync();
            var serviceResponse = new ServiceResponse<List<GetCharacterDTO>>
            {
                Data = dbCharacters.Select(c => _mapper.Map<GetCharacterDTO>(c)).ToList()
            };
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetCharacterDTO>> GetCharacterById(int id)
        {
            var dbCharacter = await _context.Characters.FirstOrDefaultAsync(c => c.Id == id);
            var serviceResponse = new ServiceResponse<GetCharacterDTO>
            {
                Data = _mapper.Map<GetCharacterDTO>(dbCharacter)
            };

            return serviceResponse;
        }

        public async Task<ServiceResponse<GetCharacterDTO>> UpdateCharacter(UpdateCharacterDTO updatedCharacter)
        {
            var serviceResponse = new ServiceResponse<GetCharacterDTO>();
            try
            {
                // Fetch the character from the database
                var dbCharacter = await _context.Characters.FirstOrDefaultAsync(c => c.Id == updatedCharacter.Id);

                // If the character doesn't exist, throw an exception
                if (dbCharacter is null)
                {
                    throw new Exception($"Character with Id '{updatedCharacter.Id}' not found.");
                }

                // Map the updated fields onto the database entity
                _mapper.Map(updatedCharacter, dbCharacter);

                // Update the character in the database
                _context.Characters.Update(dbCharacter);

                // Save the changes to the database
                await _context.SaveChangesAsync();

                // Map the updated database entity to the DTO
                serviceResponse.Data = _mapper.Map<GetCharacterDTO>(dbCharacter);
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }

            return serviceResponse;
        }


        public async Task<ServiceResponse<List<GetCharacterDTO>>> DeleteCharacter(int id)
        {
            var serviceResponse = new ServiceResponse<List<GetCharacterDTO>>();

            try
            {
                // Fetch the character from the database
                var dbCharacter = await _context.Characters.FirstOrDefaultAsync(c => c.Id == id);

                // If the character doesn't exist, throw an exception
                if (dbCharacter is null)
                {
                    throw new Exception($"Character with id '{id}' was not found");
                }

                // Remove the character from the DbContext
                _context.Characters.Remove(dbCharacter);

                // Save changes to the database
                await _context.SaveChangesAsync();

                // Fetch updated list of characters from the database
                var dbCharacters = await _context.Characters.ToListAsync();

                // Map the database models to DTOs
                serviceResponse.Data = dbCharacters.Select(c => _mapper.Map<GetCharacterDTO>(c)).ToList();
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