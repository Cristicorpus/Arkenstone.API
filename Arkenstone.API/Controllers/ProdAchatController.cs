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
                returnvalue.Add(new ProdAchatModel(prodAchatService.Get(ProdAchatId.Value).ThrowNotAuthorized(tokenCharacter.CorporationId)));
            else
                returnvalue.AddRange(prodAchatService.GetList(tokenCharacter.CorporationId).Select(x => new ProdAchatModel(x)));

            if (returnvalue.Count() == 0)
                return NoContent();

            return Ok(returnvalue);
        }
        

        [HttpPut]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProdAchatModel))]
        public IActionResult CreateProdAchat([FromBody] ProdAchatModel ProdAchatModel)
        {
            var tokenCharacter = TokenService.GetCharacterFromToken(_context, HttpContext);

            ProdAchatService prodAchatService = new ProdAchatService(_context);

            prodAchatService.ValidateUpdateModel(tokenCharacter.CorporationId, ProdAchatModel, null);

            var prodAchat = prodAchatService.Create(tokenCharacter.CorporationId, ProdAchatModel);

            if(ProdAchatModel.Type == Entities.DbSet.ProdAchatTypeEnum.production)
                prodAchatService.CreateAllChild(tokenCharacter.CorporationId, prodAchat);

            return Ok(new ProdAchatModel(prodAchatService.Get(prodAchat.Id)));
        }



        // POST api/ticket
        //[HttpPost]
        //[Authorize(Policy = "Member")]
        //public void PostTicket([FromBody]TicketModelPost ticket, [FromQuery]bool genSubTicket)
        //{
        //    var ticketDb = ticket.getTicketDb(_context, genSubTicket);
        //    _context.Tickets.Add(ticket.getTicketDb(_context, genSubTicket));
        //    _context.SaveChanges();
        //}

    }
}
