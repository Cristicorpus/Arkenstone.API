using Arkenstone.Logic.Asset;
using Arkenstone.Logic.BusinessException;
using Arkenstone.Entities;
using Arkenstone.Entities.DbSet;
using Microsoft.EntityFrameworkCore;

namespace Arkenstone.Logic.Repository
{
    public class InventoryRepository
    {
        private ArkenstoneContext _context;

        public InventoryRepository(ArkenstoneContext context)
        {
            _context = context;
        }
        private IQueryable<Inventory> GetCore()
        {
            return _context.Inventorys.Include("Item").Include("SubLocation.Location");
        }
        
        public List<Inventory> GetList(int corpId)
        {
            var temp = GetCore().Where(x => x.SubLocation.CorporationId == corpId).ToList();
            return temp;
        }
        public List<Inventory> GetListFromSubLocation(long subLocation)
        {
            var temp = GetCore().Where(x => x.SubLocationId == subLocation).ToList();
            return temp;
        }
        public List<Inventory> GetListFromLocation(long location)
        {
            var temp = GetCore().Where(x => x.SubLocation.LocationId == location).ToList();
            return temp;
        }
        public Inventory GetInventory(long subLocation, int itemId)
        {
            var temp = GetCore().FirstOrDefault(x => x.SubLocationId == subLocation && x.ItemId == itemId);
            if (temp == null)
                throw new NotFound("Inventory");
            return temp;
        }


    }
}
