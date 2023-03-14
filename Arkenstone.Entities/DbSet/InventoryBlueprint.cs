using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arkenstone.Entities.DbSet
{
    [Table("InventoryBlueprints")]
    public class InventoryBlueprint
    {
        public InventoryBlueprint()
        {
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column(TypeName = "bigint")]
        public long Id { get; set; }

        [Column(TypeName = "bigint")]
        public long LocationId { get; set; }
        public virtual Location Location { get; set; }

        public int ItemId { get; set; }
        public virtual Item Item { get; set; }
        
        public decimal MaterialEfficiency { get; set; }
        public decimal TimeEfficiency { get; set; }
        public int CycleNumber { get; set; }

    }
}
