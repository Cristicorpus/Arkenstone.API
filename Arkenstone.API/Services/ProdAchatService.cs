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

            foreach (var recipeRessource in recipeItemProdAchats.RecipeRessource)
            {
                var prodAchatChild = prodAchatParent.ProdAchatEnfants.FirstOrDefault(x => x.ItemId == recipeRessource.ItemId);
                if (prodAchatChild == null)
                    CreateChild(corpId, prodAchatParent, recipeRessource);
                else
                {
                    if (prodAchatChild.State == ProdAchatStateEnum.planifier || prodAchatChild.State == ProdAchatStateEnum.reserver)
                        UpdateChild(prodAchatChild, recipeRessource);
                }
            }
        }
        public void CreateChild(int corpId, ProdAchat prodAchatParent,RecipeRessource recipeRessource)
        {
            var prodAchatDb = new ProdAchat()
            {
                CorporationId = corpId,
                ItemId = recipeRessource.ItemId,
                Quantity = CalculateQuantityAfterEfficiency(prodAchatParent, recipeRessource),
                MEefficiency = null,
                LocationId = prodAchatParent.LocationId,
                ProdAchatParentId = prodAchatParent.Id,
                State = ProdAchatStateEnum.planifier
            };
            _context.ProdAchats.Add(prodAchatDb);
        }
        public void UpdateChild(ProdAchat prodAchat, RecipeRessource recipeRessource)
        {
            prodAchat.Quantity = CalculateQuantityAfterEfficiency(prodAchat.ProdAchatParent, recipeRessource);
        }
        private int CalculateQuantityAfterEfficiency(ProdAchat prodAchatParent, RecipeRessource recipeRessource)
        {
            EfficiencyService efficiencyService = new EfficiencyService(_context);
            var efficiencyParent = efficiencyService.GetEfficiencyFromLocation(prodAchatParent.LocationId, prodAchatParent.ItemId);
            decimal globalEfficiency = efficiencyParent * (1-(prodAchatParent.MEefficiency.Value / 100));
            decimal quantityAfterEfficiency = recipeRessource.Quantity * globalEfficiency;

            int global = (int)Math.Ceiling(quantityAfterEfficiency * prodAchatParent.Quantity);

            return global;
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
                throw new BadRequestException("you cant modify quantiry of a child prodAchat");
            return prodAchatDb.Quantity != prodAchatModel.Quantity;
        }
        public bool CompareModelWithDb_ME(ProdAchat prodAchatDb, ProdAchatModel prodAchatModel)
        {
            return prodAchatDb.MEefficiency != prodAchatModel.MEefficiency;
        }
        public bool CompareModelWithDb_Location(ProdAchat prodAchatDb, ProdAchatModel prodAchatModel)
        {
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
