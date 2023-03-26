using Arkenstone.Logic.Entities;
using Arkenstone.Logic.EsiEve;
using Arkenstone.Entities;
using Arkenstone.Entities.DbSet;
using System.Linq;

namespace Arkenstone.API.Services
{
    public class AllianceService : BaseService
    {
        public AllianceService(ArkenstoneContext context) : base(context)
        {

        }

        public Alliance GetOrCreate(int id)
        {
            return allianceRepository.GetAndCreate(id);
        }
 

    }
}
