using Arkenstone.API.Models;
using Arkenstone.API.Services;
using Arkenstone.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Arkenstone.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AssetController : OriginController
    {
        public AssetController(ArkenstoneContext context) : base(context)
        {

        }

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

        [HttpGet("Location")]
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
                    return NotFound("structure " + LocationId.ToString() + " not recognized");
                if (!structure.SubLocations.Any(x=>x.CorporationId == tokenCharacter.CorporationId))
                    return Unauthorized("You are not authorized to see this structure asset. you dont have any office in.");
            }

            AssetService assetService = new AssetService(_context);
            LocationService structureService = new LocationService(_context);

            List<AssetStationModel> returnvalue = new List<AssetStationModel>();


            if (LocationId.HasValue)
                returnvalue.Add(assetService.GetLocationAsset( LocationId.Value));
            else
            {
                foreach (var location in structureService.ListLocationCorp(tokenCharacter.CorporationId))
                {
                    returnvalue.Add(assetService.GetLocationAsset( location.Id));
                }
            }

            return Ok(returnvalue);

        }

        [HttpGet("SubLocation")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<AssetSubLocationModel>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetSubLocation([FromQuery] int? LocationId)
        {
            var tokenCharacter = TokenService.GetCharacterFromToken(_context, HttpContext);
            if (tokenCharacter == null)
                return Unauthorized("You are not authorized");

            if (LocationId.HasValue)
            {
                var structure = _context.SubLocations.FirstOrDefault(x=>x.Id ==LocationId.Value);
                if (structure == null)
                    return NotFound();
                if (structure.CorporationId != tokenCharacter.CorporationId)
                    return Unauthorized("You are not authorized to see this sublocation asset.");
            }

            AssetService assetService = new AssetService(_context);
            SubLocationService subLocationServiceService = new SubLocationService(_context);

            List<AssetSubLocationModel> returnvalue = new List<AssetSubLocationModel>();


            if (LocationId.HasValue)
                returnvalue.Add(assetService.GetSubLocationAsset( LocationId.Value));
            else
            {
                foreach (var location in subLocationServiceService.ListSubLocationCorp(tokenCharacter.CorporationId))
                {
                    returnvalue.Add(assetService.GetSubLocationAsset(location.Id));
                }
            }

            return Ok(returnvalue);

        }


        [HttpPost("Refresh")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<AssetModel>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RefreshAssetAsync()
        {
            var tokenCharacter = TokenService.GetCharacterFromToken(_context, HttpContext);
            if (tokenCharacter == null)
                return Unauthorized("You are not authorized");


            AssetService assetService = new AssetService(_context);
            await assetService.RefreshAsset(tokenCharacter.CorporationId);
            return Ok(assetService.GetGlobalAsset(tokenCharacter.CorporationId));

        }




    }
}
