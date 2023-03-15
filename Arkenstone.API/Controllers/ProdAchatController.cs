using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Arkenstone.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Arkenstone.ControllerModel;
using Microsoft.AspNetCore.Cors;
using Arkenstone.API.ControllerModel;
using Microsoft.AspNetCore.Http;
using Arkenstone.Entities.DbSet;

namespace Arkenstone.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProdAchatController : Controller
    {
        private readonly ILogger<RecipeController> _logger;
        private readonly ArkenstoneContext _context;

        public ProdAchatController(ArkenstoneContext context, ILogger<RecipeController> logger)
        {
            _logger = logger;
            _context = context;
        }

        // GET api/ProdAchat
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ProdAchatModel>))]
        public IActionResult ListProdAchatRoot([FromQuery] long? ProdAchatId)
        {
            List<ProdAchatModel> returnModel = new List<ProdAchatModel>();

            if (ProdAchatId == null)
            {
                foreach (var item in _context.ProdAchats.Where(x => x.ProdAchatParentId == null).Include("Location").Include("Item"))
                {
                    returnModel.Add(new ProdAchatModel(item));
                }
            }
            else
            {
                var ProdAchat = _context.ProdAchats.Include("Location").Include("Item").Include("ProdAchatEnfant.Item").Include("ProdAchatEnfant.Location").Include("ProdAchatParent.Item").Include("ProdAchatParent.Location").FirstOrDefault(x => x.Id == ProdAchatId);
                if(ProdAchat!=null)
                    returnModel.Add(new ProdAchatModel(ProdAchat));
            }

            return Ok(returnModel);
        }
        
        // PUT api/ProdAchat
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProdAchatModel))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult CreateProdAchat([FromQuery] ProdAchatModel ProdAchatModel)
        {
            if(ProdAchatModel.Item == null || ProdAchatModel.Item.Id == null)
                return BadRequest("Item is null");
            var item = _context.Items.Find(ProdAchatModel.Item.Id);
            if (item == null)
                return BadRequest("Item is not recognized");


            if (ProdAchatModel.Location == null || ProdAchatModel.Location.Id == null)
                return BadRequest("Location is null");
            var location = _context.Locations.Find(ProdAchatModel.Location.Id);
            if (location == null)
                return BadRequest("Location is not recognized");


            if (ProdAchatModel.ProdAchatParent != null)
                return BadRequest("ProdAchatParent is not null");


            if (ProdAchatModel.Quantity >0)
                return BadRequest("Quantity is inferior or equal to 0");

            switch (ProdAchatModel.Type)
            {
                case Entities.DbSet.ProdAchatTypeEnum.achat:
                    if (ProdAchatModel.MEefficiency.HasValue)
                        return BadRequest("MEefficiency has value");
                    break;
                case Entities.DbSet.ProdAchatTypeEnum.production:
                    if (!ProdAchatModel.MEefficiency.HasValue)
                        return BadRequest("MEefficiency has not value");
                    break;
                default:
                    return BadRequest("Type is not recognized");
            }

            
            List<Arkenstone.Entities.DbSet.ProdAchat> childProdAchat = new List<ProdAchat>();
            if(ProdAchatModel.Type == Entities.DbSet.ProdAchatTypeEnum.production)
            {
                //TODO AJOUTER LES TICKET ENFANT SI C EST UNE PROD
                //sachant que les ticket enfant seront pour le moment automatiquement de type achat
            }




            
            var ProdAchat = new Arkenstone.Entities.DbSet.ProdAchat()
            {
                ItemId = item.Id,
                LocationId = location.Id,
                Quantity = ProdAchatModel.Quantity,
                MEefficiency = ProdAchatModel.MEefficiency,
                Type = ProdAchatModel.Type
            };

            _context.ProdAchats.Add(ProdAchat);
            _context.SaveChanges();

            return Ok(new ProdAchatModel(ProdAchat));
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
