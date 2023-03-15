using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arkenstone.Entities.DbSet
{
    [Table("MarketGroupTrees")]
    public class MarketGroupTree
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public string? Name { get; set; }

    }
}
