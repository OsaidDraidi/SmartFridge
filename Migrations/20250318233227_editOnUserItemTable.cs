using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FridgeProject.Migrations
{
    /// <inheritdoc />
    public partial class editOnUserItemTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_userItem_users_UserId",
                table: "userItem");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "userItem",
                newName: "CreatorId");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "userItem",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "CreatorEmail",
                table: "userItem",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_userItem_users_UserId",
                table: "userItem",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_userItem_users_UserId",
                table: "userItem");

            migrationBuilder.DropColumn(
                name: "CreatorEmail",
                table: "userItem");

            migrationBuilder.RenameColumn(
                name: "CreatorId",
                table: "userItem",
                newName: "CreatedBy");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "userItem",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_userItem_users_UserId",
                table: "userItem",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
