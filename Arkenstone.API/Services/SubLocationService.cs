using Arkenstone.Logic.BusinessException;
using Arkenstone.Entities;
using Arkenstone.Entities.DbSet;
using System.Collections.Generic;
using System.Linq;

namespace Arkenstone.API.Services
{
    public class SubLocationService : BaseService
    {
        public SubLocationService(ArkenstoneContext context) : base(context)
        {

        }
        
        public List<SubLocation> GetList(int corpID)
        {
            return subLocationRepository.GetList(corpID);
        }
        public List<SubLocation> GetListFromLocation(int corpID,long locationId)
        {
            return subLocationRepository.GetListFromLocation(corpID, locationId);
        }
        public SubLocation Get(long subLocationID)
        {
            return subLocationRepository.Get(subLocationID);
        }

        public void EditSubLocation(long Sublocation,bool toAnalyse)
        {
            subLocationRepository.EditSubLocation(Sublocation, toAnalyse);
        }




    }
}
