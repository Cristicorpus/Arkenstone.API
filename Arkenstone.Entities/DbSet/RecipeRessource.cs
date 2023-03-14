using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arkenstone.Entities.DbSet
{
    [Table("RecipeRessources")]
    public class RecipeRessource
    {
        public RecipeRessource()
        {
        }
        [Key]
        [Column(Order = 0)]
        public int ItemId { get; set; }
        public virtual Item Item { get; set; }
        [Key]
        [Column(Order = 1)]
        public int RecipeId { get; set; }
        public virtual Recipe Recipe { get; set; }
        public int Quantity { get; set; }
    }
}
