﻿using Arkenstone.API.Models;
using Arkenstone.API.Services;
using Arkenstone.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Arkenstone.Logic.Repository;
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
        public IActionResult Get([FromQuery] long? LocationId)
        {
            var tokenCharacter = TokenService.GetCharacterFromToken(_context, HttpContext);

            LocationService locationService = new LocationService(_context);
            var returnvalue = new List<LocationModel>();
            if (LocationId.HasValue)
                returnvalue.Add(new LocationModel(locationService.Get(LocationId.Value).ThrowNotAuthorized(tokenCharacter.CorporationId)));
            else
                returnvalue.AddRange(locationService.GetList(tokenCharacter.CorporationId).Select(x => new LocationModel(x)).ToList());

            if (returnvalue.Count() == 0)
                return NoContent();
            return Ok(returnvalue);
        }



        /// <summary>
        /// set rigs of an location for Material efficiency calculation
        /// </summary>
        /// <param name="LocationId" example="1041276076345">Location Id</param>
        /// <param name="fit" example="[Azbel, *Simulated Azbel Fitting]\r\n\r\n\r\n\r\nStandup L-Set Equipment Manufacturing Efficiency II\r\nStandup L-Set Basic Large Ship Manufacturing Efficiency I\r\n\r\n\r\n\r\n\r\n\r\n">raw fit of eve, copy paste work </param>
        /// <response code="200">structure data detailled</response>
        [HttpPut]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LocationModel))]
        public IActionResult SetFit([FromQuery] long LocationId, [FromBody] string fit)
        {

            var tokenCharacter = TokenService.GetCharacterFromToken(_context, HttpContext);
            
            LocationService locationService = new LocationService(_context);
            
            locationService.Get(LocationId).ThrowNotAuthorized(tokenCharacter.CorporationId);
            
            locationService.SetFitToStructure(LocationId, fit);
            
            return Ok(new LocationModel(locationService.Get(LocationId)));

        }

    }
}
