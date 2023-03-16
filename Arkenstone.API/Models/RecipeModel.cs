using System;
using System.Collections.Generic;
using System.Linq;
using Arkenstone.Entities.DbSet;

namespace Arkenstone.API.Models
{
    public class RecipeModel
    {
        public RecipeModel()
        {

        }

        public RecipeModel(Recipe Target)
        {
            Id = Target.Id;
            Type = Target.Type;
            Item = Target.Item;
            Quantity = Target.Quantity;
            Time = Target.Time;

            if(Target.RecipeRessource!=null)
                RecipeRessource = Target.RecipeRessource.Select(x => new RecipeRessourceModel(x));
        }

        public int Id { get; set; }
        public TypeRecipeEnum Type { get; set; }
        public int Quantity { get; set; }
        public int Time { get; set; }
        public Item Item { get; set; }

        public IEnumerable<RecipeRessourceModel> RecipeRessource { get; set; }
    }
}
