using Arkenstone.Entities.DbSet;
using System.Collections.Generic;

namespace Arkenstone.API.ControllerModel
{
    public class EfficiencyModel
    {

        public decimal MEefficiency { get; set; }

        public StructureModel Station { get; set; }
        
        public List<RigsManufacturing> rigsEffect { get; set; }
    }
}
