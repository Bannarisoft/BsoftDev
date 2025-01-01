using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BSOFT.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class divisioncolumnchange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
        

            migrationBuilder.RenameColumn(
                name: "DivId",
                schema: "AppData",
                table: "Division",
                newName: "Id");

            migrationBuilder.AlterColumn<string>(
                name: "ShortName",
                schema: "AppData",
                table: "Division",
                type: "varchar(50)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "AppData",
                table: "Division",
                type: "varchar(100)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                schema: "AppData",
                table: "Division",
                type: "bit",
                nullable: false);

           

           
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            

            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "AppData",
                table: "Division",
                newName: "DivId");

           

            migrationBuilder.AlterColumn<string>(
                name: "ShortName",
                schema: "AppData",
                table: "Division",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "AppData",
                table: "Division",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)");

         

           
        }
    }
}
