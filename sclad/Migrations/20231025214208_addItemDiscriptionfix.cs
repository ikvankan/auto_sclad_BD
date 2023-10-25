using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace sclad.Migrations
{
    public partial class addItemDiscriptionfix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ItemDiscription",
                table: "Item",
                newName: "Discription");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Discription",
                table: "Item",
                newName: "ItemDiscription");
        }
    }
}
