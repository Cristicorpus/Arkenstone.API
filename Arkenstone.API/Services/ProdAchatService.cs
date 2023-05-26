using Arkenstone.API.Models;
using Arkenstone.Entities;
using Arkenstone.Entities.DbSet;
using Arkenstone.Logic.BusinessException;
using Arkenstone.Logic.Repository;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Arkenstone.API.Services
{
    public class ProdAchatService : BaseService
    {
        public ProdAchatService(ArkenstoneContext context) : base(context)
        {

        }
        private IQueryable<ProdAchat> GetCore()
        {
            return _context.ProdAchats.Include("Item").Include("Location.StructureType.Item").Include("ProdAchatParent.Item").Include("ProdAchatEnfants.Item");
        }

        public ProdAchat Get(long id)
        {
            return prodAchatRepository.Get(id);
        }
        public List<ProdAchat> GetList(int CorpId)
        {
            return prodAchatRepository.GetList(CorpId);
        }

        public ProdAchatModel GetModel(long ProdAchatId)
        {
            var prodAchatReturned = prodAchatRepository.Get(ProdAchatId);
            
            var returnvalue = new ProdAchatModel(prodAchatReturned);
            returnvalue.CostJob = prodAchatRepository.CostJobFromProduction(prodAchatReturned.Location, prodAchatReturned.Item);
            returnvalue.CostProduction = prodAchatRepository.CostProductionFromProduction(prodAchatReturned, true);
            return returnvalue;
        }

        public ProdAchat Create(int corpId, ProdAchatModel prodAchatModel)
        {
            int ItemId = prodAchatModel.Item.Id;
            long locationId = prodAchatModel.Location.Id;
            long? prodAchatParentId = null;
            if (prodAchatModel.ProdAchatParent != null)
                prodAchatParentId = prodAchatModel.ProdAchatParent.Id;

            return prodAchatRepository.Create(corpId, ItemId, prodAchatModel.Quantity,
                prodAchatModel.Type, prodAchatModel.MEefficiency,
                locationId, prodAchatParentId) ;
        }
        public ProdAchat UpdateProdAchat(ProdAchat prodAchatDb, ProdAchatModel prodAchatModel)
        {
            var prodAchatId = prodAchatDb.Id;
            long locationId = prodAchatModel.Location.Id;

            return prodAchatRepository.Update(prodAchatId, prodAchatModel.Quantity,
                prodAchatModel.Type, prodAchatModel.MEefficiency,
                locationId, prodAchatModel.State);
        }


        public bool ValidateCoreModel(int corpId, ProdAchatModel prodAchatModel)
        {
            if (prodAchatModel.Item == null || prodAchatModel.Item.Id == null)
                throw new BadRequestException("Item is null");

            var item = itemRepository.GetProductible(prodAchatModel.Item.Id);
            if (item == null)
                throw new BadRequestException("Item is not recognized");

            if (prodAchatModel.Location == null || prodAchatModel.Location.Id == null)
                throw new BadRequestException("Location is null");
            
            var location = locationRepository.Get(prodAchatModel.Location.Id).ThrowNotAuthorized(corpId);


            if (prodAchatModel.Quantity <= 0)
                throw new BadRequestException("Quantity is inferior or equal to 0");

            switch (prodAchatModel.Type)
            {
                case ProdAchatTypeEnum.achat:
                    if (prodAchatModel.MEefficiency.HasValue)
                        throw new BadRequestException("MEefficiency has value");
                    break;
                case ProdAchatTypeEnum.production:
                    if (!prodAchatModel.MEefficiency.HasValue)
                        throw new BadRequestException("MEefficiency has not value");
                    break;
                default:
                    throw new BadRequestException("Type is not recognized");
            }

            return true;
        }

        public bool ValidateCreateModel(int corpId, ProdAchatModel prodAchatModel, ProdAchat prodAchatDb)
        {
            ValidateCoreModel(corpId, prodAchatModel);

            if (prodAchatModel.Id != 0)
                throw new BadRequestException("prodAchatModel.Id useless. its an creation... WHY DID YOU PUT AN ID IN THE MODEL?");

            if (prodAchatModel.Id > 0)
                throw new BadRequestException("its an creation... WHY DID YOU PUT AN ID IN THE MODEL?");

            if (prodAchatModel.ProdAchatParent != null)
                throw new BadRequestException("ProdAchatParent is not null");

            if (prodAchatModel.State != ProdAchatStateEnum.planifier)
                throw new BadRequestException("State is not correct. its must be planified(0)");

            return true;
        }
        
        public bool ValidateUpdateModel(int corpId, ProdAchatModel prodAchatModel, ProdAchat prodAchatDb)
        {
            ValidateCoreModel(corpId, prodAchatModel);
            
            if (prodAchatDb.ItemId != prodAchatModel.Item.Id)
                throw new BadRequestException("prodAchatModel.Item.Id not match with prodachat ItemId in database! what did you DOOOOOO!!!");


            if (prodAchatDb.ProdAchatParentId != null)
                throw new BadRequestException("you cant modify quantity of a child prodAchat");
            if (prodAchatDb.ProdAchatEnfants != null && prodAchatDb.ProdAchatEnfants.Any(x => x.State >= ProdAchatStateEnum.livraison))
                throw new BadRequestException("you cant modify quantity of a prodAchat child already produced");

            
            return true;
        }

    }
}
