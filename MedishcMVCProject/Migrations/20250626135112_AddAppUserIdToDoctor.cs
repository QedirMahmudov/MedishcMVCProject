using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedishcMVCProject.Migrations
{
    /// <inheritdoc />
    public partial class AddAppUserIdToDoctor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AppUserId",
                table: "Doctors",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Doctors_AppUserId",
                table: "Doctors",
                column: "AppUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Doctors_AspNetUsers_AppUserId",
                table: "Doctors",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Doctors_AspNetUsers_AppUserId",
                table: "Doctors");

            migrationBuilder.DropIndex(
                name: "IX_Doctors_AppUserId",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "AppUserId",
                table: "Doctors");
        }
    }
}
