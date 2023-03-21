using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Arkenstone.Entities.Migrations
{
    /// <inheritdoc />
    public partial class AddVolumeToItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Volume",
                table: "Items",
                type: "double",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Volume",
                table: "Items");
        }
    }
}
