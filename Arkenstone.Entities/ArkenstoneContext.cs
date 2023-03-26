using Arkenstone.Entities.DbSet;
using Microsoft.EntityFrameworkCore;
#nullable disable


namespace Arkenstone.Entities
{
    // https://docs.microsoft.com/fr-fr/aspnet/core/data/ef-mvc/migrations?view=aspnetcore-5.0
    // SET YOUR ENV VAR BEFORE MIGRATION
    //$env:DB_DATA_connectionstring = 'yourFUUUUUUUUU_ing_connectionstring'//
    // dotnet ef migrations add "InitialCreate" --project Arkenstone.Repositorie --startup-project Arkenstone.API

    public partial class ArkenstoneContext : DbContext
    {
        public ArkenstoneContext(DbContextOptions options)
            : base(options)
        {
        }
        
        public static ArkenstoneContext GetContextWithDefaultOption()
        {
            var _dbConnectionString = System.Environment.GetEnvironmentVariable("DB_DATA_connectionstring");
            var options = new DbContextOptionsBuilder<ArkenstoneContext>().UseMySql(_dbConnectionString, ServerVersion.AutoDetect(_dbConnectionString)).Options;
            return new ArkenstoneContext(options);
        }

        public virtual DbSet<MarketGroupTree> MarketGroupTrees { get; set; }
        public virtual DbSet<CostIndice> CostIndices { get; set; }
        public virtual DbSet<Alliance> Alliances { get; set; }
        public virtual DbSet<Corporation> Corporations { get; set; }
        public virtual DbSet<Character> Characters { get; set; }
        public virtual DbSet<Inventory> Inventorys { get; set; }
        public virtual DbSet<Item> Items { get; set; }
        public virtual DbSet<RigsManufacturing> RigsManufacturings { get; set; }
        public virtual DbSet<StructureType> StructureTypes { get; set; }
        public virtual DbSet<Location> Locations { get; set; }
        public virtual DbSet<LocationRigsManufacturing> LocationRigsManufacturings { get; set; }
        public virtual DbSet<SubLocation> SubLocations { get; set; }
        public virtual DbSet<Recipe> Recipes { get; set; }
        public virtual DbSet<RecipeRessource> RecipeRessources { get; set; }
        public virtual DbSet<ProdAchat> ProdAchats { get; set; }


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
            modelBuilder.Entity<CostIndice>().HasKey(x => new {
                x.SolarSystemId,
                x.type
            });
        }

    }

}
