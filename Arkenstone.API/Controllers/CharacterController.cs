using Arkenstone.API.Services;
using Arkenstone.Logic.BusinessException;
using Arkenstone.Entities;
using EveMiningFleet.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace Arkenstone.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CharacterController : OriginController
    {
        public CharacterController(ArkenstoneContext context) : base(context)
        {

        }

        /// <summary>
        /// Retrieves characters, 
        /// </summary>
        /// <remarks>only ONE PARAMETER!</remarks>
        /// <param name="id" example="96852613">get the character by id,</param>
        /// <param name="name" example="feonor dalb">get the character by name.Withoutspace</param>
        /// <response code="200">character retrieved</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CharacterModel))]
        public IActionResult Get([FromQuery]int? id = null, [FromQuery] string name = null)
        {
            //get all
            if (!id.HasValue && name == null)
                throw new ParameterException("id,name");
            if (id.HasValue && name != null)
                throw new ParameterException("id,name");

            CharacterService characterService = new CharacterService(_context);

            if (id.HasValue && name == null)
                return Ok(new CharacterModel(characterService.Get(id.Value)));
            else
                return Ok(new CharacterModel(characterService.GetByName(name)));
            
        }

        /// <summary>
        /// Get a list of characters associated with the current user
        /// </summary>
        /// <returns>A list of characters associated with the current user</returns>
        /// <response code="200">characters retrieved</response>
        [HttpGet("my_characters")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MainCharacterModel))]

        public IActionResult GetMyCharacters()
        {
            var tokenCharacter = TokenService.GetCharacterFromToken(_context, HttpContext);
            CharacterService characterService = new CharacterService(_context);

            var returnValue = new MainCharacterModel(characterService.Get(tokenCharacter.CharacterMainId));
            returnValue.AltCharacter = characterService.GetListFromMain(tokenCharacter.CharacterMainId).Select(x => new CharacterModel(x)).ToList();
            return Ok(returnValue);
        }

        /// <summary>
        /// change the main Id of all the characters linked to the token character
        /// </summary>
        /// <param name="id" example="5">new main Id</param>
        /// <response code="200">new token</response>
        [Route("SetMain")]
        [HttpPatch]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        public IActionResult SetMain([FromQuery] int mainId)
        {
            var tokenCharacter = TokenService.GetCharacterFromToken(_context, HttpContext);

            CharacterService characterService = new CharacterService(_context);
            if (!characterService.GetListFromMain(tokenCharacter.CharacterMainId).Any(x => x.Id == mainId))
                throw new NotAuthorized();

            characterService.CharacterSetMain(tokenCharacter.CharacterMainId, mainId);
            return Ok(TokenService.Createtoken(characterService.Get(mainId)));
        }

    }
}
