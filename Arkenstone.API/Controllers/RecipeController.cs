using Arkenstone.API.Controllers;
using Arkenstone.API.Models;
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




        [HttpGet("ListRecipeName")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<string>))]
        public IActionResult ListRecipe()
        {
            return Ok(_context.Recipes.Include("Item").Select(x => x.Item.Name).ToList());
        }


        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RecipeModel))]
        public IActionResult GetRecipe(int id)
        {
            var recipe = _context.Recipes.Include("Item").Include("RecipeRessource.Item").FirstOrDefault(p => p.ItemId == id);

            if (recipe != null)
                return Ok(new RecipeModel(recipe));
            else
                return NotFound();
        }
        
        
    }
    
    

}
