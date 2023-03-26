using Arkenstone.API.Models;
using Arkenstone.Entities;
using Arkenstone.Entities.DbSet;

namespace Arkenstone.API.Services
{
    public class EfficiencyService : BaseService
    {
        public EfficiencyService(ArkenstoneContext context) : base(context)
        {

        }

        public EfficiencyModel GetEfficiencyModelFromLocation(Location location, Item Item)
        {

            var returnModel = new EfficiencyModel();

            var StructureEfficiency = efficiencyRepository.GetEfficiency(location, Item);
            
            returnModel.MEefficiency = StructureEfficiency.GlobalMaterialEfficiency;
            returnModel.Station = new LocationModel(StructureEfficiency.Location);
            returnModel.rigsEffect = StructureEfficiency.RigsEfficiency.RigsEffect;
            
            return returnModel;
        }
        

    }
}
