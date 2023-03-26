using Arkenstone.Logic.BusinessException;
using Arkenstone.Entities;
using Arkenstone.Entities.DbSet;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Arkenstone.API.Services
{
    public class LocationService : BaseService
    {
        public LocationService(ArkenstoneContext context) : base(context)
        {

        }

        public Location Get(long LocationID)
        {
            return locationRepository.Get(LocationID);
        }
        public List<Location> GetList(int corpID)
        {
            return locationRepository.GetList(corpID);
        }


        public void SetFitToStructure(long LocationID, string CopyPastFit)
        {
            var location = Get(LocationID);
            locationRepository.UpdateFit(location, CopyPastFit);
        }

    }
    

}
