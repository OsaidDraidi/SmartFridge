using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FridgeProject.Migrations
{
    /// <inheritdoc />
    public partial class addCreatedByColumnToUserItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "userItem",
                type: "nvarchar(max)",
                nullable: true,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "userItem");
        }
    }
}
