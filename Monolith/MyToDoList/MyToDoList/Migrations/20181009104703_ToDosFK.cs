using Microsoft.EntityFrameworkCore.Migrations;

namespace MyToDoList.Migrations
{
    public partial class ToDosFK : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "ToDos",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.CreateIndex(
                name: "IX_ToDos_UserId",
                table: "ToDos",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ToDos_AspNetUsers_UserId",
                table: "ToDos",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ToDos_AspNetUsers_UserId",
                table: "ToDos");

            migrationBuilder.DropIndex(
                name: "IX_ToDos_UserId",
                table: "ToDos");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "ToDos",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);
        }
    }
}
