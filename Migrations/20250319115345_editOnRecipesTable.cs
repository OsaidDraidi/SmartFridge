using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FridgeProject.Migrations
{
    /// <inheritdoc />
    public partial class editOnRecipesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatorId",
                table: "Recipes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatorId",
                table: "items",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "Recipes");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "items");
        }
    }
}
