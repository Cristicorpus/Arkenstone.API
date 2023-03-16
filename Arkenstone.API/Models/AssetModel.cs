using Arkenstone.Entities.DbSet;
using System.Collections.Generic;

namespace Arkenstone.API.Models
{
    public class AssetModel
    {
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
        public SubLocation subLocation { get; set; }

        public List<AssetModel> Assets { get; set; }

        public AssetSubLocationModel(SubLocation target)
        {
            this.subLocation = target;
            this.Assets = new List<AssetModel>(); ;
        }
    }
    public class AssetStationModel
    {
        public LocationModel structure { get; set; }

        public List<AssetModel> Assets { get; set; }

        public AssetStationModel(Location target)
        {
            this.structure = new LocationModel(target);
            this.Assets = new List<AssetModel>(); ;
        }
    }
}
