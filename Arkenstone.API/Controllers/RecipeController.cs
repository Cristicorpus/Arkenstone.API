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
        [HttpGet("ListRecipeName")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<string>))]
        public IActionResult ListRecipe()
        {
            ItemService itemService = new ItemService(_context);
            return Ok(itemService.GetListRecipe().Select(x => x.Name).ToList());
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
            return Ok(new RecipeModel (itemService.GetRessourceFromRecipe(id)));
        }
        
        
    }
    
    

}
