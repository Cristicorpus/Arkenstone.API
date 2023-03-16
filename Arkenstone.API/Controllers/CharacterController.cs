using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using EveMiningFleet.API.Models;
using Arkenstone.Entities.DbSet;
using Arkenstone.Entities;
using Arkenstone.API.Services;

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
        /// <param name="mainid" example="96852613">get all character by main id</param>
        /// <param name="name" example="feonordalb">get the character by name.Withoutspace</param>
        /// <response code="200">character retrieved</response>
        /// <response code="204">No character in database</response>
        /// <response code="400">only ONE PARAMETER!</response>
        /// <response code="404">character not found</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<CharacterModel>))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Get([FromQuery]int? id = null, [FromQuery] string name = null)
        {
            //get all
            if (!id.HasValue && name == null)
                return BadRequest("At least one parameter");
            if (id.HasValue && name != null)
                return BadRequest("At least one parameter");

            CharacterService characterService = new CharacterService(_context);
            //get from id
            if (id.HasValue && name == null)
            {
                var _Character = characterService.Get(id.Value);
                if (_Character != null)
                    return Ok(_Character);
                else
                    return NotFound();
            }
            else
            {
                var allCharacter = characterService.GetByName(name);
                if (allCharacter.Count() > 0)
                    return Ok(allCharacter);
                else
                    return NotFound();
            }
            
            return BadRequest("Just one parameter at same time");

        }
        
        /// <summary>
        /// Get a list of characters associated with the current user
        /// </summary>
        /// <returns>A list of characters associated with the current user</returns>
        /// <response code="200">characters retrieved</response>
        /// <response code="401">authorized</response>
        /// <response code="404">character not found</response>
        [HttpGet("my_characters")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<CharacterModel>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult GetMyCharacters()
        {
            var tokenCharacter = TokenService.GetCharacterFromToken(_context, HttpContext);
            if (tokenCharacter == null)
                return Unauthorized("You are not authorized");

            CharacterService characterService = new CharacterService(_context);
            return Ok(characterService.GetByMainId(tokenCharacter.CharacterMainId));
        }

        [Route("SetMain")]
        [HttpPatch]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult SetMain([FromQuery] int mainId)
        {
            var tokenCharacter = TokenService.GetCharacterFromToken(_context, HttpContext);
            if (tokenCharacter == null)
                return Unauthorized("You are not authorized");

            CharacterService characterService = new CharacterService(_context);
            if(!characterService.GetByMainId(tokenCharacter.CharacterMainId).Any(x => x.Id == mainId))
                return BadRequest("You are not authorized to set this character as main");

            characterService.UpdateMainId(tokenCharacter.CharacterMainId, mainId);


            var characterDb = _context.Characters.Find(mainId);

            return Ok(TokenService.Createtoken(characterDb));
        }

    }
}
