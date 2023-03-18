using Arkenstone.Entities.DbSet;
using ESI.NET.Models.Character;
using System.Collections.Generic;
using System.Linq;

namespace Arkenstone.API.Models
{
    public class AssetModel
    {
        public AssetModel()
        { }
        public long quantity { get; set; }
        public ItemModel ItemModel { get; set; }

        public AssetModel(Item item, long quantity)
        {
            this.quantity = quantity;
            this.ItemModel = new ItemModel(item);
        }

    }
    public class AssetSubLocationModel
    {
        public AssetSubLocationModel()
        { }
        public SubLocation subLocation { get; set; }

        public List<AssetModel> Assets { get; set; }

        public AssetSubLocationModel(SubLocation target)
        {
            this.subLocation = target;
            this.Assets = new List<AssetModel>(); ;
        }
        public AssetSubLocationModel(SubLocation target, List<Entities.DbSet.Inventory> inventorys)
        {
            this.subLocation = target;
            this.Assets = new List<AssetModel>();
            foreach (var asset in inventorys)
            {
                var temp = Assets.FirstOrDefault(x => x.ItemModel.Id == asset.Item.Id);
                if (temp == null)
                    Assets.Add(new AssetModel(asset.Item, asset.Quantity));
                else
                    temp.quantity += asset.Quantity;
            }
        }
    }
    public class AssetStationModel
    {
        public AssetStationModel()
        { }
        public LocationModel structure { get; set; }

        public List<AssetModel> Assets { get; set; }

        public AssetStationModel(Location target)
        {
            this.structure = new LocationModel(target);
            this.Assets = new List<AssetModel>(); ;
        }
        public AssetStationModel(Location target, List<Entities.DbSet.Inventory> inventorys)
        {
            this.structure = new LocationModel(target);
            this.Assets = new List<AssetModel>();
            foreach (var asset in inventorys)
            {
                var temp = Assets.FirstOrDefault(x => x.ItemModel.Id == asset.Item.Id);
                if (temp == null)
                    Assets.Add(new AssetModel(asset.Item, asset.Quantity));
                else
                    temp.quantity += asset.Quantity;
            }
        }
    }
}
