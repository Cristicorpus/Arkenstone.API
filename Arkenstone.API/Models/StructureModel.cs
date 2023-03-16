using Arkenstone.Entities.DbSet;
using System.Collections.Generic;
using System.Linq;

namespace Arkenstone.API.Models
{
    public class StructureModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public int structureTypeId { get; set; }
        public string structureTypeName { get; set; }

        public StructureModel(Location target)
        {
            this.Id = target.Id;
            this.Name = target.Name;
            if (target.StructureTypeId.HasValue)
            {
                this.structureTypeId = target.StructureType.Id;
                this.structureTypeName = target.StructureType.Name;
            }
        }
    }
    public class StructureModelDetails
    {
        public StructureModel structure { get; set; }
        public decimal Security { get; set; }
        public string RawFit { get; set; }
        public virtual ICollection<RigsManufacturing> RigsManufacturings { get; set; }
        

        public StructureModelDetails(Location target)
        {
            this.structure = new StructureModel(target);
            this.Security = target.Security;
            this.RigsManufacturings = target.LocationRigsManufacturings.Select(x => x.RigsManufacturing).ToList();
        }
    }
}
