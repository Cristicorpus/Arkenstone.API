using Arkenstone.API.Models;
using Arkenstone.Entities;
using Arkenstone.Entities.DbSet;
using Arkenstone.Logic.Asset;
using Arkenstone.Logic.BusinessException;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Arkenstone.API.Services
{
    public class ItemService
    {
        private ArkenstoneContext _context;

        public ItemService(ArkenstoneContext context)
        {
            _context = context;
        }
        private IQueryable<Item> GetCore()
        {
            return _context.Items.AsQueryable();
        }
        private IQueryable<Recipe> GetRecipeCore()
        {
            return _context.Recipes.Include("Item").Include("RecipeRessource.Item");
        }

        public List<Item> GetListRecipe()
        {
            var temp = GetRecipeCore().Select(x => x.Item).ToList();
            return temp;
        }
        public List<Item> GetList()
        {
            var temp = GetCore().ToList();
            return temp;
        }
        public Item GetFromRecipe(int itemId)
        {
            var temp = GetRecipeCore().FirstOrDefault(x => x.ItemId == itemId);
            if (temp==null)
                throw new NotFound("Recipe");
            return temp.Item;
        }
        public Item Get(int itemId)
        {
            var temp = GetCore().FirstOrDefault(x => x.Id == itemId);
            if (temp == null)
                throw new NotFound("Item");
            return temp;
        }
        public Recipe GetRessourceFromRecipe(int itemId)
        {
            var temp = GetRecipeCore().FirstOrDefault(x => x.ItemId == itemId);
            if (temp == null)
                throw new NotFound("Recipe");
            return temp;
        }
        public bool IsProductible(int itemId)
        {
            try
            {
                GetRessourceFromRecipe(itemId);
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }
    }
}
