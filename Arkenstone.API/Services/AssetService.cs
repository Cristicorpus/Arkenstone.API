using Arkenstone.API.Models;
using Arkenstone.Entities;
using Arkenstone.Entities.DbSet;
using Arkenstone.Logic.Asset;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Arkenstone.API.Services
{
    public class AssetService
    {
        private ArkenstoneContext _context;

        public AssetService(ArkenstoneContext context)
        {
            _context = context;
        }
        private IQueryable<Inventory> GetCore()
        {
            return _context.Inventorys.Include("Item").Include("SubLocation");
        }

        public List<AssetModel> GetGlobalAsset(int corpId)
        {
            List<Inventory> Asset = GetCore().Where(x => x.SubLocation.CorporationId == corpId).ToList();
            List<AssetModel> result = new List<AssetModel>();
            foreach (var item in Asset)
            {
                var itemAsset = result.FirstOrDefault(x => x.ItemModel.Id == item.ItemId);
                if (itemAsset == null)
                {
                    itemAsset = new AssetModel(item.Item, 0);
                    result.Add(itemAsset);
                }
                itemAsset.quantity += item.Quantity;
            }
            Asset = null;
            return result;
        }
        public AssetStationModel GetLocationAsset( long LocationId)
        {
            List<Inventory> Asset = GetCore().Where(x => x.SubLocation.LocationId == LocationId).ToList();

            var location = _context.Locations.Include("StructureType").FirstOrDefault(x=>x.Id ==LocationId);
            AssetStationModel result = new AssetStationModel(location);
            
            foreach (var item in Asset)
            {
                var itemAsset = result.Assets.FirstOrDefault(x => x.ItemModel.Id == item.ItemId);
                if (itemAsset == null)
                {
                    itemAsset = new AssetModel(item.Item, 0);
                    result.Assets.Add(itemAsset);
                }
                itemAsset.quantity += item.Quantity;
            }
            Asset = null;
            return result;
        }
        public AssetSubLocationModel GetSubLocationAsset(long LocationId)
        {
            List<Inventory> Asset = GetCore().Where(x => x.SubLocationId == LocationId).ToList();

            var location = _context.SubLocations.FirstOrDefault(x => x.Id == LocationId);
            AssetSubLocationModel result = new AssetSubLocationModel(location);
            foreach (var item in Asset)
            {
                var itemAsset = result.Assets.FirstOrDefault(x => x.ItemModel.Id == item.ItemId);
                if (itemAsset == null)
                {
                    itemAsset = new AssetModel(item.Item, 0);
                    result.Assets.Add(itemAsset);
                }
                itemAsset.quantity += item.Quantity;
            }
            Asset = null;
            return result;
        }

        public async Task RefreshAsset(int corpId)
        {
            await AssetDump.ReloadItemsFromSpecificCorpAsync(corpId);
        }


    }
}
