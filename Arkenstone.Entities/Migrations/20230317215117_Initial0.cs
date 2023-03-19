using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Arkenstone.Entities.Migrations
{
    public partial class Initial0 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Alliances",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alliances", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Corporations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Corporations", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Published = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MarketGroupId = table.Column<int>(type: "int", nullable: false),
                    PriceBuy = table.Column<double>(type: "double", nullable: true),
                    PriceSell = table.Column<double>(type: "double", nullable: true),
                    PriceAdjustedPrice = table.Column<double>(type: "double", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "MarketGroupTrees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    ParentId = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarketGroupTrees", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Characters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Token = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RefreshToken = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CorporationId = table.Column<int>(type: "int", nullable: false),
                    AllianceId = table.Column<int>(type: "int", nullable: false),
                    CharacterMainId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Characters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Characters_Alliances_AllianceId",
                        column: x => x.AllianceId,
                        principalTable: "Alliances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Characters_Characters_CharacterMainId",
                        column: x => x.CharacterMainId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Characters_Corporations_CorporationId",
                        column: x => x.CorporationId,
                        principalTable: "Corporations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Recipes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Time = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recipes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Recipes_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "RigsManufacturings",
                columns: table => new
                {
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    MarketIdEffect = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MarketIdNotEffect = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CostEffect = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    TimeEffect = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    MaterialEffect = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    MultiplierHS = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    MultiplierLS = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    MultiplierNS = table.Column<decimal>(type: "decimal(65,30)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RigsManufacturings", x => x.ItemId);
                    table.ForeignKey(
                        name: "FK_RigsManufacturings_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "StructureTypes",
                columns: table => new
                {
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    CostEffect = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    TimeEffect = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    MaterialEffect = table.Column<decimal>(type: "decimal(65,30)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StructureTypes", x => x.ItemId);
                    table.ForeignKey(
                        name: "FK_StructureTypes_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "RecipeRessources",
                columns: table => new
                {
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    RecipeId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecipeRessources", x => new { x.RecipeId, x.ItemId });
                    table.ForeignKey(
                        name: "FK_RecipeRessources_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RecipeRessources_Recipes_RecipeId",
                        column: x => x.RecipeId,
                        principalTable: "Recipes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Security = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    StructureTypeId = table.Column<int>(type: "int", nullable: false),
                    CanReact = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CanReprocess = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CanProd = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CanCopy = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CanResearch = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CanInvent = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Locations_StructureTypes_StructureTypeId",
                        column: x => x.StructureTypeId,
                        principalTable: "StructureTypes",
                        principalColumn: "ItemId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "LocationRigsManufacturings",
                columns: table => new
                {
                    LocationId = table.Column<long>(type: "bigint", nullable: false),
                    RigsManufacturingId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocationRigsManufacturings", x => new { x.LocationId, x.RigsManufacturingId });
                    table.ForeignKey(
                        name: "FK_LocationRigsManufacturings_Locations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LocationRigsManufacturings_RigsManufacturings_RigsManufactur~",
                        column: x => x.RigsManufacturingId,
                        principalTable: "RigsManufacturings",
                        principalColumn: "ItemId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ProdAchats",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CorporationId = table.Column<int>(type: "int", nullable: false),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    MEefficiency = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    LocationId = table.Column<long>(type: "bigint", nullable: false),
                    ProdAchatParentId = table.Column<long>(type: "bigint", nullable: true),
                    State = table.Column<int>(type: "int", nullable: false),
                    CharacterIdReservation = table.Column<int>(type: "int", nullable: true),
                    DatetimeReservation = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProdAchats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProdAchats_Corporations_CorporationId",
                        column: x => x.CorporationId,
                        principalTable: "Corporations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProdAchats_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProdAchats_Locations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProdAchats_ProdAchats_ProdAchatParentId",
                        column: x => x.ProdAchatParentId,
                        principalTable: "ProdAchats",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SubLocations",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    LocationId = table.Column<long>(type: "bigint", nullable: false),
                    Flag = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CorporationId = table.Column<int>(type: "int", nullable: false),
                    IsAssetAnalysed = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubLocations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubLocations_Locations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Inventorys",
                columns: table => new
                {
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    SubLocationId = table.Column<long>(type: "bigint", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inventorys", x => new { x.ItemId, x.SubLocationId });
                    table.ForeignKey(
                        name: "FK_Inventorys_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Inventorys_SubLocations_SubLocationId",
                        column: x => x.SubLocationId,
                        principalTable: "SubLocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Characters_AllianceId",
                table: "Characters",
                column: "AllianceId");

            migrationBuilder.CreateIndex(
                name: "IX_Characters_CharacterMainId",
                table: "Characters",
                column: "CharacterMainId");

            migrationBuilder.CreateIndex(
                name: "IX_Characters_CorporationId",
                table: "Characters",
                column: "CorporationId");

            migrationBuilder.CreateIndex(
                name: "IX_Inventorys_SubLocationId",
                table: "Inventorys",
                column: "SubLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_LocationRigsManufacturings_RigsManufacturingId",
                table: "LocationRigsManufacturings",
                column: "RigsManufacturingId");

            migrationBuilder.CreateIndex(
                name: "IX_Locations_StructureTypeId",
                table: "Locations",
                column: "StructureTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ProdAchats_CorporationId",
                table: "ProdAchats",
                column: "CorporationId");

            migrationBuilder.CreateIndex(
                name: "IX_ProdAchats_ItemId",
                table: "ProdAchats",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ProdAchats_LocationId",
                table: "ProdAchats",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_ProdAchats_ProdAchatParentId",
                table: "ProdAchats",
                column: "ProdAchatParentId");

            migrationBuilder.CreateIndex(
                name: "IX_RecipeRessources_ItemId",
                table: "RecipeRessources",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Recipes_ItemId",
                table: "Recipes",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_SubLocations_LocationId",
                table: "SubLocations",
                columns: new[] { "LocationId", "Flag", "CorporationId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Characters");

            migrationBuilder.DropTable(
                name: "Inventorys");

            migrationBuilder.DropTable(
                name: "LocationRigsManufacturings");

            migrationBuilder.DropTable(
                name: "MarketGroupTrees");

            migrationBuilder.DropTable(
                name: "ProdAchats");

            migrationBuilder.DropTable(
                name: "RecipeRessources");

            migrationBuilder.DropTable(
                name: "Alliances");

            migrationBuilder.DropTable(
                name: "SubLocations");

            migrationBuilder.DropTable(
                name: "RigsManufacturings");

            migrationBuilder.DropTable(
                name: "Corporations");

            migrationBuilder.DropTable(
                name: "Recipes");

            migrationBuilder.DropTable(
                name: "Locations");

            migrationBuilder.DropTable(
                name: "StructureTypes");

            migrationBuilder.DropTable(
                name: "Items");
        }
    }
}
