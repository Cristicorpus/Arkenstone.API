using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arkenstone.Entities.DbSet
{
    [Table("LocationRigsManufacturings")]
    public class LocationRigsManufacturing
    {
        [Key]
        [Column(Order = 0, TypeName = "bigint")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long LocationId { get; set; }
        [ForeignKey("LocationId")]
        public virtual Location Location { get; set; }
        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int RigsManufacturingId { get; set; }
        [ForeignKey("RigsManufacturingId")]
        public virtual RigsManufacturing RigsManufacturing { get; set; }
    }
}
