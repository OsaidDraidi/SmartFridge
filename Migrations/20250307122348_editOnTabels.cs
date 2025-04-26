using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FridgeProject.Migrations
{
    /// <inheritdoc />
    public partial class editOnTabels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Count",
                table: "userItem");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "userItem");

            migrationBuilder.DropColumn(
                name: "Count",
                table: "items");

            migrationBuilder.DropColumn(
                name: "ExpiredDate",
                table: "items");

            migrationBuilder.AddColumn<DateOnly>(
                name: "CreateDate",
                table: "userItem",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "userItem",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "userItem",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ExpiryDays",
                table: "items",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_userItem_UserId",
                table: "userItem",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_userItem_users_UserId",
                table: "userItem",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_userItem_users_UserId",
                table: "userItem");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropIndex(
                name: "IX_userItem_UserId",
                table: "userItem");

            migrationBuilder.DropColumn(
                name: "CreateDate",
                table: "userItem");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "userItem");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "userItem");

            migrationBuilder.DropColumn(
                name: "ExpiryDays",
                table: "items");

            migrationBuilder.AddColumn<double>(
                name: "Count",
                table: "userItem",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "userItem",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "Count",
                table: "items",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<DateOnly>(
                name: "ExpiredDate",
                table: "items",
                type: "date",
                nullable: true);
        }
    }
}
