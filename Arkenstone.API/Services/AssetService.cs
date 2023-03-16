using Arkenstone.API.Models;
using Arkenstone.Entities;
using Arkenstone.Entities.DbSet;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Arkenstone.API.Services
{
    public class AssetService
    {
        private ArkenstoneContext _context;

        public AssetService(ArkenstoneContext context)
        {
            _context = context;
        }

        
        public List<AssetModel> GetGlobalAsset(int corpId)
        {
            List<Inventory> Asset = GetCore(corpId);
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
        public AssetStationModel GetStationAsset(int corpId, long LocationId)
        {
            List<Inventory> Asset = GetCore(corpId,LocationId);

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


        private List<Inventory> GetCore(int corpId, long? LocationId = null)
        {
            var request = _context.Inventorys.Include("Item").Include("SubLocation.Location.StructureType");

            if (LocationId == null)
                return request.ToList();
            else
                return request.Where(x => x.SubLocation.Location.Id == LocationId.Value).ToList(); ;
        }

    }
}
