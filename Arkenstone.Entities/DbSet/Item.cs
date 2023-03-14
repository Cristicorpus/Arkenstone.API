using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arkenstone.Entities.DbSet
{
    [Table("Items")]
    public class Item
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public bool Published { get; set; }
        public string Name { get; set; }
        public int MarketGroupId { get; set; }
        public double? PriceBuy { get; set; }
        public double? PriceSell { get; set; }

    }
}
