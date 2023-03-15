using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Arkenstone.Entities.Migrations
{
    public partial class Initial5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MarketIdNotEffect",
                table: "RigsManufacturings",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MarketIdNotEffect",
                table: "RigsManufacturings");
        }
    }
}
