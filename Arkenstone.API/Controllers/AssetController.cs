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

        /// <summary>
        /// provides the overall assets of the corporation
        /// </summary>
        /// <response code="200">list of assets</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<AssetModel>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult GetSimple()
        {
            var tokenCharacter = TokenService.GetCharacterFromToken(_context, HttpContext);
            if (tokenCharacter == null)
                return Unauthorized("You are not authorized");

            AssetService assetService = new AssetService(_context);
            return Ok(assetService.GetGlobalAsset(tokenCharacter.CorporationId));
        }

        /// <summary>
        /// provides all the assets of the Corporation by location
        /// </summary>
        /// <param name="LocationId" example="1041276076345">Location Id</param>
        /// <response code="200">list of assets</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">location not found</response>
        [HttpGet("Location")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<AssetStationModel>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
                    return Forbid("You are not authorized to see this structure asset. you dont have any office in.");
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

        /// <summary>
        /// provides all the assets of the Corporation by Sublocation
        /// </summary>
        /// <param name="SublocationId" example="5">SubLocation Id</param>
        /// <response code="200">list of assets</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">sublocation not found</response>
        [HttpGet("SubLocation")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<AssetSubLocationModel>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetSubLocation([FromQuery] int? SublocationId)
        {
            var tokenCharacter = TokenService.GetCharacterFromToken(_context, HttpContext);
            if (tokenCharacter == null)
                return Unauthorized("You are not authorized");

            if (SublocationId.HasValue)
            {
                var structure = _context.SubLocations.FirstOrDefault(x=>x.Id == SublocationId.Value);
                if (structure == null)
                    return NotFound();
                if (structure.CorporationId != tokenCharacter.CorporationId)
                    return Forbid("You are not authorized to see this sublocation asset.");
            }

            AssetService assetService = new AssetService(_context);
            SubLocationService subLocationServiceService = new SubLocationService(_context);

            List<AssetSubLocationModel> returnvalue = new List<AssetSubLocationModel>();


            if (SublocationId.HasValue)
                returnvalue.Add(assetService.GetSubLocationAsset(SublocationId.Value));
            else
            {
                foreach (var location in subLocationServiceService.ListSubLocationCorp(tokenCharacter.CorporationId))
                {
                    returnvalue.Add(assetService.GetSubLocationAsset(location.Id));
                }
            }

            return Ok(returnvalue);

        }


        /// <summary>
        /// Refresh and provides the overall assets of the corporation
        /// </summary>
        /// <param name="id" example="5">SubLocation Id</param>
        /// <response code="200">list of assets</response>
        /// <response code="401">Unauthorized</response>
        [HttpPost("Refresh")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<AssetModel>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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
