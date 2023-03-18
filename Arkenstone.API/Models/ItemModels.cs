using Arkenstone.Entities.DbSet;

namespace Arkenstone.API.Models
{
    public class ItemModel
    {
        public ItemModel()
        { }
        public int Id { get; set; }
        public string Name { get; set; }

        public ItemModel(Item target)
        {
            this.Id = target.Id;
            this.Name = target.Name;
        }
    }
}
