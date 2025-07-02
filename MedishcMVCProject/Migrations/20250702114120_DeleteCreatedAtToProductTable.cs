using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedishcMVCProject.Migrations
{
    /// <inheritdoc />
    public partial class DeleteCreatedAtToProductTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Products");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Products",
                type: "datetime2",
                nullable: true,
                defaultValueSql: "GETUTCDATE()");
        }
    }
}
