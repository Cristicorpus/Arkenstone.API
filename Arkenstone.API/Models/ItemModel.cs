using Arkenstone.Entities.DbSet;

namespace Arkenstone.API.Models
{
    public class ItemModel
    {
        public int Id { get; set; }
        public bool Published { get; set; }
        public string Name { get; set; }
        public int MarketGroupId { get; set; }

        public ItemModel(Item target)
        {
            this.Id = target.Id;
            this.Published = target.Published;
            this.Name = target.Name;
            this.MarketGroupId = target.MarketGroupId;
        }
    }
}
