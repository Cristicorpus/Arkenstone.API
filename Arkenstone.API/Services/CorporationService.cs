using Arkenstone.Entities;
using Arkenstone.Entities.DbSet;

namespace Arkenstone.API.Services
{
    public class CorporationService : BaseService
    {
        public CorporationService(ArkenstoneContext context) : base(context)
        {

        }
        public Corporation GetOrCreate(int id)
        {
            return corporationRepository.GetAndCreate(id);
        }

    }
}
