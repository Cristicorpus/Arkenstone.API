using Arkenstone.API.Models;
using Arkenstone.Entities;
using Arkenstone.Entities.DbSet;
using Arkenstone.Logic.BusinessException;
using Microsoft.EntityFrameworkCore;
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
                MEefficiency = prodAchatModel.MEefficiency != null ? prodAchatModel.MEefficiency : null,
                LocationId = prodAchatModel.Location.Id,
                ProdAchatParentId = prodAchatModel.ProdAchatParent != null ? prodAchatModel.ProdAchatParent.Id : null,
                State = ProdAchatStateEnum.planifier
            };
            _context.ProdAchats.Add(prodAchatDb);
            _context.SaveChanges();
            return prodAchatDb;
        }

        public void CreateAllChild(int corpId, ProdAchat prodAchatParent)
        {
            ItemService itemService = new ItemService(_context);
            var recipeItemProdAchat = itemService.GetRessourceFromRecipe(prodAchatParent.ItemId);

            foreach (var item in recipeItemProdAchat.RecipeRessource)
            {
                var prodAchatDb = new ProdAchat()
                {
                    CorporationId = corpId,
                    ItemId = item.ItemId,
                    Quantity = item.Quantity * prodAchatParent.Quantity,
                    MEefficiency = null,
                    LocationId = prodAchatParent.LocationId,
                    ProdAchatParentId = prodAchatParent.Id,
                    State = ProdAchatStateEnum.planifier
                };
                _context.ProdAchats.Add(prodAchatDb);
            }
            _context.SaveChanges();
        }

        public bool ValidateUpdateModel(int corpId, ProdAchatModel prodAchatModel, long? ProdAchatDbId)
        {
            if(ProdAchatDbId.HasValue && prodAchatModel.Id!= ProdAchatDbId)
                throw new BadRequestException("prodAchatModel.Id and ID in query not match! what did you DOOOOOO!!!");

            if (!ProdAchatDbId.HasValue && prodAchatModel.Id > 0)
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

            //on verifie que lors d une creation ProdAchatId==0 on ai pas de parent
            if (!ProdAchatDbId.HasValue && prodAchatModel.ProdAchatParent != null)
                throw new BadRequestException("ProdAchatParent is not null");

            if (prodAchatModel.Quantity <= 0)
                throw new BadRequestException("Quantity is inferior or equal to 0");

            if (!ProdAchatDbId.HasValue  && prodAchatModel.State != ProdAchatStateEnum.planifier)
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
        public bool CompareModelWithDb_Item(ProdAchat ProdAchatDb, ProdAchatModel prodAchatModel)
        {
            if (ProdAchatDb.ItemId != prodAchatModel.Item.Id)
                throw new BadRequestException("prodAchatModel.Item.Id not match with prodachat ItemId in database! what did you DOOOOOO!!!");
            return true;
        }
        public bool CompareModelWithDb_Quantity(ProdAchat ProdAchatDb, ProdAchatModel prodAchatModel)
        {
            return ProdAchatDb.Quantity != prodAchatModel.Quantity;
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
