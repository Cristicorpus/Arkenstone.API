using Arkenstone.API.Models;
using Arkenstone.API.Services;
using Arkenstone.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using Microsoft.AspNetCore.Authorization;
using Arkenstone.Entities.DbSet;
using System.Linq;

namespace Arkenstone.API.Controllers
{
    public class SubLocationController : OriginController
    {

        public SubLocationController(ArkenstoneContext context) : base(context)
        {

        }

        [HttpGet]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<SubLocation>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Get([FromQuery] int? SubLocationId)
        {
            var tokenCharacter = TokenService.GetCharacterFromToken(_context, HttpContext);
            if (tokenCharacter == null)
                return Unauthorized("You are not authorized");

            SubLocationService subLocationServiceService = new SubLocationService(_context);
            
            if (SubLocationId.HasValue)
            {
                var subLocation = _context.SubLocations.FirstOrDefault(x=>x.Id == SubLocationId);
                if (subLocation == null)
                    return NotFound();
                if (subLocation.CorporationId != tokenCharacter.CorporationId)
                    return Unauthorized("You are not authorized to see this sublocation.");
                return Ok(subLocationServiceService.GetSubLocationBySubLocationId(SubLocationId.Value));
            }
            return Ok(subLocationServiceService.GetSubLocationByCorp(tokenCharacter.CorporationId));
        }

        [HttpGet("Location")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<SubLocation>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetByLocation([FromQuery] long LocationId)
        {
            var tokenCharacter = TokenService.GetCharacterFromToken(_context, HttpContext);
            if (tokenCharacter == null)
                return Unauthorized("You are not authorized");



            var subLocation = _context.Locations.FirstOrDefault(x => x.Id == LocationId);
            if (subLocation == null)
                return NotFound();

            SubLocationService subLocationServiceService = new SubLocationService(_context);
            var temp = subLocationServiceService.GetSubLocationByLocation(tokenCharacter.CorporationId, LocationId);
            if (temp.Count() <= 0)
                return NoContent();
            else
                return Ok(temp);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Edit([FromQuery] int SubLocationId, [FromQuery] bool Toanalyse)
        {
            var tokenCharacter = TokenService.GetCharacterFromToken(_context, HttpContext);
            if (tokenCharacter == null)
                return Unauthorized("You are not authorized");

            SubLocationService subLocationServiceService = new SubLocationService(_context);

            var subLocation = _context.SubLocations.FirstOrDefault(x=>x.Id == SubLocationId);
            if (subLocation == null)
                return NotFound();
            if (subLocation.CorporationId != tokenCharacter.CorporationId)
                return Unauthorized("You are not authorized to see this sublocation.");
            
            subLocationServiceService.EditSubLocation(SubLocationId, Toanalyse);
            return Ok();
        }

    }
}
