using Arkenstone.Logic.BusinessException;
using Arkenstone.Logic.Logs;
using Arkenstone.Entities;
using Arkenstone.Entities.DbSet;
using Microsoft.EntityFrameworkCore;

namespace Arkenstone.Logic.Entities
{
    public class ItemRepository
    {
        private ArkenstoneContext _context;

        public ItemRepository(ArkenstoneContext context)
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

        public Item Get(int itemId)
        {
            var temp = GetCore().FirstOrDefault(x => x.Id == itemId);
            if (temp == null)
                throw new NotFound("Item");
            return temp;
        }
        public List<Item> GetList()
        {
            var temp = GetCore().ToList();
            return temp;
        }
        
        public Item GetProductible(int itemId)
        {
            var temp = GetRecipeCore().FirstOrDefault(x => x.ItemId == itemId);
            if (temp == null)
                throw new NotFound("Recipe");
            return temp.Item;
        }
        public List<Item> GetListProductible()
        {
            var temp = GetRecipeCore().Select(x => x.Item).ToList();
            return temp;
        }
        public Recipe GetRecipe(int itemId)
        {
            var temp = GetRecipeCore().FirstOrDefault(x => x.ItemId == itemId);
            if (temp == null)
                throw new NotFound("Recipe");
            return temp;
        }

        public List<int> GetAllGroupMarketIdFromItemId(int ItemId)
        {
            var returnList = new List<int>();
            int? parentID = ItemId;
            do
            {
                var marketGroup = _context.MarketGroupTrees.Find(parentID);
                if (marketGroup != null)
                {
                    returnList.Add(marketGroup.Id);
                    parentID = marketGroup.ParentId;
                }
                else
                    parentID = null;

            } while (parentID.HasValue);

            return returnList;
        }


        public List<Item> GetAllItemWithPriceUpdatable()
        {
            List<int> ListItemRecipeRessource = _context.RecipeRessources.Select(x => x.ItemId).Distinct().ToList();
            List<int> ListItemRecipe = _context.Recipes.Select(x => x.ItemId).Distinct().ToList();

            List<int> allItemId = new List<int>();
            allItemId.AddRange(ListItemRecipeRessource);
            allItemId.AddRange(ListItemRecipe);
            allItemId = allItemId.Distinct().ToList();

            return _context.Items.Where(x => allItemId.Contains(x.Id)).ToList();
        }

        
    }
}
