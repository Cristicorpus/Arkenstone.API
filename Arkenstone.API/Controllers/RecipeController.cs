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
    public class RecipeController : Controller
    {
        private readonly ILogger<RecipeController> _logger;
        private readonly ArkenstoneContext _context;

        public RecipeController(ArkenstoneContext context, ILogger<RecipeController> logger)
        {
            _logger = logger;
            _context = context;
        }



        //GET api/recipe/ListRecipe
        [HttpGet("ListRecipeName")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<string>))]
        public IActionResult ListRecipe()
        {
            return Ok(_context.Recipes.Include("Item").Select(x => x.Item.Name).ToList());
        }

        // GET api/recipe?id=682
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RecipeModel))]
        public IActionResult GetRecipe(int id)
        {
            var recipe = _context.Recipes.Include("Item").Include("RecipeRessource.Item").FirstOrDefault(p => p.ItemId == id);

            if (recipe != null)
                return Ok(new RecipeModel(recipe));
            else
                return NotFound("La recette avec l'id '" + id + "' n'existe pas.");
        }
        
        
    }
    
    

}
