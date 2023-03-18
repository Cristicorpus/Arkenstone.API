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
        [HttpGet]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<SubLocation>))]
        public IActionResult Get([FromQuery] int? SubLocationId)
        {
            var tokenCharacter = TokenService.GetCharacterFromToken(_context, HttpContext);

            SubLocationService subLocationServiceService = new SubLocationService(_context);
            var returnvalue = new List<SubLocation>();
            
            if (SubLocationId.HasValue)
                returnvalue.Add(subLocationServiceService.Get(SubLocationId.Value).ThrowNotAuthorized(tokenCharacter.CorporationId));
            else
                returnvalue.AddRange(subLocationServiceService.GetList(tokenCharacter.CorporationId));

            if (returnvalue.Count() == 0)
                return NoContent();

            return Ok(returnvalue);
        }


        /// <summary>
        /// get data of sublocation in an location
        /// </summary>
        /// <param name="LocationId" example="1041276076345">Location Id</param>
        /// <response code="200">list of sublocation</response>
        [HttpGet("Location")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<SubLocation>))]
        public IActionResult GetByLocation([FromQuery] long LocationId)
        {
            var tokenCharacter = TokenService.GetCharacterFromToken(_context, HttpContext);
            
            LocationService locationServiceService = new LocationService(_context);
            SubLocationService subLocationServiceService = new SubLocationService(_context);
            
            var location = locationServiceService.Get(LocationId);
            var returnvalue = new List<SubLocation>();
            returnvalue.AddRange(subLocationServiceService.GetListFromLocation(tokenCharacter.CorporationId, location.Id));
            if (returnvalue.Count() == 0)
                return NoContent();

            return Ok(returnvalue);
        }

        /// <summary>
        /// set or reset sublocation to analyse
        /// </summary>
        /// <param name="SubLocationId" example="5">SubLocation Id</param>
        /// <response code="200"></response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SubLocation))]
        public IActionResult Edit([FromQuery] long SubLocationId, [FromQuery] bool Toanalyse)
        {
            var tokenCharacter = TokenService.GetCharacterFromToken(_context, HttpContext);

            SubLocationService subLocationServiceService = new SubLocationService(_context);

            var subLocation = subLocationServiceService.Get(SubLocationId).ThrowNotAuthorized(tokenCharacter.CorporationId);
            subLocationServiceService.EditSubLocation(subLocation.Id, Toanalyse);
            return Ok(subLocationServiceService.Get(SubLocationId).ThrowNotAuthorized(tokenCharacter.CorporationId));
        }

    }
}
