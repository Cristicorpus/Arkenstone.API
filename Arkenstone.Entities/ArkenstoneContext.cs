using Arkenstone.Entities.DbSet;
using Microsoft.EntityFrameworkCore;
#nullable disable


namespace Arkenstone.Entities
{
    // https://docs.microsoft.com/fr-fr/aspnet/core/data/ef-mvc/migrations?view=aspnetcore-5.0
    // SET YOUR ENV VAR BEFORE MIGRATION
    //$env:DB_DATA_connectionstring = 'yourFUUUUUUUUU_ing_connectionstring'//
    // dotnet ef migrations add "InitialCreate" --project Arkenstone.Entities --startup-project Arkenstone.API

    public partial class ArkenstoneContext : DbContext
    {
        public ArkenstoneContext(DbContextOptions options)
            : base(options)
        {
        }

        public virtual DbSet<Character> Characters { get; set; }
        public virtual DbSet<Inventory> Inventorys { get; set; }
        public virtual DbSet<InventoryBlueprint> InventoryBlueprints { get; set; }
        public virtual DbSet<Item> Items { get; set; }
        public virtual DbSet<RigsManufacturing> RigsManufacturings { get; set; }
        public virtual DbSet<StructureType> StructureTypes { get; set; }
        public virtual DbSet<Location> Locations { get; set; }
        public virtual DbSet<LocationRigsManufacturing> LocationRigsManufacturings { get; set; }
        public virtual DbSet<SubLocation> SubLocations { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<Recipe> Recipes { get; set; }
        public virtual DbSet<RecipeRessource> RecipeRessources { get; set; }
        public virtual DbSet<Ticket> Tickets { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RecipeRessource>().HasKey(x => new { 
                x.RecipeId, 
                x.ItemId 
            });
            modelBuilder.Entity<Inventory>().HasKey(x => new
            {
                x.ItemId,
                x.SubLocationId
            });
            modelBuilder.Entity<LocationRigsManufacturing>().HasKey(x => new
            {
                x.LocationId,
                x.RigsManufacturingId
            });
        }

    }

}
