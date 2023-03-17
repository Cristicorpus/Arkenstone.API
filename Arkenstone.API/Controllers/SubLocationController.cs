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
    [Route("api/[controller]")]
    [ApiController]
    public class SubLocationController : OriginController
    {

        public SubLocationController(ArkenstoneContext context) : base(context)
        {

        }

        /// <summary>
        /// get data of sublocation
        /// </summary>
        /// <param name="SubLocationId" example="5">SubLocation Id</param>
        /// <response code="200">list of sublocation</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">sublocation not found</response>
        [HttpGet]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<SubLocation>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Get([FromQuery] int? SubLocationId)
        {
            var tokenCharacter = TokenService.GetCharacterFromToken(_context, HttpContext);
            if (tokenCharacter == null)
                return Unauthorized("You are not authorized");

            SubLocationService subLocationServiceService = new SubLocationService(_context);
            
            if (SubLocationId.HasValue)
            {
                var subLocation = subLocationServiceService.GetFirstOrDefault(SubLocationId.Value);
                if (subLocation == null)
                    return NotFound();
                if (subLocation.CorporationId != tokenCharacter.CorporationId)
                    return Forbid("You are not authorized to see this sublocation.");
                return Ok(subLocationServiceService.GetSubLocationBySubLocationId(SubLocationId.Value));
            }
            return Ok(subLocationServiceService.GetSubLocationByCorp(tokenCharacter.CorporationId));
        }


        /// <summary>
        /// get data of sublocation in an location
        /// </summary>
        /// <param name="LocationId" example="1041276076345">Location Id</param>
        /// <response code="200">list of sublocation</response>
        /// <response code="204">no content</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">location not found</response>
        [HttpGet("Location")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<SubLocation>))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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

        /// <summary>
        /// set or reset sublocation to analyse
        /// </summary>
        /// <param name="SubLocationId" example="5">SubLocation Id</param>
        /// <response code="200"></response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">sublocation not found</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Edit([FromQuery] int SubLocationId, [FromQuery] bool Toanalyse)
        {
            var tokenCharacter = TokenService.GetCharacterFromToken(_context, HttpContext);
            if (tokenCharacter == null)
                return Unauthorized("You are not authorized");

            SubLocationService subLocationServiceService = new SubLocationService(_context);

            var subLocation = subLocationServiceService.GetFirstOrDefault(SubLocationId);
            if (subLocation == null)
                return NotFound();
            if (subLocation.CorporationId != tokenCharacter.CorporationId)
                return Forbid("You are not authorized to see this sublocation.");
            
            subLocationServiceService.EditSubLocation(SubLocationId, Toanalyse);
            return Ok();
        }

    }
}
