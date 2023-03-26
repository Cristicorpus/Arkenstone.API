using Arkenstone.Entities;
using Arkenstone.Entities.DbSet;
using Arkenstone.Logic.Asset;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Arkenstone.API.Services
{
    public class InventoryService : BaseService
    {
        public InventoryService(ArkenstoneContext context) : base(context)
        {

        }        
        public List<Inventory> GetList(int corpId)
        {
            return inventoryRepository.GetList(corpId);
        }
        public List<Inventory> GetListFromLocation(long location)
        {
            return inventoryRepository.GetListFromLocation(location);
        }
        public List<Inventory> GetListFromSubLocation(long subLocation)
        {
            return inventoryRepository.GetListFromSubLocation(subLocation);
        }
        public Inventory GetInventory(long subLocation, int itemId)
        {
            return inventoryRepository.GetInventory(subLocation, itemId);
        }

        public async Task RefreshAsset(int corpId)
        {
            await AssetDump.ReloadItemsFromSpecificCorpAsync(_context, corpId);
        }


    }
}
