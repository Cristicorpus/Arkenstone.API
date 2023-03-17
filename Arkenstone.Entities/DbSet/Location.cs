using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arkenstone.Entities.DbSet
{
    [Table("Locations")]
    public class Location
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column(TypeName = "bigint")]
        public long Id { get; set; }
        
        public virtual ICollection<SubLocation> SubLocations { get; set; }
        public virtual ICollection<LocationRigsManufacturing> LocationRigsManufacturings { get; set; }

        public string Name { get; set; }
        
        public decimal Security { get; set; }
        
        public int? StructureTypeId { get; set; }
        [ForeignKey("StructureTypeId")]
        public virtual StructureType StructureType { get; set; }

        public bool CanReact { get; set; }
        public bool CanReprocess { get; set; }
        public bool CanProd { get; set; }
        public bool CanCopy { get; set; }
        public bool CanResearch { get; set; }
        public bool CanInvent { get; set; }

        public Location()
        {
            SubLocations = new HashSet<SubLocation>();
            LocationRigsManufacturings = new HashSet<LocationRigsManufacturing>();
        }

    }
}
