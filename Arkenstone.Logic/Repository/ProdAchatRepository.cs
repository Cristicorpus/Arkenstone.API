using Arkenstone.Entities;
using Arkenstone.Entities.DbSet;
using Arkenstone.Logic.BusinessException;
using Arkenstone.Logic.Entities;
using Microsoft.EntityFrameworkCore;

namespace Arkenstone.Logic.Repository
{
    public class ProdAchatRepository
    {
        private ArkenstoneContext _context;

        public ProdAchatRepository(ArkenstoneContext context)
        {
            _context = context;
        }
        private IQueryable<ProdAchat> GetCore()
        {
            return _context.ProdAchats.Include("Item").Include("Location.StructureType.Item").Include("ProdAchatParent.Item").Include("ProdAchatEnfants.Item");
        }

        public ProdAchat Get(long id)
        {
            var temp = GetCore().FirstOrDefault(x => x.Id == id);
            if (temp == null)
                throw new NotFound("ProdAchat");
            return temp;
        }
        public List<ProdAchat> GetList(int CorpId)
        {
            var temp = GetCore().Where(x => x.ProdAchatParentId == null && x.State != ProdAchatStateEnum.terminer).ToList();
            return temp;
        }

        public ProdAchat Create(int corpId, int itemId, long quantity, 
            ProdAchatTypeEnum type, decimal? MEefficiency, 
            long locationId, long? prodAchatParentId)
        {
            var prodAchat = new ProdAchat()
            {
                CorporationId = corpId,
                ItemId = itemId,
                Quantity = quantity,
                Type = type,
                MEefficiency = MEefficiency != null ? MEefficiency : null,
                LocationId = locationId,
                ProdAchatParentId = prodAchatParentId,
                State = ProdAchatStateEnum.planifier
            };
            _context.ProdAchats.Add(prodAchat);
            _context.SaveChanges();
            
            if (type == ProdAchatTypeEnum.production)
                UpdateChilds(prodAchat);
            _context.SaveChanges();
            
            return prodAchat;
        }

        public ProdAchat Update(long id, long quantity, 
            ProdAchatTypeEnum type, decimal? MEefficiency, 
            long locationId, ProdAchatStateEnum state)
        {
            var prodAchat = Get(id);
            prodAchat.Type = type;
            prodAchat.Quantity = quantity;
            prodAchat.MEefficiency = MEefficiency != null ? MEefficiency : null;
            prodAchat.LocationId = locationId;
            prodAchat.State = state;
            _context.SaveChanges();
            if (type == ProdAchatTypeEnum.production)
                UpdateChilds(prodAchat);
            if (type == ProdAchatTypeEnum.achat)
                DeleteChilds(prodAchat);
            return prodAchat;
        }

        public void DeleteChilds(ProdAchat prodAchatParent)
        {
            if (prodAchatParent.ProdAchatEnfants != null)
            {
                foreach (var prodAchatChild in prodAchatParent.ProdAchatEnfants)
                    _context.ProdAchats.Remove(prodAchatChild);
                _context.SaveChanges();
            }
        }
        public void UpdateChilds(ProdAchat prodAchatParent)
        {
            ItemRepository itemRepository = new ItemRepository(_context);
            var recipeItemProdAchats = itemRepository.GetRecipe(prodAchatParent.ItemId);

            foreach (var prodAchatChild in prodAchatParent.ProdAchatEnfants.Where(x => !recipeItemProdAchats.RecipeRessource.Any(y => y.ItemId == x.ItemId)))
                _context.ProdAchats.Remove(prodAchatChild);

            foreach (var recipeRessource in recipeItemProdAchats.RecipeRessource)
            {
                var prodAchatChild = prodAchatParent.ProdAchatEnfants.FirstOrDefault(x => x.ItemId == recipeRessource.ItemId);
                var quantity = CalculateQuantityAfterEfficiency(prodAchatParent, recipeRessource, recipeItemProdAchats);

                if (prodAchatChild == null)
                    Create(prodAchatParent.CorporationId, recipeRessource.ItemId, quantity, ProdAchatTypeEnum.achat, null, prodAchatParent.LocationId, prodAchatParent.Id);

                if (prodAchatChild != null && (prodAchatChild.State == ProdAchatStateEnum.planifier || prodAchatChild.State == ProdAchatStateEnum.reserver))
                    Update(prodAchatChild.Id, quantity, prodAchatChild.Type, prodAchatChild.MEefficiency, prodAchatChild.LocationId, prodAchatChild.State);
            }
            _context.SaveChanges();
        }

        private int CalculateQuantityAfterEfficiency(ProdAchat prodAchatParent, RecipeRessource recipeRessource, Recipe recipe)
        {
            EfficiencyRepository efficiencyRepository = new EfficiencyRepository(_context);
            ItemRepository itemRepository = new ItemRepository(_context);
            LocationRepository locationRepository = new LocationRepository(_context);
            decimal globalEfficiency = 1;

            var efficiencyParent = efficiencyRepository.GetEfficiency(locationRepository.Get(prodAchatParent.LocationId), itemRepository.Get(prodAchatParent.ItemId));
            globalEfficiency = efficiencyParent.GlobalMaterialEfficiency;
            
            if (prodAchatParent.MEefficiency.HasValue)
                globalEfficiency = globalEfficiency * (1-(prodAchatParent.MEefficiency.Value / 100));
            
            decimal quantityAfterEfficiency = recipeRessource.Quantity * globalEfficiency;
            int global = (int)Math.Ceiling(quantityAfterEfficiency * Math.Ceiling((decimal)prodAchatParent.Quantity / (decimal)recipe.Quantity));
            return global;
        }

        public decimal CostJobFromProduction(Location location, Item item)
        {
            var indexCostManufacture = _context.CostIndices.FirstOrDefault(x => x.SolarSystemId == location.SolarSystemId && x.type == CostIndiceType.manufacturing);
            if (indexCostManufacture == null)
                return 0;

            ItemRepository itemRepository = new ItemRepository(_context);
            var GlobalAdjustedPrice = itemRepository.GetRecipe(item.Id).RecipeRessource.Sum(x => x.Item.PriceAdjustedPrice * x.Quantity);

            var returnvalue = Math.Ceiling(indexCostManufacture.Cost * GlobalAdjustedPrice * 100) / 100;
            return returnvalue;
        }
        
        public decimal CostProductionFromProduction(ProdAchat prodAchat, bool seller = true)
        {
            ItemRepository itemRepository = new ItemRepository(_context);
            var recipeItemProdAchats = itemRepository.GetRecipe(prodAchat.ItemId);
            decimal ProductionPrice = 0;


            foreach (var recipeRessource in recipeItemProdAchats.RecipeRessource)
            {
                var quantity = CalculateQuantityAfterEfficiency(prodAchat, recipeRessource, recipeItemProdAchats);
                if (seller)
                    ProductionPrice += itemRepository.Get(recipeRessource.ItemId).PriceSell* quantity;
                else
                    ProductionPrice += itemRepository.Get(recipeRessource.ItemId).PriceBuy * quantity;
            }
            var returnvalue = Math.Ceiling(ProductionPrice * 100) / 100;
            return returnvalue;
        }

    }
    public static class ProdAchatExtension
    {
        public static ProdAchat ThrowNotAuthorized(this ProdAchat prodAchat, int corpId)
        {
            if (prodAchat.CorporationId != corpId)
                throw new NotAuthorized();
            return prodAchat;
        }
    }
}
