using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Arkenstone.Entities.DbSet
{
    [Table("Recipes")]
    public class Recipe
    {
        public Recipe()
        {
            RecipeRessource = new HashSet<RecipeRessource>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public TypeRecipeEnum Type { get; set; }
        public int ItemId { get; set; }
        [ForeignKey("ItemId")]
        public virtual Item Item { get; set; }
        public int Quantity { get; set; }
        public int Time { get; set; }


        public virtual ICollection<RecipeRessource> RecipeRessource { get; set; }
    }
    public enum TypeRecipeEnum
    {
        Manufacturing = 1,
        ResearchTech = 2,
        ResearchTime = 3,
        ResearchMaterial = 4,
        Copying = 5,
        Duplicating = 6,
        Reverse = 7,
        Research = 8,
    }
}
