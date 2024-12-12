using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BSOFT.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Deptnullallow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

        migrationBuilder.AlterColumn<int>(
        name: "ModifiedBy",
        schema: "AppData",
        table: "Department",
        type: "int",
        nullable: true,
        oldClrType: typeof(int),
        oldType: "int");

    migrationBuilder.AlterColumn<DateTime>(
        name: "ModifiedAt",
        schema: "AppData",
        table: "Department",
        type: "datetime2",
        nullable: true,
        oldClrType: typeof(DateTime),
        oldType: "datetime2");

    migrationBuilder.AlterColumn<string>(
        name: "ModifiedByName",
        schema: "AppData",
        table: "Department",
        type: "nvarchar(max)",
        nullable: true,
        oldClrType: typeof(string),
        oldType: "nvarchar(max)");

    migrationBuilder.AlterColumn<string>(
        name: "ModifiedIP",
        schema: "AppData",
        table: "Department",
        type: "nvarchar(max)",
        nullable: true,
        oldClrType: typeof(string),
        oldType: "nvarchar(max)");
          
          
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
             migrationBuilder.AlterColumn<int>(
        name: "ModifiedBy",
        schema: "AppData",
        table: "Department",
        type: "int",
        nullable: false,
        defaultValue: 0,
        oldClrType: typeof(int),
        oldType: "int",
        oldNullable: true);

    migrationBuilder.AlterColumn<DateTime>(
        name: "ModifiedAt",
        schema: "AppData",
        table: "Department",
        type: "datetime2",
        nullable: false,
        defaultValue: DateTime.MinValue,
        oldClrType: typeof(DateTime),
        oldType: "datetime2",
        oldNullable: true);

    migrationBuilder.AlterColumn<string>(
        name: "ModifiedByName",
        schema: "AppData",
        table: "Department",
        type: "nvarchar(max)",
        nullable: false,
        defaultValue: "",
        oldClrType: typeof(string),
        oldType: "nvarchar(max)",
        oldNullable: true);

    migrationBuilder.AlterColumn<string>(
        name: "ModifiedIP",
        schema: "AppData",
        table: "Department",
        type: "nvarchar(max)",
        nullable: false,
        defaultValue: "",
        oldClrType: typeof(string),
        oldType: "nvarchar(max)",
        oldNullable: true);
           
        }
    }
}
