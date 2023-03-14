using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using Arkenstone.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Arkenstone.Entities;

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

        // GET api/recipe/ListRecipe
        //[HttpGet("ListRecipe")]
        //[Authorize(Policy = "Member")]
        //public IQueryable<RecipeModel> ListRecipe()
        //{
        //    return _context.Recipes.Include("Item").Include("RecipeRessource.Item").Select(x => new RecipeModel(x));
        //}

        // GET api/recipe/GetRecipe?id=682
        //[HttpGet("GetRecipe")]
        //[Authorize(Policy = "Member")]
        //public object GetRecipe(int id)
        //{
        //    if (_context.Recipes.Any(x => x.Id == id))
        //        return new RecipeModel(_context.Recipes.Include("Item").Include("RecipeRessource.Item").First(p => p.Id == id));
        //    else
        //        return NotFound("La recette avec l'id '" + id + "' n'existe pas.");
        //}

        // GET api/recipe/ListNameRecipe
        //[HttpGet("ListNameRecipe")]
        //[Authorize(Policy = "Member")]
        //public IQueryable<string> ListNameRecipe()
        //{
        //    return _context.Recipes.Include("Item").Where(f => f.Item.Name!="").Select(x => x.Item.Name);
        //}

        // GET api/recipe/NameRecipe?id=5
        //[HttpGet("NameRecipe")]
        //[Authorize(Policy = "Member")]
        //public object NameRecipe(int id)
        //{
        //    if (_context.Recipes.Any(x => x.Id == id))
        //        return _context.Recipes.Include("Item").First(x => x.Id == id).Item.Name;
        //    else
        //        return NotFound("La recette avec l'id '" + id + "' n'existe pas.");


        //}
    }
    
    

}
