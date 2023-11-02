using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace sclad.Migrations
{
    public partial class ShortDecsAddTotalCostRemoveFromItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalCost",
                table: "Item");

            migrationBuilder.AddColumn<string>(
                name: "ShortDesc",
                table: "Item",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShortDesc",
                table: "Item");

            migrationBuilder.AddColumn<decimal>(
                name: "TotalCost",
                table: "Item",
                type: "decimal(18,2)",
                nullable: true);
        }
    }
}
