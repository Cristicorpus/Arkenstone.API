using Arkenstone.Entities.DbSet;
using System.Collections.Generic;
using System.Linq;

namespace Arkenstone.API.Models
{
    public class LocationModel
    {
        public LocationModel()
        { }
        public long Id { get; set; }
        public string Name { get; set; }
        public Item structureType { get; set; }
        public decimal Security { get; set; }
        public string RawFit { get; set; }
        public virtual ICollection<RigsManufacturing> RigsManufacturings { get; set; }

        public LocationModel(Location target)
        {
            this.Id = target.Id;
            this.Name = target.Name;
            if (target.StructureTypeId.HasValue && target.StructureTypeId.Value != 0)
                this.structureType = target.StructureType.Item;
            else
                this.structureType = new Item() { Id = 2071, Name = "Station" };
        }
    }
}
