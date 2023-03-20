using Arkenstone.API.Controllers;
using Arkenstone.API.Models;
using Arkenstone.API.Services;
using Arkenstone.Entities;
using Arkenstone.Entities.DbSet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
                var returnModel = new ProdAchatModel(prodAchatReturned);
                returnModel.CostJob = prodAchatService.CostPriceFromProduction(prodAchatReturned.Location, prodAchatReturned.Item);
                returnvalue.Add(returnModel);
            }
            else
            {
                foreach (var prodAchat in prodAchatService.GetList(tokenCharacter.CorporationId))
                {
                    var returnModel = new ProdAchatModel(prodAchat);
                    returnModel.CostJob = prodAchatService.CostPriceFromProduction(prodAchat.Location, prodAchat.Item);
                    returnvalue.Add(returnModel);
                }
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

            prodAchatService.ValidateUpdateModel(tokenCharacter.CorporationId, ProdAchatModel, null);

            var prodAchat = prodAchatService.Create(tokenCharacter.CorporationId, ProdAchatModel);
            _context.SaveChanges();

            if (ProdAchatModel.Type == Entities.DbSet.ProdAchatTypeEnum.production)
                prodAchatService.UpdateChilds(tokenCharacter.CorporationId, prodAchat);

            _context.SaveChanges();

            var prodAchatReturned = prodAchatService.Get(prodAchat.Id);
            var returnvalue = new ProdAchatModel(prodAchatReturned);
            returnvalue.CostJob = prodAchatService.CostPriceFromProduction(prodAchatReturned.Location, prodAchatReturned.Item);

            return Ok(returnvalue);
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
            prodAchatService.CompareModelWithDb_Item(prodAchatDb, ProdAchatModel);

            var ProjectedStateChild = prodAchatService.GetProjectedStateChild(prodAchatDb, ProdAchatModel);

            prodAchatDb = prodAchatService.UpdateProdAchat(prodAchatDb, ProdAchatModel);
            
            switch (ProjectedStateChild)
            {
                case ProjectedStateChild.delete:
                    prodAchatService.DeleteChilds(prodAchatDb);
                    break;
                case ProjectedStateChild.create: case ProjectedStateChild.update:
                    prodAchatService.UpdateChilds(tokenCharacter.CorporationId, prodAchatDb);
                    break;
                default:
                    break;
            }
            _context.SaveChanges();

            var prodAchatReturned = prodAchatService.Get(ProdAchatId);
            var returnvalue = new ProdAchatModel(prodAchatReturned);
            returnvalue.CostJob = prodAchatService.CostPriceFromProduction(prodAchatReturned.Location, prodAchatReturned.Item);

            return Ok(returnvalue);
        }

    }
}
