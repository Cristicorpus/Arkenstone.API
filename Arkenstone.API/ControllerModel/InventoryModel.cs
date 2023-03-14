using Arkenstone.Entities.DbSet;

namespace Arkenstone.ControllerModel
{
    public class InventoryModel
    {
        public int Quantity { get; set; }

        public virtual Item Item { get; set; }

        public virtual SubLocation SubLocation { get; set; }


        public InventoryModel(Inventory inventory)
        {
            this.Quantity = inventory.Quantity;
            this.Item = inventory.Item;
            this.SubLocation = inventory.SubLocation;
        }
    }
}
