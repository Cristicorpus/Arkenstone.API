using Arkenstone.API.ControllerModel;
using Arkenstone.Controllers;
using Arkenstone.Entities;
using Arkenstone.Logic.Efficiency;
using Arkenstone.Logic.Structure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
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
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EfficiencyModel))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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

                var returnModel = new EfficiencyModel();
                
                decimal StructureEfficiency = EfficiencyStructure.GetMEEfficiencyFromStation(_context, structure);
                EfficiencyStructureRigsEffect RigsEfficiency = EfficiencyStructure.GetMEEfficiencyFromRigs(_context, structure, item);

                returnModel.MEefficiency = (StructureEfficiency * RigsEfficiency.MeEfficiency);
                returnModel.Station = new StructureModel(structure);
                returnModel.rigsEffect = RigsEfficiency.rigsManufacturings;

                return Ok(returnModel);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        // GET api/Efficiency
        [HttpGet("chooseStation")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EfficiencyModel))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetBestEfficiency([FromQuery] int ItemId)
        {
            try
            {
                var item = _context.Items.Find(ItemId);
                if (item == null)
                    return NotFound("Item not found");

                var returnModel = new EfficiencyModel();
                returnModel.MEefficiency = 5;
                
                foreach (var locationId in _context.SubLocations.Select(x=>x.LocationId).Distinct().ToList())
                {
                    var structure = _context.Locations.Find(locationId);
                    if (structure == null)
                        return NotFound("Structure not found");

                    decimal StructureEfficiency = EfficiencyStructure.GetMEEfficiencyFromStation(_context, structure);
                    EfficiencyStructureRigsEffect RigsEfficiency = EfficiencyStructure.GetMEEfficiencyFromRigs(_context, structure, item);

                    if(returnModel.MEefficiency>(StructureEfficiency* RigsEfficiency.MeEfficiency))
                    {
                        returnModel.MEefficiency = (StructureEfficiency * RigsEfficiency.MeEfficiency);
                        returnModel.Station = new StructureModel(structure);
                        returnModel.rigsEffect = RigsEfficiency.rigsManufacturings;
                    }
                    
                }
                return Ok(returnModel);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
