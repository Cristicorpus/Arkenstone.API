using Arkenstone.API.ControllerModel;
using Arkenstone.Controllers;
using Arkenstone.Entities;
using Arkenstone.Logic.Structure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Arkenstone.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StructureController : Controller
    {
        private readonly ILogger<RecipeController> _logger;
        private readonly ArkenstoneContext _context;

        public StructureController(ArkenstoneContext context, ILogger<RecipeController> logger)
        {
            _logger = logger;
            _context = context;
        }


        // POST api/structure
        [HttpPost]
        public IActionResult PostTicket([FromQuery] long LocationId, [FromBody] StructureModelDetails PostModel)
        {
            try
            {
                StructureEdit.SetFitToStructure(LocationId, PostModel.RawFit);
                return Ok();
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
