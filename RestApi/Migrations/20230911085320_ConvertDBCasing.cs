using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RestApi.Migrations
{
    /// <inheritdoc />
    public partial class ConvertDBCasing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Temperatures",
                table: "Temperatures");

            migrationBuilder.DropColumn(
                name: "DateTime",
                table: "Temperatures");

            migrationBuilder.RenameTable(
                name: "Temperatures",
                newName: "temperatures");

            migrationBuilder.RenameColumn(
                name: "Temp",
                table: "temperatures",
                newName: "temp");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "temperatures",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "temperatures",
                newName: "created_at");

            migrationBuilder.AddPrimaryKey(
                name: "PK_temperatures",
                table: "temperatures",
                column: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_temperatures",
                table: "temperatures");

            migrationBuilder.RenameTable(
                name: "temperatures",
                newName: "Temperatures");

            migrationBuilder.RenameColumn(
                name: "temp",
                table: "Temperatures",
                newName: "Temp");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Temperatures",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "Temperatures",
                newName: "CreatedAt");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateTime",
                table: "Temperatures",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddPrimaryKey(
                name: "PK_Temperatures",
                table: "Temperatures",
                column: "Id");
        }
    }
}
