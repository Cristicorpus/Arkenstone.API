using Arkenstone.Entities;
using Arkenstone.Entities.DbSet;
using System.Collections.Generic;

namespace Arkenstone.API.Services
{
    public class ItemService : BaseService
    {
        public ItemService(ArkenstoneContext context) : base(context)
        {

        }

        public Item Get(int itemId)
        {
            return itemRepository.Get(itemId);
        }
        public List<Item> GetList()
        {
            return itemRepository.GetList();
        }

        public Item GetProductible(int itemId)
        {
            return itemRepository.GetProductible(itemId);
        }
        public List<Item> GetListProductible()
        {
            return itemRepository.GetListProductible();
        }
        public Recipe GetRecipe(int itemId)
        {
            return itemRepository.GetRecipe(itemId);
        }






    }
}
