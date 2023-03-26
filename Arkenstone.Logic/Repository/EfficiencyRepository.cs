using Arkenstone.Entities;
using Arkenstone.Entities.DbSet;
using Arkenstone.Logic.Entities;

namespace Arkenstone.Logic.Repository
{
    public class EfficiencyRepository
    {
        private ArkenstoneContext _context;

        public EfficiencyRepository(ArkenstoneContext context)
        {
            _context = context;
        }


        public decimal GetStationMaterialEfficiency(Location Location)
        {
            decimal MeEfficiency = 1;

            if (Location.StructureTypeId.HasValue)
            {
                var structureType = _context.StructureTypes.Find(Location.StructureTypeId);
                if (structureType != null)
                {
                    if (structureType.MaterialEffect == 0)
                        MeEfficiency = 1;
                    else
                        MeEfficiency = structureType.MaterialEffect;
                }
            }

            return MeEfficiency;
        }
        public EfficiencyEntitieRigsResult GetRigsMaterialEfficiency(Location Location, Item Item)
        {
            EfficiencyEntitieRigsResult returnValue = new EfficiencyEntitieRigsResult();

            if (Location.StructureType != null)
            {
                ItemRepository itemRepository = new ItemRepository(_context);
                var AllMarketGroupFromItem = itemRepository.GetAllGroupMarketIdFromItemId(Item.MarketGroupId);

                foreach (var rigItem in Location.LocationRigsManufacturings)
                {
                    if (!IsImpactedByRigs(AllMarketGroupFromItem, rigItem.RigsManufacturing))
                        continue;
                    decimal StatusMultiplier = 1;
                    if (Location.Security >= 5)
                        StatusMultiplier = rigItem.RigsManufacturing.MultiplierHS;
                    if (Location.Security < 5 && Location.Security > 0)
                        StatusMultiplier = rigItem.RigsManufacturing.MultiplierLS;
                    if (Location.Security <= 0)
                        StatusMultiplier = rigItem.RigsManufacturing.MultiplierNS;

                    returnValue.RigsMaterialEfficiency = returnValue.RigsMaterialEfficiency * ( 1 + ((rigItem.RigsManufacturing.MaterialEffect / 100) * StatusMultiplier));
                    returnValue.RigsEffect.Add(rigItem.RigsManufacturing);
                }
            }

            return returnValue;
        }
        private bool IsImpactedByRigs(List<int> MarketGroup, RigsManufacturing Rig)
        {
            var rigMarketGroupEffect = Rig.MarketIdEffect.Split('*');
            var rigMarketGroupIgnore = Rig.MarketIdNotEffect.Split('*');

            foreach (var rigMarketGroup in rigMarketGroupIgnore)
            {
                if (MarketGroup.Contains(int.Parse(rigMarketGroup)))
                    return false;
            }
            foreach (var rigMarketGroup in rigMarketGroupEffect)
            {
                if (MarketGroup.Contains(int.Parse(rigMarketGroup)))
                    return true;
            }
            return false;
        }

        public EfficiencyEntitieResult GetEfficiency(Location location, Item Item)
        {
            var StructureMaterialEfficiency = GetStationMaterialEfficiency(location);
            var RigsEfficiency = GetRigsMaterialEfficiency(location, Item);
            return new EfficiencyEntitieResult(location, StructureMaterialEfficiency, RigsEfficiency);
        }
    }
    public class EfficiencyEntitieResult
    {
        public Location Location { get; set; }
        public decimal StructureMaterialEfficiency;
        public EfficiencyEntitieRigsResult RigsEfficiency;

        public decimal GlobalMaterialEfficiency
        {
            get
            {
                return StructureMaterialEfficiency * RigsEfficiency.RigsMaterialEfficiency;
            }
        }
        
        public EfficiencyEntitieResult(Location location, decimal structureMaterialEfficiency, EfficiencyEntitieRigsResult rigsEfficiency)
        {
            this.Location = location;
            this.StructureMaterialEfficiency = structureMaterialEfficiency;
            this.RigsEfficiency = rigsEfficiency;
        }
    }
    public class EfficiencyEntitieRigsResult
    {
        public decimal RigsMaterialEfficiency;
        public List<RigsManufacturing> RigsEffect;
        public EfficiencyEntitieRigsResult()
        {
            this.RigsMaterialEfficiency = 1;
            this.RigsEffect = new List<RigsManufacturing>();
        }
    }
}
