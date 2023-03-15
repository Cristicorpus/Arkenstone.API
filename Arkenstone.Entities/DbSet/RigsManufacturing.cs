using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arkenstone.Entities.DbSet
{
    [Table("RigsManufacturings")]
    public class RigsManufacturing
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string MarketIdEffect { get; set; }

        public decimal CostEffect { get; set; } = 0;
        public decimal TimeEffect { get; set; } = 0;
        public decimal MaterialEffect { get; set; } = 0;
        public decimal MultiplierHS { get; set; } = 1;
        public decimal MultiplierLS { get; set; } = 1;
        public decimal MultiplierNS { get; set; } = 1;
    }
}
