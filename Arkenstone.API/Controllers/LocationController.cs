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


        /// <summary>
        /// get location data
        /// </summary>
        /// <param name="LocationId" example="1041276076345">Location Id</param>
        /// <response code="200">structure data</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">location  not found</response>
        [HttpGet]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<LocationModel>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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

        /// <summary>
        /// get location data detailled(rigs)
        /// </summary>
        /// <param name="LocationId" example="1041276076345">Location Id</param>
        /// <response code="200">structure data detailled</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">location  not found</response>
        [HttpGet("Detailed")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<LocationModelDetails>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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


        /// <summary>
        /// set rigs of an location for Material efficiency calculation
        /// </summary>
        /// <param name="LocationId" example="1041276076345">Location Id</param>
        /// <param name="fit" example="[Azbel, *Simulated Azbel Fitting]\r\n\r\n\r\n\r\nStandup L-Set Equipment Manufacturing Efficiency II\r\nStandup L-Set Basic Large Ship Manufacturing Efficiency I\r\n\r\n\r\n\r\n\r\n\r\n">raw fit of eve, copy paste work </param>
        /// <response code="200">structure data detailled</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">location  not found</response>
        /// <response code="400">its not an structure</response>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<LocationModelDetails>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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
