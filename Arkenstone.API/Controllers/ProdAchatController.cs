using Arkenstone.API.Controllers;
using Arkenstone.API.Models;
using Arkenstone.API.Services;
using Arkenstone.Logic.Repository;
using Arkenstone.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace Arkenstone.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProdAchatController : OriginController
    {
        public ProdAchatController(ArkenstoneContext context) : base(context)
        {

        }


        /// <summary>
        /// Create an ProdAchat
        /// </summary>
        /// <param name="ProdAchatId" example="1">ProdAchat Id</param>
        [HttpGet]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ProdAchatModel>))]
        public IActionResult ListProdAchatRoot([FromQuery] long? ProdAchatId)
        {
            var tokenCharacter = TokenService.GetCharacterFromToken(_context, HttpContext);
            
            ProdAchatService prodAchatService = new ProdAchatService(_context);
            var returnvalue = new List<ProdAchatModel>();
            if (ProdAchatId.HasValue)
            {
                var prodAchatReturned = prodAchatService.Get(ProdAchatId.Value).ThrowNotAuthorized(tokenCharacter.CorporationId);
                returnvalue.Add(prodAchatService.GetModel(prodAchatReturned.Id));
            }
            else
            {
                foreach (var prodAchat in prodAchatService.GetList(tokenCharacter.CorporationId))
                    returnvalue.Add(prodAchatService.GetModel(prodAchat.Id));
            }
                

            if (returnvalue.Count() == 0)
                return NoContent();

            return Ok(returnvalue);
        }

        [HttpPost]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProdAchatModel))]
        public IActionResult CreateProdAchat([FromBody] ProdAchatModel ProdAchatModel)
        {
            var tokenCharacter = TokenService.GetCharacterFromToken(_context, HttpContext);

            ProdAchatService prodAchatService = new ProdAchatService(_context);

            prodAchatService.ValidateCreateModel(tokenCharacter.CorporationId, ProdAchatModel, null);

            var prodAchat = prodAchatService.Create(tokenCharacter.CorporationId, ProdAchatModel);            

            return Ok(prodAchatService.GetModel(prodAchat.Id));
        }

        [HttpPut]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProdAchatModel))]
        public IActionResult EditProdAchat([FromQuery] long ProdAchatId, [FromBody] ProdAchatModel ProdAchatModel)
        {
            var tokenCharacter = TokenService.GetCharacterFromToken(_context, HttpContext);

            ProdAchatService prodAchatService = new ProdAchatService(_context);

            var prodAchatDb = prodAchatService.Get(ProdAchatId).ThrowNotAuthorized(tokenCharacter.CorporationId);
            
            prodAchatService.ValidateUpdateModel(tokenCharacter.CorporationId, ProdAchatModel, prodAchatDb);     
            
            prodAchatDb = prodAchatService.UpdateProdAchat(prodAchatDb, ProdAchatModel);

            return Ok(prodAchatService.GetModel(ProdAchatId));
        }

    }
}
