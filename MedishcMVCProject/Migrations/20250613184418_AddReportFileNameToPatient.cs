using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedishcMVCProject.Migrations
{
    /// <inheritdoc />
    public partial class AddReportFileNameToPatient : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Patients_BloodGroups_BloodGroupId",
                table: "Patients");

            migrationBuilder.AlterColumn<int>(
                name: "BloodGroupId",
                table: "Patients",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReportFileName",
                table: "Patients",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Patients_BloodGroups_BloodGroupId",
                table: "Patients",
                column: "BloodGroupId",
                principalTable: "BloodGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Patients_BloodGroups_BloodGroupId",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "ReportFileName",
                table: "Patients");

            migrationBuilder.AlterColumn<int>(
                name: "BloodGroupId",
                table: "Patients",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Patients_BloodGroups_BloodGroupId",
                table: "Patients",
                column: "BloodGroupId",
                principalTable: "BloodGroups",
                principalColumn: "Id");
        }
    }
}
