using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arkenstone.Entities.DbSet
{
    [Table("StructureTypes")]
    public class StructureType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        
        public string Name { get; set; }
        public decimal CostEffect { get; set; } = 0;
        public decimal TimeEffect { get; set; } = 0;
        public decimal MaterialEffect { get; set; } = 0;
    }
}
