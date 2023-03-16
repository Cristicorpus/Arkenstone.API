using Arkenstone.API.Models;
using Arkenstone.API.Services;
using Arkenstone.Controllers;
using Arkenstone.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Arkenstone.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationController : OriginController
    {
        public LocationController(ArkenstoneContext context) : base(context)
        {

        }


        [HttpGet]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<LocationModel>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetSimple([FromQuery] long? LocationId)
        {
            var tokenCharacter = TokenService.GetCharacterFromToken(_context, HttpContext);
            if (tokenCharacter == null)
                return Unauthorized("You are not authorized");

            if (LocationId.HasValue)
            {
                var structure = _context.Locations.Find(LocationId);
                if (structure == null)
                    return NotFound();
            }

            LocationService structureService = new LocationService(_context);
            return Ok(structureService.GetBasicModel(LocationId));

        }

        [HttpGet("Detailed")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<LocationModelDetails>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetDetailed([FromQuery] long? LocationId)
        {
            var tokenCharacter = TokenService.GetCharacterFromToken(_context, HttpContext);
            if (tokenCharacter == null)
                return Unauthorized("You are not authorized");

            if (LocationId.HasValue)
            {
                var structure = _context.Locations.Find(LocationId);
                if (structure == null)
                    return NotFound();
            }

            LocationService structureService = new LocationService(_context);
            return Ok(structureService.GetDetailledModel(LocationId));

        }


        [HttpPost]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<LocationModelDetails>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult SetFit([FromQuery] long LocationId, [FromBody] string fit)
        {

            var tokenCharacter = TokenService.GetCharacterFromToken(_context, HttpContext);
            if (tokenCharacter == null)
                return Unauthorized("You are not authorized");

            var structure = _context.Locations.Find(LocationId);
            if (structure == null)
                return NotFound();

            if (structure.Id < 1000000000)
                return BadRequest(LocationId.ToString() + " isn t an struture, is an station.");

            LocationService structureService = new LocationService(_context);
            structureService.SetFitToStructure(LocationId, fit);
            return Ok(structureService.GetDetailledModel(LocationId));

        }

    }
}
