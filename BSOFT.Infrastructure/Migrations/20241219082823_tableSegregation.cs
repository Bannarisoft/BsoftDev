using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BSOFT.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class tableSegregation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Company",
                schema: "AppData",
                table: "Company");

                migrationBuilder.DropColumn(
                name: "Address1",
                schema: "AppData",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "Address2",
                schema: "AppData",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "Address3",
                schema: "AppData",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "Email",
                schema: "AppData",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "Phone",
                schema: "AppData",
                table: "Company");

                 migrationBuilder.RenameColumn(
                name: "YearofEstablishment",
                schema: "AppData",
                table: "Company",
                newName: "YearOfEstablishment");

                migrationBuilder.RenameColumn(
                name: "CoId",
                schema: "AppData",
                table: "Company",
                newName: "Id");

                migrationBuilder.AlterColumn<string>(
                name: "TIN",
                schema: "AppData",
                table: "Company",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TAN",
                schema: "AppData",
                table: "Company",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Logo",
                schema: "AppData",
                table: "Company",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CSTNo",
                schema: "AppData",
                table: "Company",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Company",
                schema: "AppData",
                table: "Company",
                column: "Id");

           
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
             
        }
    }
}
