using Arkenstone.API.Models;
using Arkenstone.API.Services;
using Arkenstone.Controllers;
using Arkenstone.Entities;
using Arkenstone.Logic.Efficiency;
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

        // GET api/Efficiency
        [HttpGet]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EfficiencyModel))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetEfficiencyFromStation([FromQuery] long LocationId, [FromQuery] int ItemId)
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

            EfficiencyService efficiencyService = new EfficiencyService(_context);

            return Ok(efficiencyService.GetEfficiencyFromStation(structure, item));

        }


        // GET api/Efficiency
        [HttpGet("chooseStation")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EfficiencyModel))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
                var temp = efficiencyService.GetEfficiencyFromStation(location, item);
                if (returnModel ==null || returnModel.MEefficiency > temp.MEefficiency)
                    returnModel = temp;
            }
            return Ok(returnModel);

        }
    }
}
