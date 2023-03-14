using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arkenstone.Entities.DbSet
{
    [Table("Inventorys")]
    public class Inventory
    {
        public Inventory()
        {

        }
        [Key]
        [Column(Order = 0)]
        public int ItemId { get; set; }
        public virtual Item Item { get; set; }

        [Key]
        [Column(Order = 1, TypeName = "bigint")]
        public long SubLocationId { get; set; }
        public virtual SubLocation SubLocation { get; set; }
        

        public int Quantity { get; set; }
    }
}
