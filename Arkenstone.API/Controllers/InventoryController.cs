using Arkenstone.API.Models;
using Arkenstone.API.Services;
using Arkenstone.Logic.Repository;
using Arkenstone.Entities;
using Arkenstone.Entities.DbSet;
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
    public class InventoryController : OriginController
    {
        public InventoryController(ArkenstoneContext context) : base(context)
        {

        }

        /// <summary>
        /// provides the overall assets of the corporation
        /// </summary>
        /// <response code="200">list of assets</response>
        [HttpGet]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<InventoryModel>))]
        public IActionResult GetSimple()
        {
            var tokenCharacter = TokenService.GetCharacterFromToken(_context, HttpContext);

            InventoryService assetService = new InventoryService(_context);

            Dictionary<Item, long> returnvalue = new Dictionary<Item, long>();
            foreach (var asset in assetService.GetList(tokenCharacter.CorporationId))
            {
                if (returnvalue.ContainsKey(asset.Item))
                    returnvalue[asset.Item] += asset.Quantity;
                else
                    returnvalue.Add(asset.Item, asset.Quantity);
            }
            if (returnvalue.Count == 0)
                return NoContent();
            return Ok(returnvalue.Select(x => new InventoryModel(x.Key, x.Value)).ToList());
        }

        /// <summary>
        /// provides all the assets of the Corporation by location
        /// </summary>
        /// <param name="LocationId" example="1041276076345">Location Id</param>
        /// <response code="200">list of assets</response>
        [HttpGet("Location")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<InventoryStationModel>))]
        public IActionResult GetLocation([FromQuery] long? LocationId)
        {
            var tokenCharacter = TokenService.GetCharacterFromToken(_context, HttpContext);

            LocationService LocationService = new LocationService(_context);
            InventoryService assetService = new InventoryService(_context);

            List<InventoryStationModel> returnvalue = new List<InventoryStationModel>();
            
            
            if (LocationId.HasValue)
            {
                var location = LocationService.Get(LocationId.Value).ThrowNotAuthorized(tokenCharacter.CorporationId);
                var inventorys = assetService.GetListFromLocation(location.Id);
                returnvalue.Add(new InventoryStationModel(location, inventorys));
            }
            else
            {
                foreach (var location in LocationService.GetList(tokenCharacter.CorporationId))
                {
                    try
                    {
                        var inventorys = assetService.GetListFromLocation(location.Id);
                        returnvalue.Add(new InventoryStationModel(location, inventorys));
                    }
                    catch (Exception)
                    {

                    }
                }
            }

            if (returnvalue.Count() == 0)
                return NoContent();
            return Ok(returnvalue);
        }

        /// <summary>
        /// provides all the assets of the Corporation by Sublocation
        /// </summary>
        /// <param name="SublocationId" example="5">SubLocation Id</param>
        /// <response code="200">list of assets</response>
        [HttpGet("SubLocation")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<InventorySubLocationModel>))]
        public IActionResult GetSubLocation([FromQuery] long? SublocationId)
        {
            var tokenCharacter = TokenService.GetCharacterFromToken(_context, HttpContext);

            SubLocationService subLocationServiceService = new SubLocationService(_context);
            InventoryService assetService = new InventoryService(_context);

            List<InventorySubLocationModel> returnvalue = new List<InventorySubLocationModel>();


            if (SublocationId.HasValue)
            {
                var sublocation = subLocationServiceService.Get(SublocationId.Value).ThrowNotAuthorized(tokenCharacter.CorporationId);
                var inventorys = assetService.GetListFromSubLocation(sublocation.Id);
                returnvalue.Add(new InventorySubLocationModel(sublocation, inventorys));
            }
            else
            {
                foreach (var sublocation in subLocationServiceService.GetList(tokenCharacter.CorporationId))
                {
                    try
                    {
                        var inventorys = assetService.GetListFromSubLocation(sublocation.Id);
                        returnvalue.Add(new InventorySubLocationModel(sublocation, inventorys));
                    }
                    catch (Exception)
                    {

                    }
                }
            }
            if (returnvalue.Count() == 0)
                return NoContent();
            return Ok(returnvalue);

        }


        /// <summary>
        /// Refresh and provides the overall assets of the corporation
        /// </summary>
        /// <param name="id" example="5">SubLocation Id</param>
        /// <response code="200">list of assets</response>
        [HttpPost("Refresh")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<InventoryModel>))]
        public async Task<IActionResult> RefreshAssetAsync()
        {
            var tokenCharacter = TokenService.GetCharacterFromToken(_context, HttpContext);
            InventoryService assetService = new InventoryService(_context);
            await assetService.RefreshAsset(tokenCharacter.CorporationId);

            Dictionary<Item, long> returnvalue = new Dictionary<Item, long>();
            foreach (var asset in assetService.GetList(tokenCharacter.CorporationId))
            {
                if (returnvalue.ContainsKey(asset.Item))
                    returnvalue[asset.Item] += asset.Quantity;
                else
                    returnvalue.Add(asset.Item, asset.Quantity);
            }

            if (returnvalue.Count() == 0)
                return NoContent();
            return Ok(returnvalue.Select(x => new InventoryModel(x.Key, x.Value)).ToList());
        }




    }
}
