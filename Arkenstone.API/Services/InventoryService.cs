using Arkenstone.API.Models;
using Arkenstone.Entities;
using Arkenstone.Entities.DbSet;
using Arkenstone.Logic.Asset;
using Arkenstone.Logic.BusinessException;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Arkenstone.API.Services
{
    public class InventoryService
    {
        private ArkenstoneContext _context;

        public InventoryService(ArkenstoneContext context)
        {
            _context = context;
        }
        private IQueryable<Inventory> GetCore()
        {
            return _context.Inventorys.Include("Item").Include("SubLocation");
        }
        
        public List<Inventory> GetList(int corpId)
        {
            var temp = GetCore().Where(x => x.SubLocation.CorporationId == corpId).ToList();
            if (temp.Count() == 0)
                throw new NoContent("Inventory");
            return temp;
        }
        public List<Inventory> GetListFromLocation(long location)
        {
            var temp = GetCore().Where(x => x.SubLocation.LocationId == location).ToList();
            if (temp.Count() == 0)
                throw new NoContent("Inventory");
            return temp;
        }
        public List<Inventory> GetListFromSubLocation(long subLocation)
        {
            var temp = GetCore().Where(x => x.SubLocationId == subLocation).ToList();
            if (temp.Count() == 0)
                throw new NoContent("Inventory");
            return temp;
        }
        public Inventory GetInventory(long subLocation, int itemId)
        {
            var temp = GetCore().FirstOrDefault(x => x.SubLocationId == subLocation && x.ItemId == itemId);
            if (temp == null)
                throw new NotFound("Inventory");
            return temp;
        }

        public async Task RefreshAsset(int corpId)
        {
            await AssetDump.ReloadItemsFromSpecificCorpAsync(corpId);
        }


    }
}
