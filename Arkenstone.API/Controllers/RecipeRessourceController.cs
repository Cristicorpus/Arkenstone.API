using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using Arkenstone.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;

namespace Arkenstone.Controllers
{
    [Authorize(Policy = "Member")]
    [Route("api/[controller]")]
    [ApiController]
    public class RecipeRessourceController : Controller
    {
        private readonly ILogger<RecipeRessourceController> _logger;
        private readonly ArkenstoneContext _context;

        public RecipeRessourceController(ArkenstoneContext context, ILogger<RecipeRessourceController> logger)
        {
            _logger = logger;
            _context = context;
        }

        // GET api/<SessionController>/5
        //[HttpGet("{id}")]
        //public object Get(int id)
        //{
        //    if (_context.Recipes.Any(x => x.Id == id))
        //        return new RecipeModel(_context.Recipes.Include("Item").First(x => x.Id == id));
        //    else
        //        return new ErrorModel("La recette avec l'id '" + id + "' n'existe pas.");
        //}
    }
}
