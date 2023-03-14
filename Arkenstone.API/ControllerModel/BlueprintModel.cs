using Arkenstone.Entities.DbSet;

namespace Arkenstone.ControllerModel
{
    public class BlueprintModel
    {
        public long Id { get; set; }
        public decimal MaterialEfficiency { get; set; }
        public decimal TimeEfficiency { get; set; }
        public int CycleNumber { get; set; }
        public virtual Item Item { get; set; }
        public virtual Location Location { get; set; }

        public BlueprintModel(InventoryBlueprint blueprint)
        {
            Item = blueprint.Item;
            Location = blueprint.Location;
            Id = blueprint.Id;
            MaterialEfficiency = blueprint.MaterialEfficiency;
            TimeEfficiency = blueprint.TimeEfficiency;
            CycleNumber = blueprint.CycleNumber;
        }
    }
}
