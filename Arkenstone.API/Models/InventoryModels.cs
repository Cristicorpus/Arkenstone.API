using Arkenstone.Entities.DbSet;
using System.Collections.Generic;
using System.Linq;

namespace Arkenstone.API.Models
{

    public class InventoryModel
    {
        public long quantity { get; set; }
        public Item ItemModel { get; set; }

        public InventoryModel(Item item, long quantity)
        {
            this.quantity = quantity;
            this.ItemModel = item;
        }

    }
    public class InventorySubLocationModel
    {
        public SubLocation subLocation { get; set; }

        public List<InventoryModel> Assets { get; set; }

        public InventorySubLocationModel(SubLocation target)
        {
            this.subLocation = target;
            this.Assets = new List<InventoryModel>(); ;
        }
        public InventorySubLocationModel(SubLocation target, List<Entities.DbSet.Inventory> inventorys)
        {
            this.subLocation = target;
            this.Assets = new List<InventoryModel>();
            foreach (var asset in inventorys)
            {
                var temp = Assets.FirstOrDefault(x => x.ItemModel.Id == asset.Item.Id);
                if (temp == null)
                    Assets.Add(new InventoryModel(asset.Item, asset.Quantity));
                else
                    temp.quantity += asset.Quantity;
            }
        }
    }
    public class InventoryStationModel
    {
        public LocationModel structure { get; set; }

        public List<InventoryModel> Assets { get; set; }

        public InventoryStationModel(Location target)
        {
            this.structure = new LocationModel(target);
            this.Assets = new List<InventoryModel>(); ;
        }
        public InventoryStationModel(Location target, List<Entities.DbSet.Inventory> inventorys)
        {
            this.structure = new LocationModel(target);
            this.Assets = new List<InventoryModel>();
            foreach (var asset in inventorys)
            {
                var temp = Assets.FirstOrDefault(x => x.ItemModel.Id == asset.Item.Id);
                if (temp == null)
                    Assets.Add(new InventoryModel(asset.Item, asset.Quantity));
                else
                    temp.quantity += asset.Quantity;
            }
        }
    }

}
