using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FridgeProject.Migrations
{
    /// <inheritdoc />
    public partial class edit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_items_Recipes_RecipeId",
                table: "items");

            migrationBuilder.DropIndex(
                name: "IX_items_RecipeId",
                table: "items");

            migrationBuilder.DropColumn(
                name: "RecipeId",
                table: "items");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RecipeId",
                table: "items",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_items_RecipeId",
                table: "items",
                column: "RecipeId");

            migrationBuilder.AddForeignKey(
                name: "FK_items_Recipes_RecipeId",
                table: "items",
                column: "RecipeId",
                principalTable: "Recipes",
                principalColumn: "Id");
        }
    }
}
