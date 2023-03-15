using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Arkenstone.Entities.Migrations
{
    public partial class Initial2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Locations_StructureTypes_StructureTypeId",
                table: "Locations");

            migrationBuilder.DropColumn(
                name: "StructureId",
                table: "Locations");

            migrationBuilder.AlterColumn<int>(
                name: "StructureTypeId",
                table: "Locations",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Locations_StructureTypes_StructureTypeId",
                table: "Locations",
                column: "StructureTypeId",
                principalTable: "StructureTypes",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Locations_StructureTypes_StructureTypeId",
                table: "Locations");

            migrationBuilder.AlterColumn<int>(
                name: "StructureTypeId",
                table: "Locations",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StructureId",
                table: "Locations",
                type: "int",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Locations_StructureTypes_StructureTypeId",
                table: "Locations",
                column: "StructureTypeId",
                principalTable: "StructureTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
