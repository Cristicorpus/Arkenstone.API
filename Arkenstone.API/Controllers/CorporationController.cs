using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Arkenstone.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Arkenstone.ControllerModel;
using System.Collections;
using System.Collections.Generic;

namespace Arkenstone.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class CorporationController : Controller
    {

        private readonly ILogger<CorporationController> _logger;
        private readonly ArkenstoneContext _context;

        public CorporationController(ArkenstoneContext context, ILogger<CorporationController> logger)
        {
            _logger = logger;
            _context = context;
        }

        // GET: CorporationController/items
        //[HttpGet("items")]
        //[Authorize(Policy = "Member")]
        //public IEnumerable<InventoryModel> itemsList(string itemName, ulong locationId, int itemId)
        //{

        //    System.Func<Inventory, bool> filteritem = (Inventory inventory) =>
        //    {
        //        bool value = true;

        //        if (itemName != null)
        //            value &= inventory.Item.Name == itemName;

        //        if (locationId != 0)
        //            value &= inventory.LocationId == locationId;

        //        if (itemId != 0)
        //            value &= inventory.Item.Id == itemId;

        //        return value;
        //    };

        //    return _context.Inventorys.Include("Item").Include("Location").Where(filteritem).Select(x => new InventoryModel(x));
        //}

        // GET: CorporationController/blueprints
        //[HttpGet("blueprints")]
        //[Authorize(Policy = "Member")]
        //public IEnumerable<BlueprintModel> blueprintsList(string itemName, ulong locationId, int itemId)
        //{

        //    System.Func<InventoryBlueprint, bool> filteritem = (InventoryBlueprint inventory) =>
        //    {
        //        bool value = true;

        //        if (itemName != null)
        //            value &= inventory.Item.Name == itemName;

        //        if (locationId != 0)
        //            value &= inventory.LocationId == locationId;

        //        if (itemId != 0)
        //            value &= inventory.Item.Id == itemId;

        //        return value;
        //    };
        //    return _context.InventoryBlueprints.Include("Item").Include("Location").Where(filteritem).Select(x => new BlueprintModel(x));
        //}

        // GET: CorporationController/items/{id}
        //[HttpGet("items/{id}")]
        //[Authorize(Policy = "Member")]
        //public InventoryModel items(ulong id)
        //{
        //    return new InventoryModel(_context.Inventorys.Include("Item").Include("Location").Where(x => x.Id == id).First());
        //}

        // GET: CorporationController/blueprints//{id}
        //[HttpGet("blueprints/{id}")]
        //[Authorize(Policy = "Member")]
        //public BlueprintModel blueprints(ulong id)
        //{
        //    return new BlueprintModel(_context.InventoryBlueprints.Include("Item").Include("Location").Where(x => x.Id == id).First());
        //}
    }
}
