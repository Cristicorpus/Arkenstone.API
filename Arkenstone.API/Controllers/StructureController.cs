using Arkenstone.API.ControllerModel;
using Arkenstone.Controllers;
using Arkenstone.Entities;
using Arkenstone.Logic.Structure;
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
    public class StructureController : Controller
    {
        private readonly ILogger<RecipeController> _logger;
        private readonly ArkenstoneContext _context;

        public StructureController(ArkenstoneContext context, ILogger<RecipeController> logger)
        {
            _logger = logger;
            _context = context;
        }

        // GET api/structure
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<StructureModel>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetSimple([FromQuery] long? LocationId)
        {
            try
            {
                List<StructureModel> StructureList = new List<StructureModel>();
                if (LocationId == null)
                {
                    foreach (var Structure in _context.Locations)
                    {
                        StructureList.Add(new StructureModel(Structure));
                    }
                }
                else
                {
                    var targetStructure = _context.Locations.Find(LocationId.Value);
                    if (targetStructure == null)
                        return NotFound();
                    else
                        StructureList.Add(new StructureModel(targetStructure));
                }

                return Ok(StructureList);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        // GET api/structure/Detailed
        [HttpGet("Detailed")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<StructureModelDetails>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetDetailed([FromQuery] long? LocationId)
        {
            try
            {
                List<StructureModelDetails> StructureList = new List<StructureModelDetails>();
                if (LocationId == null)
                {
                    foreach (var Structure in _context.Locations)
                    {
                        StructureList.Add(new StructureModelDetails(Structure));
                    }
                }
                else
                {
                    var targetStructure = _context.Locations.Find(LocationId.Value);
                    if (targetStructure == null)
                        return NotFound();
                    else
                        StructureList.Add(new StructureModelDetails(targetStructure));
                }

                return Ok(StructureList);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST api/structure
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<StructureModelDetails>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult SetFit([FromQuery] long LocationId, [FromBody] StructureModelDetails PostModel)
        {
            try
            {
                StructureEdit.SetFitToStructure(LocationId, PostModel.RawFit);
                var targetStructure = _context.Locations.Include("LocationRigsManufacturings.RigsManufacturing").First(x=>x.Id == LocationId);
                return Ok(new StructureModelDetails(targetStructure));
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
