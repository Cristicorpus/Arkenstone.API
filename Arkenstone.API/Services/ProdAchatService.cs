using Arkenstone.API.Models;
using Arkenstone.Entities;
using Arkenstone.Entities.DbSet;
using Arkenstone.Logic.BusinessException;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Arkenstone.API.Services
{
    public class ProdAchatService
    {
        private ArkenstoneContext _context;

        public ProdAchatService(ArkenstoneContext context)
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

        public ProdAchatModel GetModel(long ProdAchatId)
        {
            var prodAchatReturned = Get(ProdAchatId);
            var returnvalue = new ProdAchatModel(prodAchatReturned);
            returnvalue.CostJob = CostJobFromProduction(prodAchatReturned.Location, prodAchatReturned.Item);
            returnvalue.CostProduction = CostProductionFromProduction(prodAchatReturned, true);

            if (prodAchatReturned.ProdAchatEnfants != null && prodAchatReturned.ProdAchatEnfants.Count > 0)
            {
                ItemService itemService = new ItemService(_context);
                foreach (var prodAchat in prodAchatReturned.ProdAchatEnfants)
                {
                    if (itemService.IsProductible(prodAchat.ItemId))
                    {
                        var childModel = returnvalue.ProdAchatEnfants.First(x => x.Id == prodAchat.Id);
                        childModel.CostJob = CostJobFromProduction(prodAchat.Location, prodAchat.Item);
                        childModel.CostProduction = CostProductionFromProduction(prodAchat, true);
                    }
                }
            }
            return returnvalue;
        }


        public ProdAchat Create(int corpId, ProdAchatModel prodAchatModel)
        {
            var prodAchatDb = new ProdAchat()
            {
                CorporationId = corpId,
                ItemId = prodAchatModel.Item.Id,
                Quantity = prodAchatModel.Quantity,
                Type = prodAchatModel.Type,
                MEefficiency = prodAchatModel.MEefficiency != null ? prodAchatModel.MEefficiency : null,
                LocationId = prodAchatModel.Location.Id,
                ProdAchatParentId = prodAchatModel.ProdAchatParent != null ? prodAchatModel.ProdAchatParent.Id : null,
                State = ProdAchatStateEnum.planifier
            };
            _context.ProdAchats.Add(prodAchatDb);
            return prodAchatDb;
        }
        public ProdAchat UpdateProdAchat(ProdAchat prodAchatDb, ProdAchatModel prodAchatModel)
        {
            prodAchatDb.Type = prodAchatModel.Type;
            prodAchatDb.Quantity = prodAchatModel.Quantity;
            prodAchatDb.MEefficiency = prodAchatModel.MEefficiency != null ? prodAchatModel.MEefficiency : null;
            prodAchatDb.LocationId = prodAchatModel.Location.Id;
            prodAchatDb.State = prodAchatModel.State;
            return prodAchatDb;
        }

        public void DeleteChilds(ProdAchat prodAchatParent)
        {
            if (prodAchatParent.ProdAchatEnfants != null)
            {
                foreach (var prodAchatChild in prodAchatParent.ProdAchatEnfants)
                    _context.ProdAchats.Remove(prodAchatChild);
            }
        }
        public void UpdateChilds(int corpId, ProdAchat prodAchatParent)
        {
            ItemService itemService = new ItemService(_context);
            var recipeItemProdAchats = itemService.GetRessourceFromRecipe(prodAchatParent.ItemId);

            foreach (var prodAchatChild in prodAchatParent.ProdAchatEnfants.Where(x => !recipeItemProdAchats.RecipeRessource.Any(y => y.ItemId == x.ItemId)))
                _context.ProdAchats.Remove(prodAchatChild);

            var parentMaterialEfficiency = GetEfficiency(prodAchatParent);
            foreach (var recipeRessource in recipeItemProdAchats.RecipeRessource)
            {
                var prodAchatChild = prodAchatParent.ProdAchatEnfants.FirstOrDefault(x => x.ItemId == recipeRessource.ItemId);
                if (prodAchatChild == null)
                    CreateChild(corpId, prodAchatParent, recipeRessource, recipeItemProdAchats, parentMaterialEfficiency);
                else
                {
                    if (prodAchatChild.State == ProdAchatStateEnum.planifier || prodAchatChild.State == ProdAchatStateEnum.reserver)
                        UpdateChild(prodAchatChild, recipeRessource, recipeItemProdAchats, parentMaterialEfficiency);
                }
            }
        }
        public void CreateChild(int corpId, ProdAchat prodAchatParent,RecipeRessource recipeRessource,Recipe recipe,decimal parentMaterialEfficiency)
        {
            var prodAchatDb = new ProdAchat()
            {
                CorporationId = corpId,
                ItemId = recipeRessource.ItemId,
                Quantity = CalculateQuantity(prodAchatParent, parentMaterialEfficiency, recipeRessource, recipe),
                MEefficiency = null,
                LocationId = prodAchatParent.LocationId,
                ProdAchatParentId = prodAchatParent.Id,
                State = ProdAchatStateEnum.planifier
            };
            _context.ProdAchats.Add(prodAchatDb);
        }
        public void UpdateChild(ProdAchat prodAchat, RecipeRessource recipeRessource, Recipe recipe, decimal parentMaterialEfficiency)
        {
            prodAchat.Quantity = CalculateQuantity(prodAchat.ProdAchatParent, parentMaterialEfficiency, recipeRessource, recipe);
        }
        
        private int CalculateQuantity(ProdAchat prodAchatParent, decimal efficiency ,RecipeRessource recipeRessource, Recipe recipe)
        {
            decimal quantityAfterEfficiency = recipeRessource.Quantity * efficiency;
            int global = (int)Math.Ceiling(quantityAfterEfficiency * Math.Ceiling((decimal)prodAchatParent.Quantity / (decimal)recipe.Quantity));
            return global;
        }
        private decimal GetEfficiency(ProdAchat prodAchatParent)
        {
            EfficiencyService efficiencyService = new EfficiencyService(_context);
            decimal globalEfficiency;

            if (prodAchatParent.Type == ProdAchatTypeEnum.achat)
                globalEfficiency = efficiencyService.GetBestEfficiencyFromLocation(prodAchatParent.CorporationId, prodAchatParent.ItemId);
            else
                globalEfficiency = efficiencyService.GetEfficiencyFromLocation(prodAchatParent.LocationId, prodAchatParent.ItemId);

            if (prodAchatParent.Type == ProdAchatTypeEnum.production && prodAchatParent.MEefficiency.HasValue)
                globalEfficiency = globalEfficiency * (1 - (prodAchatParent.MEefficiency.Value / 100));

            return globalEfficiency;
        }
        
        public decimal CostJobFromProduction(Location location, Item item)
        {
            var indexCostManufacture = _context.CostIndices.FirstOrDefault(x => x.SolarSystemId == location.SolarSystemId && x.type == CostIndiceType.manufacturing);
            if (indexCostManufacture == null)
                return 0;

            ItemService itemService = new ItemService(_context);
            var GlobalAdjustedPrice = itemService.GetRessourceFromRecipe(item.Id).RecipeRessource.Sum(x => x.Item.PriceAdjustedPrice * x.Quantity);

            var returnvalue = Math.Ceiling(indexCostManufacture.Cost * GlobalAdjustedPrice * 100) / 100;
            return returnvalue;
        }
        public decimal CostProductionFromProduction(ProdAchat prodAchat, bool seller = true)
        {
            decimal ProductionPrice = 0;
            if (prodAchat.Type == ProdAchatTypeEnum.achat)
            {
                ItemService itemService = new ItemService(_context);
                var parentMaterialEfficiency = GetEfficiency(prodAchat);
                var recipeItemProdAchats = itemService.GetRessourceFromRecipe(prodAchat.ItemId);
                foreach (var recipeRessource in recipeItemProdAchats.RecipeRessource)
                {
                    var quantity = CalculateQuantity(prodAchat, parentMaterialEfficiency, recipeRessource, recipeItemProdAchats);
                    if (seller)
                        ProductionPrice += itemService.Get(recipeRessource.ItemId).PriceSell * quantity;
                    else
                        ProductionPrice += itemService.Get(recipeRessource.ItemId).PriceBuy * quantity;
                }
            }
            else
            {
                foreach (var prodChild in prodAchat.ProdAchatEnfants)
                {
                    if (seller)
                        ProductionPrice += prodChild.Item.PriceSell * prodChild.Quantity;
                    else
                        ProductionPrice += prodChild.Item.PriceBuy * prodChild.Quantity;
                }
            }

            var returnvalue = Math.Ceiling(ProductionPrice * 100) / 100;
            return returnvalue;
        }

        public ProjectedStateChild GetProjectedStateChild(ProdAchat prodAchatDb, ProdAchatModel prodAchatModel)
        {
            var ProjectedStateChild = CompareModelWithDb_Type(prodAchatDb, prodAchatModel);

            if (ProjectedStateChild == ProjectedStateChild.none &&
                prodAchatModel.Type == ProdAchatTypeEnum.production)
            {
                var noUpdateChild = true;

                noUpdateChild &= CompareModelWithDb_Quantity(prodAchatDb, prodAchatModel);
                noUpdateChild &= CompareModelWithDb_Location(prodAchatDb, prodAchatModel);
                noUpdateChild &= CompareModelWithDb_ME(prodAchatDb, prodAchatModel);

                if (!noUpdateChild)
                    ProjectedStateChild = ProjectedStateChild.update;
            }

            return ProjectedStateChild;
        }

        public bool ValidateUpdateModel(int corpId, ProdAchatModel prodAchatModel, ProdAchat prodAchatDb=null)
        {
            if(prodAchatDb!=null && prodAchatModel.Id!= prodAchatDb.Id)
                throw new BadRequestException("prodAchatModel.Id and ID in query not match! what did you DOOOOOO!!!");

            if (prodAchatDb == null && prodAchatModel.Id > 0)
                throw new BadRequestException("its an creation... WHY DID YOU PUT AN ID IN THE MODEL?");

            if (prodAchatModel.Item == null || prodAchatModel.Item.Id == null)
                throw new BadRequestException("Item is null");


            ItemService itemService = new ItemService(_context);
            var item = itemService.GetFromRecipe(prodAchatModel.Item.Id);
            if (item == null)
                throw new BadRequestException("Item is not recognized");

            if (prodAchatModel.Location == null || prodAchatModel.Location.Id == null)
                throw new BadRequestException("Location is null");

            LocationService locationService = new LocationService(_context);
            var location = locationService.Get(prodAchatModel.Location.Id).ThrowNotAuthorized(corpId);

            if (prodAchatDb == null && prodAchatModel.ProdAchatParent != null)
                throw new BadRequestException("ProdAchatParent is not null");

            if (prodAchatModel.Quantity <= 0)
                throw new BadRequestException("Quantity is inferior or equal to 0");

            if (prodAchatDb == null && prodAchatModel.State != ProdAchatStateEnum.planifier)
                throw new BadRequestException("State is not correct. its must be planified(0)");

            switch (prodAchatModel.Type)
            {
                case Entities.DbSet.ProdAchatTypeEnum.achat:
                    if (prodAchatModel.MEefficiency.HasValue)
                        throw new BadRequestException("MEefficiency has value");
                    break;
                case Entities.DbSet.ProdAchatTypeEnum.production:
                    if (!prodAchatModel.MEefficiency.HasValue)
                        throw new BadRequestException("MEefficiency has not value");
                    break;
                default:
                    throw new BadRequestException("Type is not recognized");
            }

            return true;
        }
        public bool CompareModelWithDb_Item(ProdAchat prodAchatDb, ProdAchatModel prodAchatModel)
        {
            if (prodAchatDb.ItemId != prodAchatModel.Item.Id)
                throw new BadRequestException("prodAchatModel.Item.Id not match with prodachat ItemId in database! what did you DOOOOOO!!!");
            return true;
        }
        public bool CompareModelWithDb_Quantity(ProdAchat prodAchatDb, ProdAchatModel prodAchatModel)
        {
            if (prodAchatDb.ProdAchatParentId != null)
                throw new BadRequestException("you cant modify quantity of a child prodAchat");
            if (prodAchatDb.ProdAchatEnfants != null && prodAchatDb.ProdAchatEnfants.Any(x => x.State >= ProdAchatStateEnum.livraison))
                throw new BadRequestException("you cant modify quantity if a prodAchat child already produced");
            return prodAchatDb.Quantity != prodAchatModel.Quantity;
        }
        public bool CompareModelWithDb_ME(ProdAchat prodAchatDb, ProdAchatModel prodAchatModel)
        {
            if (prodAchatDb.ProdAchatEnfants != null && prodAchatDb.ProdAchatEnfants.Any(x => x.State >= ProdAchatStateEnum.livraison))
                throw new BadRequestException("you cant modify ME if a prodAchat child already produced");
            return prodAchatDb.MEefficiency != prodAchatModel.MEefficiency;
        }
        public bool CompareModelWithDb_Location(ProdAchat prodAchatDb, ProdAchatModel prodAchatModel)
        {
            if (prodAchatDb.ProdAchatEnfants != null && prodAchatDb.ProdAchatEnfants.Any(x => x.State >= ProdAchatStateEnum.livraison))
                throw new BadRequestException("you cant modify location if a prodAchat child already produced");
            return prodAchatDb.LocationId != prodAchatModel.Location.Id;
        }
        public ProjectedStateChild CompareModelWithDb_Type(ProdAchat prodAchatDb, ProdAchatModel prodAchatModel)
        {
            if (prodAchatDb.Type != prodAchatModel.Type)
            {
                if (prodAchatDb.Type == ProdAchatTypeEnum.production && prodAchatModel.Type == ProdAchatTypeEnum.achat)
                    return ProjectedStateChild.delete;
                else if (prodAchatDb.Type == ProdAchatTypeEnum.achat && prodAchatModel.Type == ProdAchatTypeEnum.production)
                    return ProjectedStateChild.create;
                else
                    throw new NotImplementedException();
            }
            else
                return ProjectedStateChild.none;
        }
    }
    public enum ProjectedStateChild
    {
        none,
        delete,
        create,
        update
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
