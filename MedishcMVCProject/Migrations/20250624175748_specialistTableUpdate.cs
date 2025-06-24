using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedishcMVCProject.Migrations
{
    /// <inheritdoc />
    public partial class specialistTableUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Doctors_Specialists_SpecialistId",
                table: "Doctors");

            migrationBuilder.AddColumn<int>(
                name: "HeadDoctorId",
                table: "Specialists",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Specialists_HeadDoctorId",
                table: "Specialists",
                column: "HeadDoctorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Doctors_Specialists_SpecialistId",
                table: "Doctors",
                column: "SpecialistId",
                principalTable: "Specialists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Specialists_Doctors_HeadDoctorId",
                table: "Specialists",
                column: "HeadDoctorId",
                principalTable: "Doctors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Doctors_Specialists_SpecialistId",
                table: "Doctors");

            migrationBuilder.DropForeignKey(
                name: "FK_Specialists_Doctors_HeadDoctorId",
                table: "Specialists");

            migrationBuilder.DropIndex(
                name: "IX_Specialists_HeadDoctorId",
                table: "Specialists");

            migrationBuilder.DropColumn(
                name: "HeadDoctorId",
                table: "Specialists");

            migrationBuilder.AddForeignKey(
                name: "FK_Doctors_Specialists_SpecialistId",
                table: "Doctors",
                column: "SpecialistId",
                principalTable: "Specialists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
