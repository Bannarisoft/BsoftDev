using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BSOFT.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class companydataType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CompanyName",
                schema: "AppData",
                table: "Company",
                type: "varchar(50)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

                 migrationBuilder.AlterColumn<string>(
                name: "LegalName",
                schema: "AppData",
                table: "Company",
                type: "varchar(50)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

                migrationBuilder.AlterColumn<string>(
                name: "GstNumber",
                schema: "AppData",
                table: "Company",
                type: "varchar(50)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

                  migrationBuilder.AlterColumn<string>(
                name: "TIN",
                schema: "AppData",
                table: "Company",
                type: "varchar(50)",
                nullable: true,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: false);

                migrationBuilder.AlterColumn<string>(
                name: "TAN",
                schema: "AppData",
                table: "Company",
                type: "varchar(50)",
                nullable: true,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: false);

                migrationBuilder.AlterColumn<string>(
                name: "CSTNo",
                schema: "AppData",
                table: "Company",
                type: "varchar(50)",
                nullable: true,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: false);

                migrationBuilder.AlterColumn<string>(
                name: "Website",
                schema: "AppData",
                table: "Company",
                type: "varchar(50)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
