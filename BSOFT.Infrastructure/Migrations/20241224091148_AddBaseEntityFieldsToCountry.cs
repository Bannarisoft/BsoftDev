using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BSOFT.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBaseEntityFieldsToCountry : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Code",
                schema: "AppData",
                table: "Country");

            migrationBuilder.DropColumn(
                name: "Name",
                schema: "AppData",
                table: "Country");

            migrationBuilder.RenameTable(
                name: "Country",
                schema: "AppData",
                newName: "Country");

            migrationBuilder.AddColumn<string>(
                name: "CountryCode",
                table: "Country",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CountryName",
                table: "Country",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Country",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                table: "Country",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByName",
                table: "Country",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CreatedIP",
                table: "Country",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAt",
                table: "Country",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ModifiedBy",
                table: "Country",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedByName",
                table: "Country",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedIP",
                table: "Country",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CountryCode",
                table: "Country");

            migrationBuilder.DropColumn(
                name: "CountryName",
                table: "Country");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Country");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Country");

            migrationBuilder.DropColumn(
                name: "CreatedByName",
                table: "Country");

            migrationBuilder.DropColumn(
                name: "CreatedIP",
                table: "Country");

            migrationBuilder.DropColumn(
                name: "ModifiedAt",
                table: "Country");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "Country");

            migrationBuilder.DropColumn(
                name: "ModifiedByName",
                table: "Country");

            migrationBuilder.DropColumn(
                name: "ModifiedIP",
                table: "Country");

            migrationBuilder.RenameTable(
                name: "Country",
                newName: "Country",
                newSchema: "AppData");

            migrationBuilder.AddColumn<string>(
                name: "Code",
                schema: "AppData",
                table: "Country",
                type: "varchar(5)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                schema: "AppData",
                table: "Country",
                type: "varchar(50)",
                nullable: false,
                defaultValue: "");
        }
    }
}
