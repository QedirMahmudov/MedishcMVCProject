using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedishcMVCProject.Migrations
{
    /// <inheritdoc />
    public partial class doctornullableupdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Doctors_Degrees_DegreeId",
                table: "Doctors");

            migrationBuilder.DropForeignKey(
                name: "FK_Doctors_Universities_UniversityId",
                table: "Doctors");

            migrationBuilder.AlterColumn<int>(
                name: "UniversityId",
                table: "Doctors",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "DegreeId",
                table: "Doctors",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Doctors_Degrees_DegreeId",
                table: "Doctors",
                column: "DegreeId",
                principalTable: "Degrees",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Doctors_Universities_UniversityId",
                table: "Doctors",
                column: "UniversityId",
                principalTable: "Universities",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Doctors_Degrees_DegreeId",
                table: "Doctors");

            migrationBuilder.DropForeignKey(
                name: "FK_Doctors_Universities_UniversityId",
                table: "Doctors");

            migrationBuilder.AlterColumn<int>(
                name: "UniversityId",
                table: "Doctors",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DegreeId",
                table: "Doctors",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Doctors_Degrees_DegreeId",
                table: "Doctors",
                column: "DegreeId",
                principalTable: "Degrees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Doctors_Universities_UniversityId",
                table: "Doctors",
                column: "UniversityId",
                principalTable: "Universities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
