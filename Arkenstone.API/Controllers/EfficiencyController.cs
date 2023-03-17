using Arkenstone.API.Models;
using Arkenstone.API.Services;
using Arkenstone.Controllers;
using Arkenstone.Entities;
using Arkenstone.Logic.Efficiency;
using ESI.NET.Enumerations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace Arkenstone.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EfficiencyController : OriginController
    {
        public EfficiencyController(ArkenstoneContext context) : base(context)
        {

        }

        /// <summary>
        /// gives the material efficiency rate of the structure on a item Id
        /// </summary>
        /// <param name="LocationId" example="1041276076345">Location Id</param>
        /// <param name="ItemId" example="24692">item IDd</param>
        /// <response code="200">efficiency</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">strcture or item not found</response>
        [HttpGet]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EfficiencyModel))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetEfficiencyFromLocation([FromQuery] long LocationId, [FromQuery] int ItemId)
        {
            var tokenCharacter = TokenService.GetCharacterFromToken(_context, HttpContext);
            if (tokenCharacter == null)
                return Unauthorized("You are not authorized");

            var structure = _context.Locations.Include("StructureType").FirstOrDefault(x=>x.Id == LocationId);
            if (structure == null)
                return NotFound("Structure not found");
            var item = _context.Items.Find(ItemId);
            if (item == null)
                return NotFound("Item not found");

            LocationService locationService = new LocationService(_context);


            if (!locationService.ListLocationCorp(tokenCharacter.CorporationId).Any(x=>x.Id== structure.Id))
                return Forbid("You are not authorized to see this sublocation.");

            EfficiencyService efficiencyService = new EfficiencyService(_context);

            return Ok(efficiencyService.GetEfficiencyFromLocation(structure, item));

        }



        /// <summary>
        /// gives the material efficiency rate and the best location  on a item Id
        /// </summary>
        /// <param name="ItemId" example="24692">item IDd</param>
        /// <response code="200">efficiency</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404"> item not found</response>
        [HttpGet("chooseStation")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EfficiencyModel))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetBestEfficiency([FromQuery] int ItemId)
        {
            var tokenCharacter = TokenService.GetCharacterFromToken(_context, HttpContext);
            if (tokenCharacter == null)
                return Unauthorized("You are not authorized");

            var item = _context.Items.Find(ItemId);
            if (item == null)
                return NotFound("Item not found");


            EfficiencyService efficiencyService = new EfficiencyService(_context);

            EfficiencyModel returnModel = null;
                
            foreach (var location in _context.Locations.Include("StructureType").Include("SubLocations").Where(x=>x.SubLocations.Any(y=>y.CorporationId== tokenCharacter.CorporationId)))
            {
                var temp = efficiencyService.GetEfficiencyFromLocation(location, item);
                if (returnModel ==null || returnModel.MEefficiency > temp.MEefficiency)
                    returnModel = temp;
            }
            return Ok(returnModel);

        }
    }
}
