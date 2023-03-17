using Arkenstone.Entities.DbSet;
using System.Collections.Generic;
using System.Linq;

namespace Arkenstone.API.Models
{
    public class LocationModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public int structureTypeId { get; set; }
        public string structureTypeName { get; set; }

        public LocationModel(Location target)
        {
            this.Id = target.Id;
            this.Name = target.Name;
            if (target.StructureTypeId.HasValue && target.StructureTypeId.Value != 0)
            {
                this.structureTypeId = target.StructureType.Id;
                this.structureTypeName = target.StructureType.Name;
            }
            else
            {
                this.structureTypeId = 2071;
                this.structureTypeName = "Station"; 
            }
        }
    }
    public class LocationModelDetails
    {
        public LocationModel structure { get; set; }
        public decimal Security { get; set; }
        public string RawFit { get; set; }
        public virtual ICollection<RigsManufacturing> RigsManufacturings { get; set; }
        

        public LocationModelDetails(Location target)
        {
            this.structure = new LocationModel(target);
            this.Security = target.Security;
            this.RigsManufacturings = target.LocationRigsManufacturings.Select(x => x.RigsManufacturing).ToList();
        }
    }
}
