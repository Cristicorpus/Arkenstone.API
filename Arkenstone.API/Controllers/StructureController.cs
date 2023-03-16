using Arkenstone.API.Models;
using Arkenstone.API.Services;
using Arkenstone.Controllers;
using Arkenstone.Entities;
using Microsoft.AspNetCore.Authorization;
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
    public class StructureController : OriginController
    {
        public StructureController(ArkenstoneContext context) : base(context)
        {

        }

        // GET api/structure
        [HttpGet]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<StructureModel>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetSimple([FromQuery] long? LocationId)
        {
            var tokenCharacter = TokenService.GetCharacterFromToken(_context, HttpContext);
            if (tokenCharacter == null)
                return Unauthorized("You are not authorized");
            
            try
            {
                StructureService structureService = new StructureService(_context);
                return Ok(structureService.GetBasicModel(LocationId));
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        // GET api/structure/Detailed
        [HttpGet("Detailed")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<StructureModelDetails>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetDetailed([FromQuery] long? LocationId)
        {
            var tokenCharacter = TokenService.GetCharacterFromToken(_context, HttpContext);
            if (tokenCharacter == null)
                return Unauthorized("You are not authorized");
            
            try
            {
                StructureService structureService = new StructureService(_context);
                return Ok(structureService.GetDetailledModel(LocationId));
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST api/structure
        [HttpPost]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<StructureModelDetails>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult SetFit([FromQuery] long LocationId, [FromBody] StructureModelDetails PostModel)
        {

            var tokenCharacter = TokenService.GetCharacterFromToken(_context, HttpContext);
            if (tokenCharacter == null)
                return Unauthorized("You are not authorized");
            
            try
            {

                StructureService structureService = new StructureService(_context);
                structureService.SetFitToStructure(LocationId, PostModel.RawFit);
                return Ok(structureService.GetDetailledModel(LocationId));
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
