using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Arkenstone.Entities.Migrations
{
    /// <inheritdoc />
    public partial class AddCostIndice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SolarSystemId",
                table: "Locations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "CostIndices",
                columns: table => new
                {
                    SolarSystemId = table.Column<int>(type: "int", nullable: false),
                    type = table.Column<int>(type: "int", nullable: false),
                    Cost = table.Column<decimal>(type: "decimal(65,30)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CostIndices", x => new { x.SolarSystemId, x.type });
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CostIndices");

            migrationBuilder.DropColumn(
                name: "SolarSystemId",
                table: "Locations");
        }
    }
}
