﻿// <auto-generated />
using System;
using Arkenstone.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Arkenstone.Entities.Migrations
{
    [DbContext(typeof(ArkenstoneContext))]
    [Migration("20230314192429_Initial1")]
    partial class Initial1
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.15")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("Arkenstone.Entities.DbSet.Character", b =>
                {
                    b.Property<int>("Id")
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

                    b.ToTable("Characters");
                });

            modelBuilder.Entity("Arkenstone.Entities.DbSet.Inventory", b =>
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

            modelBuilder.Entity("Arkenstone.Entities.DbSet.InventoryBlueprint", b =>
                {
                    b.Property<long>("Id")
                        .HasColumnType("bigint");

                    b.Property<int>("CycleNumber")
                        .HasColumnType("int");

                    b.Property<int>("ItemId")
                        .HasColumnType("int");

                    b.Property<long>("LocationId")
                        .HasColumnType("bigint");

                    b.Property<decimal>("MaterialEfficiency")
                        .HasColumnType("decimal(65,30)");

                    b.Property<decimal>("TimeEfficiency")
                        .HasColumnType("decimal(65,30)");

                    b.HasKey("Id");

                    b.HasIndex("ItemId");

                    b.HasIndex("LocationId");

                    b.ToTable("InventoryBlueprints");
                });

            modelBuilder.Entity("Arkenstone.Entities.DbSet.Item", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<int>("MarketGroupId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<double?>("PriceBuy")
                        .HasColumnType("double");

                    b.Property<double?>("PriceSell")
                        .HasColumnType("double");

                    b.Property<bool>("Published")
                        .HasColumnType("tinyint(1)");

                    b.HasKey("Id");

                    b.ToTable("Items");
                });

            modelBuilder.Entity("Arkenstone.Entities.DbSet.Location", b =>
                {
                    b.Property<long>("Id")
                        .HasColumnType("bigint");

                    b.Property<bool>("CanProd")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("CanReact")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("CanReprocess")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<decimal>("Security")
                        .HasColumnType("decimal(65,30)");

                    b.HasKey("Id");

                    b.ToTable("Locations");
                });

            modelBuilder.Entity("Arkenstone.Entities.DbSet.Order", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("CharacterId")
                        .HasColumnType("int");

                    b.Property<int>("LocationId")
                        .HasColumnType("int");

                    b.Property<long>("LocationId1")
                        .HasColumnType("bigint");

                    b.Property<int>("TicketId")
                        .HasColumnType("int");

                    b.Property<int>("TypeOrder")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("LocationId1");

                    b.HasIndex("TicketId");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("Arkenstone.Entities.DbSet.Recipe", b =>
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

            modelBuilder.Entity("Arkenstone.Entities.DbSet.RecipeRessource", b =>
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

            modelBuilder.Entity("Arkenstone.Entities.DbSet.SubLocation", b =>
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

                    b.Property<long>("LocationId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex(new[] { "LocationId", "Flag", "CorporationId" }, "IX_SubLocations_LocationId")
                        .IsUnique();

                    b.ToTable("SubLocations");
                });

            modelBuilder.Entity("Arkenstone.Entities.DbSet.Ticket", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("CharacterId")
                        .HasColumnType("int");

                    b.Property<int>("ItemId")
                        .HasColumnType("int");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<int>("State")
                        .HasColumnType("int");

                    b.Property<int?>("TicketParentId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ItemId");

                    b.HasIndex("TicketParentId");

                    b.ToTable("Tickets");
                });

            modelBuilder.Entity("Arkenstone.Entities.DbSet.Inventory", b =>
                {
                    b.HasOne("Arkenstone.Entities.DbSet.Item", "Item")
                        .WithMany()
                        .HasForeignKey("ItemId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Arkenstone.Entities.DbSet.SubLocation", "SubLocation")
                        .WithMany()
                        .HasForeignKey("SubLocationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Item");

                    b.Navigation("SubLocation");
                });

            modelBuilder.Entity("Arkenstone.Entities.DbSet.InventoryBlueprint", b =>
                {
                    b.HasOne("Arkenstone.Entities.DbSet.Item", "Item")
                        .WithMany()
                        .HasForeignKey("ItemId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Arkenstone.Entities.DbSet.Location", "Location")
                        .WithMany()
                        .HasForeignKey("LocationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Item");

                    b.Navigation("Location");
                });

            modelBuilder.Entity("Arkenstone.Entities.DbSet.Order", b =>
                {
                    b.HasOne("Arkenstone.Entities.DbSet.Location", "Location")
                        .WithMany()
                        .HasForeignKey("LocationId1")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Arkenstone.Entities.DbSet.Ticket", "Ticket")
                        .WithMany()
                        .HasForeignKey("TicketId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Location");

                    b.Navigation("Ticket");
                });

            modelBuilder.Entity("Arkenstone.Entities.DbSet.Recipe", b =>
                {
                    b.HasOne("Arkenstone.Entities.DbSet.Item", "Item")
                        .WithMany()
                        .HasForeignKey("ItemId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Item");
                });

            modelBuilder.Entity("Arkenstone.Entities.DbSet.RecipeRessource", b =>
                {
                    b.HasOne("Arkenstone.Entities.DbSet.Item", "Item")
                        .WithMany()
                        .HasForeignKey("ItemId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Arkenstone.Entities.DbSet.Recipe", "Recipe")
                        .WithMany("RecipeRessource")
                        .HasForeignKey("RecipeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Item");

                    b.Navigation("Recipe");
                });

            modelBuilder.Entity("Arkenstone.Entities.DbSet.SubLocation", b =>
                {
                    b.HasOne("Arkenstone.Entities.DbSet.Location", "Location")
                        .WithMany("SubLocations")
                        .HasForeignKey("LocationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Location");
                });

            modelBuilder.Entity("Arkenstone.Entities.DbSet.Ticket", b =>
                {
                    b.HasOne("Arkenstone.Entities.DbSet.Item", "Item")
                        .WithMany()
                        .HasForeignKey("ItemId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Arkenstone.Entities.DbSet.Ticket", "TicketParent")
                        .WithMany("TicketEnfant")
                        .HasForeignKey("TicketParentId");

                    b.Navigation("Item");

                    b.Navigation("TicketParent");
                });

            modelBuilder.Entity("Arkenstone.Entities.DbSet.Location", b =>
                {
                    b.Navigation("SubLocations");
                });

            modelBuilder.Entity("Arkenstone.Entities.DbSet.Recipe", b =>
                {
                    b.Navigation("RecipeRessource");
                });

            modelBuilder.Entity("Arkenstone.Entities.DbSet.Ticket", b =>
                {
                    b.Navigation("TicketEnfant");
                });
#pragma warning restore 612, 618
        }
    }
}
