using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedishcMVCProject.Migrations
{
    /// <inheritdoc />
    public partial class SpecialistCategoryByProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SpecialistId",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_SpecialistId",
                table: "Products",
                column: "SpecialistId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Specialists_SpecialistId",
                table: "Products",
                column: "SpecialistId",
                principalTable: "Specialists",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Specialists_SpecialistId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_SpecialistId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "SpecialistId",
                table: "Products");
        }
    }
}
