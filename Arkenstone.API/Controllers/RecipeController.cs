using Arkenstone.API.Controllers;
using Arkenstone.API.Models;
using Arkenstone.API.Services;
using Arkenstone.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace Arkenstone.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class RecipeController : OriginController
    {
        
        public RecipeController(ArkenstoneContext context) : base(context)
        {

        }

        /// <summary>
        /// provides the recipe name
        /// </summary>
        /// <response code="200">list of assets</response>
        [HttpGet("ListRecipe")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<object>))]
        public IActionResult ListRecipe()
        {
            ItemService itemService = new ItemService(_context);
            var returnvalue = itemService.GetListProductible().Select(x => new {x.Id,x.Name}).ToList();

            if (returnvalue.Count == 0)
                return NoContent();

            return Ok(returnvalue);
        }

        /// <summary>
        /// provides the recipe material for an item if this item is manufacturable
        /// </summary>
        /// <response code="200">list of assets</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RecipeModel))]
        public IActionResult GetRecipe(int id)
        {
            var recipe = _context.Recipes.Include("Item").Include("RecipeRessource.Item").FirstOrDefault(p => p.ItemId == id);

            ItemService itemService = new ItemService(_context);
            return Ok(new RecipeModel (itemService.GetRecipe(id)));
        }
        
    }
    
    

}
