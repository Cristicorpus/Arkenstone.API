using System.Collections.Generic;
using Arkenstone.Entities.DbSet;

namespace Arkenstone.API.Models
{
    public class RecipeRessourceModel
    {
        public RecipeRessourceModel()
        { }
        public RecipeRessourceModel(RecipeRessource Target)
        {
            RecipeId = Target.RecipeId;
            Item = Target.Item;
            Quantity = Target.Quantity;
        }

        public int RecipeId { get; set; }
        public Item Item { get; set; }
        public int Quantity { get; set; }
    }
 
}
