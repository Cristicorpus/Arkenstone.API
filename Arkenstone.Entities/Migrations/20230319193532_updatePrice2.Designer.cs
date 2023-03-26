﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Arkenstone.Entities.Migrations
{
    [DbContext(typeof(ArkenstoneContext))]
    [Migration("20230319193532_updatePrice2")]
    partial class updatePrice2
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("Arkenstone.Repositorie.DbSet.Alliance", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("Alliances");
                });

            modelBuilder.Entity("Arkenstone.Repositorie.DbSet.Character", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<int>("AllianceId")
                        .HasColumnType("int");

                    b.Property<int>("CharacterMainId")
                        .HasColumnType("int");

                    b.Property<int>("CorporationId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("RefreshToken")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Token")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.HasIndex("AllianceId");

                    b.HasIndex("CharacterMainId");

                    b.HasIndex("CorporationId");

                    b.ToTable("Characters");
                });

            modelBuilder.Entity("Arkenstone.Repositorie.DbSet.Corporation", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("Corporations");
                });

            modelBuilder.Entity("Arkenstone.Repositorie.DbSet.Inventory", b =>
                {
                    b.Property<int>("ItemId")
                        .HasColumnType("int")
                        .HasColumnOrder(0);

                    b.Property<long>("SubLocationId")
                        .HasColumnType("bigint")
                        .HasColumnOrder(1);

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.HasKey("ItemId", "SubLocationId");

                    b.HasIndex("SubLocationId");

                    b.ToTable("Inventorys");
                });

            modelBuilder.Entity("Arkenstone.Repositorie.DbSet.Item", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<int>("MarketGroupId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<decimal>("PriceAdjustedPrice")
                        .HasColumnType("decimal(65,30)");

                    b.Property<decimal>("PriceBuy")
                        .HasColumnType("decimal(65,30)");

                    b.Property<decimal>("PriceSell")
                        .HasColumnType("decimal(65,30)");

                    b.Property<bool>("Published")
                        .HasColumnType("tinyint(1)");

                    b.HasKey("Id");

                    b.ToTable("Items");
                });

            modelBuilder.Entity("Arkenstone.Repositorie.DbSet.Location", b =>
                {
                    b.Property<long>("Id")
                        .HasColumnType("bigint");

                    b.Property<bool>("CanCopy")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("CanInvent")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("CanProd")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("CanReact")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("CanReprocess")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("CanResearch")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<decimal>("Security")
                        .HasColumnType("decimal(65,30)");

                    b.Property<int?>("StructureTypeId")
                        .IsRequired()
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("StructureTypeId");

                    b.ToTable("Locations");
                });

            modelBuilder.Entity("Arkenstone.Repositorie.DbSet.LocationRigsManufacturing", b =>
                {
                    b.Property<long>("LocationId")
                        .HasColumnType("bigint")
                        .HasColumnOrder(0);

                    b.Property<int>("RigsManufacturingId")
                        .HasColumnType("int")
                        .HasColumnOrder(1);

                    b.HasKey("LocationId", "RigsManufacturingId");

                    b.HasIndex("RigsManufacturingId");

                    b.ToTable("LocationRigsManufacturings");
                });

            modelBuilder.Entity("Arkenstone.Repositorie.DbSet.MarketGroupTree", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnType("longtext");

                    b.Property<int?>("ParentId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("MarketGroupTrees");
                });

            modelBuilder.Entity("Arkenstone.Repositorie.DbSet.ProdAchat", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    b.Property<int?>("CharacterIdReservation")
                        .HasColumnType("int");

                    b.Property<int>("CorporationId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("DatetimeReservation")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("ItemId")
                        .HasColumnType("int");

                    b.Property<long>("LocationId")
                        .HasColumnType("bigint");

                    b.Property<decimal?>("MEefficiency")
                        .HasColumnType("decimal(65,30)");

                    b.Property<long?>("ProdAchatParentId")
                        .HasColumnType("bigint");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<int>("State")
                        .HasColumnType("int");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CorporationId");

                    b.HasIndex("ItemId");

                    b.HasIndex("LocationId");

                    b.HasIndex("ProdAchatParentId");

                    b.ToTable("ProdAchats");
                });

            modelBuilder.Entity("Arkenstone.Repositorie.DbSet.Recipe", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<int>("ItemId")
                        .HasColumnType("int");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<int>("Time")
                        .HasColumnType("int");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ItemId");

                    b.ToTable("Recipes");
                });

            modelBuilder.Entity("Arkenstone.Repositorie.DbSet.RecipeRessource", b =>
                {
                    b.Property<int>("RecipeId")
                        .HasColumnType("int")
                        .HasColumnOrder(1);

                    b.Property<int>("ItemId")
                        .HasColumnType("int")
                        .HasColumnOrder(0);

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.HasKey("RecipeId", "ItemId");

                    b.HasIndex("ItemId");

                    b.ToTable("RecipeRessources");
                });

            modelBuilder.Entity("Arkenstone.Repositorie.DbSet.RigsManufacturing", b =>
                {
                    b.Property<int>("ItemId")
                        .HasColumnType("int");

                    b.Property<decimal>("CostEffect")
                        .HasColumnType("decimal(65,30)");

                    b.Property<string>("MarketIdEffect")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("MarketIdNotEffect")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<decimal>("MaterialEffect")
                        .HasColumnType("decimal(65,30)");

                    b.Property<decimal>("MultiplierHS")
                        .HasColumnType("decimal(65,30)");

                    b.Property<decimal>("MultiplierLS")
                        .HasColumnType("decimal(65,30)");

                    b.Property<decimal>("MultiplierNS")
                        .HasColumnType("decimal(65,30)");

                    b.Property<decimal>("TimeEffect")
                        .HasColumnType("decimal(65,30)");

                    b.HasKey("ItemId");

                    b.ToTable("RigsManufacturings");
                });

            modelBuilder.Entity("Arkenstone.Repositorie.DbSet.StructureType", b =>
                {
                    b.Property<int>("ItemId")
                        .HasColumnType("int");

                    b.Property<decimal>("CostEffect")
                        .HasColumnType("decimal(65,30)");

                    b.Property<decimal>("MaterialEffect")
                        .HasColumnType("decimal(65,30)");

                    b.Property<decimal>("TimeEffect")
                        .HasColumnType("decimal(65,30)");

                    b.HasKey("ItemId");

                    b.ToTable("StructureTypes");
                });

            modelBuilder.Entity("Arkenstone.Repositorie.DbSet.SubLocation", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasColumnOrder(0);

                    b.Property<int>("CorporationId")
                        .HasColumnType("int");

                    b.Property<string>("Flag")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.Property<bool>("IsAssetAnalysed")
                        .HasColumnType("tinyint(1)");

                    b.Property<DateTime?>("LastUpdated")
                        .HasColumnType("datetime(6)");

                    b.Property<long>("LocationId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex(new[] { "LocationId", "Flag", "CorporationId" }, "IX_SubLocations_LocationId")
                        .IsUnique();

                    b.ToTable("SubLocations");
                });

            modelBuilder.Entity("Arkenstone.Repositorie.DbSet.Character", b =>
                {
                    b.HasOne("Arkenstone.Repositorie.DbSet.Alliance", "Alliance")
                        .WithMany()
                        .HasForeignKey("AllianceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Arkenstone.Repositorie.DbSet.Character", "CharacterMain")
                        .WithMany()
                        .HasForeignKey("CharacterMainId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Arkenstone.Repositorie.DbSet.Corporation", "Corporation")
                        .WithMany()
                        .HasForeignKey("CorporationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Alliance");

                    b.Navigation("CharacterMain");

                    b.Navigation("Corporation");
                });

            modelBuilder.Entity("Arkenstone.Repositorie.DbSet.Inventory", b =>
                {
                    b.HasOne("Arkenstone.Repositorie.DbSet.Item", "Item")
                        .WithMany()
                        .HasForeignKey("ItemId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Arkenstone.Repositorie.DbSet.SubLocation", "SubLocation")
                        .WithMany("Inventorys")
                        .HasForeignKey("SubLocationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Item");

                    b.Navigation("SubLocation");
                });

            modelBuilder.Entity("Arkenstone.Repositorie.DbSet.Location", b =>
                {
                    b.HasOne("Arkenstone.Repositorie.DbSet.StructureType", "StructureType")
                        .WithMany()
                        .HasForeignKey("StructureTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("StructureType");
                });

            modelBuilder.Entity("Arkenstone.Repositorie.DbSet.LocationRigsManufacturing", b =>
                {
                    b.HasOne("Arkenstone.Repositorie.DbSet.Location", "Location")
                        .WithMany("LocationRigsManufacturings")
                        .HasForeignKey("LocationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Arkenstone.Repositorie.DbSet.RigsManufacturing", "RigsManufacturing")
                        .WithMany()
                        .HasForeignKey("RigsManufacturingId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Location");

                    b.Navigation("RigsManufacturing");
                });

            modelBuilder.Entity("Arkenstone.Repositorie.DbSet.ProdAchat", b =>
                {
                    b.HasOne("Arkenstone.Repositorie.DbSet.Corporation", "Corporation")
                        .WithMany()
                        .HasForeignKey("CorporationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Arkenstone.Repositorie.DbSet.Item", "Item")
                        .WithMany()
                        .HasForeignKey("ItemId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Arkenstone.Repositorie.DbSet.Location", "Location")
                        .WithMany()
                        .HasForeignKey("LocationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Arkenstone.Repositorie.DbSet.ProdAchat", "ProdAchatParent")
                        .WithMany("ProdAchatEnfants")
                        .HasForeignKey("ProdAchatParentId");

                    b.Navigation("Corporation");

                    b.Navigation("Item");

                    b.Navigation("Location");

                    b.Navigation("ProdAchatParent");
                });

            modelBuilder.Entity("Arkenstone.Repositorie.DbSet.Recipe", b =>
                {
                    b.HasOne("Arkenstone.Repositorie.DbSet.Item", "Item")
                        .WithMany()
                        .HasForeignKey("ItemId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Item");
                });

            modelBuilder.Entity("Arkenstone.Repositorie.DbSet.RecipeRessource", b =>
                {
                    b.HasOne("Arkenstone.Repositorie.DbSet.Item", "Item")
                        .WithMany()
                        .HasForeignKey("ItemId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Arkenstone.Repositorie.DbSet.Recipe", "Recipe")
                        .WithMany("RecipeRessource")
                        .HasForeignKey("RecipeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Item");

                    b.Navigation("Recipe");
                });

            modelBuilder.Entity("Arkenstone.Repositorie.DbSet.RigsManufacturing", b =>
                {
                    b.HasOne("Arkenstone.Repositorie.DbSet.Item", "Item")
                        .WithMany()
                        .HasForeignKey("ItemId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Item");
                });

            modelBuilder.Entity("Arkenstone.Repositorie.DbSet.StructureType", b =>
                {
                    b.HasOne("Arkenstone.Repositorie.DbSet.Item", "Item")
                        .WithMany()
                        .HasForeignKey("ItemId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Item");
                });

            modelBuilder.Entity("Arkenstone.Repositorie.DbSet.SubLocation", b =>
                {
                    b.HasOne("Arkenstone.Repositorie.DbSet.Location", "Location")
                        .WithMany("SubLocations")
                        .HasForeignKey("LocationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Location");
                });

            modelBuilder.Entity("Arkenstone.Repositorie.DbSet.Location", b =>
                {
                    b.Navigation("LocationRigsManufacturings");

                    b.Navigation("SubLocations");
                });

            modelBuilder.Entity("Arkenstone.Repositorie.DbSet.ProdAchat", b =>
                {
                    b.Navigation("ProdAchatEnfants");
                });

            modelBuilder.Entity("Arkenstone.Repositorie.DbSet.Recipe", b =>
                {
                    b.Navigation("RecipeRessource");
                });

            modelBuilder.Entity("Arkenstone.Repositorie.DbSet.SubLocation", b =>
                {
                    b.Navigation("Inventorys");
                });
#pragma warning restore 612, 618
        }
    }
}
