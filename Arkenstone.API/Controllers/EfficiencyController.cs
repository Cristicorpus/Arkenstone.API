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
        [HttpGet]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EfficiencyModel))]
        public IActionResult GetEfficiencyFromLocation([FromQuery] long LocationId, [FromQuery] int ItemId)
        {
            var tokenCharacter = TokenService.GetCharacterFromToken(_context, HttpContext);


            LocationService locationService = new LocationService(_context);
            var location = locationService.Get(LocationId).ThrowNotAuthorized(tokenCharacter.CorporationId);


            ItemService itemService = new ItemService(_context);
            var item = itemService.GetFromRecipe(ItemId);

            EfficiencyService efficiencyService = new EfficiencyService(_context);

            return Ok(efficiencyService.GetEfficiencyModelFromLocation(location, item));

        }



        /// <summary>
        /// gives the material efficiency rate and the best location  on a item Id
        /// </summary>
        /// <param name="ItemId" example="24692">item IDd</param>
        /// <response code="200">efficiency</response>
        [HttpGet("chooseStation")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EfficiencyModel))]
        public IActionResult GetBestEfficiency([FromQuery] int ItemId)
        {
            var tokenCharacter = TokenService.GetCharacterFromToken(_context, HttpContext);

            ItemService itemService = new ItemService(_context);
            var item = itemService.GetFromRecipe(ItemId);


            EfficiencyService efficiencyService = new EfficiencyService(_context);

            EfficiencyModel returnModel = null;

            LocationService locationService = new LocationService(_context);
            foreach (var location in locationService.GetList(tokenCharacter.CorporationId))
            {
                var temp = efficiencyService.GetEfficiencyModelFromLocation(location, item);
                if (returnModel ==null || returnModel.MEefficiency > temp.MEefficiency)
                    returnModel = temp;
            }
            return Ok(returnModel);

        }
    }
}
