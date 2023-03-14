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

namespace Arkenstone.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketController : Controller
    {
        private readonly ILogger<RecipeController> _logger;
        private readonly ArkenstoneContext _context;

        public TicketController(ArkenstoneContext context, ILogger<RecipeController> logger)
        {
            _logger = logger;
            _context = context;
        }

        // GET api/ticket/listticket
        //[HttpGet("ListTicket")]
        //[Authorize(Policy = "Member")]
        //public IQueryable<TicketModel> ListTicket(bool mainTicketOnly)
        //{
        //    if (mainTicketOnly)
        //        return _context.Tickets
        //            .Include("Item")
        //            .Include("TicketParent")
        //            .Include("TicketEnfant")
        //            .Where(x => x.TicketParent == null)
        //            .Select(x => new TicketModel(x));
        //    else
        //        return _context.Tickets
        //             .Select(x => new TicketModel(x));
        //}

        // GET api/ticket/{id}
        //[HttpGet("{id}")]
        //[Authorize(Policy = "Member")]
        //public TicketModel getTicket(int id)
        //{
        //    Ticket ticket = _context.Tickets
        //        .Include("Item")
        //        .Include("TicketParent")
        //        .Include("TicketEnfant")
        //        .Single(x => x.Id == id);

        //    return new TicketModel(ticket);
        //}

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
