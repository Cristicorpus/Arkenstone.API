using Arkenstone.API.Models;
using Arkenstone.API.Services;
using Arkenstone.Controllers;
using Arkenstone.Entities;
using Arkenstone.Logic.BusinessException;
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
        [HttpGet]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<LocationModel>))]
        public IActionResult GetSimple([FromQuery] long? LocationId)
        {
            var tokenCharacter = TokenService.GetCharacterFromToken(_context, HttpContext);

            LocationService locationService = new LocationService(_context);

            if (LocationId.HasValue)
                return Ok(new List<LocationModel> { new LocationModel(locationService.Get(LocationId.Value).ThrowNotAuthorized(tokenCharacter.CorporationId)) });
            else
                return Ok(locationService.GetList(tokenCharacter.CorporationId).Select(x => new LocationModel(x)).ToList());

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
        public IActionResult GetDetailed([FromQuery] long? LocationId)
        {
            var tokenCharacter = TokenService.GetCharacterFromToken(_context, HttpContext);

            LocationService locationService = new LocationService(_context);

            if (LocationId.HasValue)
                return Ok(new List<LocationModelDetails> { new LocationModelDetails(locationService.Get(LocationId.Value).ThrowNotAuthorized(tokenCharacter.CorporationId)) });
            else
                return Ok(locationService.GetList(tokenCharacter.CorporationId).Select(x => new LocationModelDetails(x)).ToList());
        }


        /// <summary>
        /// set rigs of an location for Material efficiency calculation
        /// </summary>
        /// <param name="LocationId" example="1041276076345">Location Id</param>
        /// <param name="fit" example="[Azbel, *Simulated Azbel Fitting]\r\n\r\n\r\n\r\nStandup L-Set Equipment Manufacturing Efficiency II\r\nStandup L-Set Basic Large Ship Manufacturing Efficiency I\r\n\r\n\r\n\r\n\r\n\r\n">raw fit of eve, copy paste work </param>
        /// <response code="200">structure data detailled</response>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<LocationModelDetails>))]
        public IActionResult SetFit([FromQuery] long LocationId, [FromBody] string fit)
        {

            var tokenCharacter = TokenService.GetCharacterFromToken(_context, HttpContext);
            
            LocationService locationService = new LocationService(_context);
            var location = locationService.Get(LocationId).ThrowNotAuthorized(tokenCharacter.CorporationId);

            locationService.SetFitToStructure(location, fit);
            
            return Ok(new List<LocationModelDetails> { new LocationModelDetails(locationService.Get(LocationId).ThrowNotAuthorized(tokenCharacter.CorporationId)) });

        }

    }
}
