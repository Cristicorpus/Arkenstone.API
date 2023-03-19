using Arkenstone.Entities.DbSet;
using NuGet.Packaging;
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
        public List<RigsManufacturing> RigsManufacturings { get; set; }

        public LocationModel(Location target)
        {
            this.Id = target.Id;
            this.Name = target.Name;
            this.Security = target.Security;

            if (target.StructureTypeId.HasValue && target.StructureTypeId.Value != 0)
                this.structureType = target.StructureType.Item;
            else
                this.structureType = new Item() { Id = 2071, Name = "Station" };

            if (target.LocationRigsManufacturings != null)
            {
                this.RigsManufacturings = new List<RigsManufacturing>();
                this.RigsManufacturings.AddRange(target.LocationRigsManufacturings.Select(x => x.RigsManufacturing));
            }

        }
    }
}
