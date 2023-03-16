using Arkenstone.API.Models;
using Arkenstone.API.Services;
using Arkenstone.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace Arkenstone.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssetController : OriginController
    {
        public AssetController(ArkenstoneContext context) : base(context)
        {

        }

        // GET api/structure
        [HttpGet]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<AssetModel>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetSimple()
        {
            var tokenCharacter = TokenService.GetCharacterFromToken(_context, HttpContext);
            if (tokenCharacter == null)
                return Unauthorized("You are not authorized");

            AssetService assetService = new AssetService(_context);
            return Ok(assetService.GetGlobalAsset(tokenCharacter.CorporationId));

        }

        [HttpGet("Structure")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<AssetStationModel>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetStructure([FromQuery] long? LocationId)
        {
            var tokenCharacter = TokenService.GetCharacterFromToken(_context, HttpContext);
            if (tokenCharacter == null)
                return Unauthorized("You are not authorized");

            if (LocationId.HasValue)
            {
                var structure = _context.Locations.Find(LocationId);
                if (structure == null)
                    throw new Exception("La structure " + LocationId.ToString() + " n'existe pas");
            }

            AssetService assetService = new AssetService(_context);
            StructureService structureService = new StructureService(_context);

            List<AssetStationModel> returnvalue = new List<AssetStationModel>();


            if (LocationId.HasValue)
                returnvalue.Add(assetService.GetStationAsset(tokenCharacter.CorporationId, LocationId.Value));
            else
            {
                foreach (var location in structureService.ListStructureCorp(tokenCharacter.CorporationId))
                {
                    returnvalue.Add(assetService.GetStationAsset(tokenCharacter.CorporationId, location.Id));
                }
            }

            return Ok(returnvalue);

        }



    }
}
