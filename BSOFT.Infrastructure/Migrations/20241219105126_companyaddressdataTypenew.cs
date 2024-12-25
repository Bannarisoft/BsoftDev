using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BSOFT.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class companyaddressdataTypenew : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
             migrationBuilder.AlterColumn<string>(
                name: "AddressLine1",
                schema: "AppData",
                table: "CompanyAddress",
                type: "varchar(100)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

                 migrationBuilder.AlterColumn<string>(
                name: "AddressLine2",
                schema: "AppData",
                table: "CompanyAddress",
                type: "varchar(100)",
                nullable: true,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: false);

                migrationBuilder.AlterColumn<string>(
                name: "Phone",
                schema: "AppData",
                table: "CompanyAddress",
                type: "varchar(20)",
                nullable: true,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: false);

                migrationBuilder.AlterColumn<string>(
                name: "AlternatePhone",
                schema: "AppData",
                table: "CompanyAddress",
                type: "varchar(20)",
                nullable: true,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: false);

            migrationBuilder.AlterColumn<int>(
                name: "YearOfEstablishment",
                schema: "AppData",
                table: "Company",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Phone",
                schema: "AppData",
                table: "CompanyAddress",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(20)");

            migrationBuilder.AlterColumn<string>(
                name: "AlternatePhone",
                schema: "AppData",
                table: "CompanyAddress",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(20)");

            migrationBuilder.AlterColumn<string>(
                name: "YearOfEstablishment",
                schema: "AppData",
                table: "Company",
                type: "varchar(50)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
