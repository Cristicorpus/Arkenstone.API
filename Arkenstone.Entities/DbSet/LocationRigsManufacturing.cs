using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arkenstone.Entities.DbSet
{
    [Table("LocationRigsManufacturings")]
    public class LocationRigsManufacturing
    {
        public long LocationId { get; set; }
        public virtual Location Location { get; set; }
        public int RigsManufacturingId { get; set; }
        public virtual RigsManufacturing RigsManufacturing { get; set; }
    }
}
