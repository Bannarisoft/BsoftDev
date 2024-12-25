using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BSOFT.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class deptchangestructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CoId",
                schema: "AppData",
                table: "Department",
                newName: "CompanyId");

            migrationBuilder.RenameColumn(
                name: "DeptId",
                schema: "AppData",
                table: "Department",
                newName: "Id");

                 migrationBuilder.AlterColumn<string>(
                name: "CreatedIP",
                schema: "AppData",
                table: "Department",
                type: "varchar(15)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ModifiedIP",
                schema: "AppData",
                table: "Department",
                type: "varchar(15)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

           
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
           
        }
    }
}
