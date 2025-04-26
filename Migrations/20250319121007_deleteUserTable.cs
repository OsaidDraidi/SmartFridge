using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FridgeProject.Migrations
{
    /// <inheritdoc />
    public partial class deleteUserTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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
                name: "UserId",
                table: "userItem");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "userItem",
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
                principalColumn: "Id");
        }
    }
}
