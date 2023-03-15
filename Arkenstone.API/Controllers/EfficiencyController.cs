using Arkenstone.Controllers;
using Arkenstone.Entities;
using Arkenstone.Logic.Efficiency;
using Arkenstone.Logic.Structure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace Arkenstone.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EfficiencyController : Controller
    {
        private readonly ILogger<RecipeController> _logger;
        private readonly ArkenstoneContext _context;

        public EfficiencyController(ArkenstoneContext context, ILogger<RecipeController> logger)
        {
            _logger = logger;
            _context = context;
        }


        // GET api/Efficiency
        [HttpGet]
        public IActionResult GetEfficiencyFromStation([FromQuery] long LocationId, [FromQuery] int ItemId)
        {
            try
            {
                var structure = _context.Locations.Find(LocationId);
                if (structure == null)
                    return NotFound("Structure not found");
                var item = _context.Items.Find(ItemId);
                if (item == null)
                    return NotFound("Item not found");

                decimal StructureEfficiency = EfficiencyStructure.GetMEEfficiencyFromStation(_context, structure);
                decimal RigsEfficiency = EfficiencyStructure.GetMEEfficiencyFromRigs(_context, structure, item);

                return Ok(StructureEfficiency* RigsEfficiency);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
