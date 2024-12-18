using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BSOFT.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Unitmigration3tablessplit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address1",
                schema: "AppData",
                table: "Unit");

            migrationBuilder.DropColumn(
                name: "Address2",
                schema: "AppData",
                table: "Unit");

            migrationBuilder.DropColumn(
                name: "Address3",
                schema: "AppData",
                table: "Unit");

            migrationBuilder.DropColumn(
                name: "Email",
                schema: "AppData",
                table: "Unit");

            migrationBuilder.DropColumn(
                name: "Mobile",
                schema: "AppData",
                table: "Unit");

            migrationBuilder.RenameColumn(
                name: "DivId",
                schema: "AppData",
                table: "Unit",
                newName: "DivisionId");

            migrationBuilder.RenameColumn(
                name: "CoId",
                schema: "AppData",
                table: "Unit",
                newName: "CompanyId");

            migrationBuilder.AlterColumn<string>(
                name: "UnitName",
                schema: "AppData",
                table: "Unit",
                type: "varchar(50)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "UnitHeadName",
                schema: "AppData",
                table: "Unit",
                type: "varchar(50)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ShortName",
                schema: "AppData",
                table: "Unit",
                type: "varchar(10)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                schema: "AppData",
                table: "Unit",
                type: "bit",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint");

            migrationBuilder.CreateTable(
                name: "UnitAddress",
                schema: "AppData",
                columns: table => new
                {
                    UnitAddressId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UnitId = table.Column<int>(type: "int", nullable: false),
                    CountryId = table.Column<int>(type: "int", nullable: false),
                    StateId = table.Column<int>(type: "int", nullable: false),
                    CityId = table.Column<int>(type: "int", nullable: false),
                    AddressLine1 = table.Column<string>(type: "varchar(250)", nullable: false),
                    AddressLine2 = table.Column<string>(type: "varchar(250)", nullable: false),
                    PinCode = table.Column<int>(type: "int", nullable: false),
                    ContactNumber = table.Column<string>(type: "nvarchar(20)", nullable: false),
                    AlternateNumber = table.Column<string>(type: "nvarchar(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnitAddress", x => x.UnitAddressId);
                    table.ForeignKey(
                        name: "FK_UnitAddress_Unit_UnitId",
                        column: x => x.UnitId,
                        principalSchema: "AppData",
                        principalTable: "Unit",
                        principalColumn: "UnitId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UnitContacts",
                schema: "AppData",
                columns: table => new
                {
                    UnitContactId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UnitId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "varchar(50)", nullable: false),
                    Designation = table.Column<string>(type: "varchar(50)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    PhoneNo = table.Column<string>(type: "nvarchar(20)", nullable: false),
                    Remarks = table.Column<string>(type: "varchar(250)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnitContacts", x => x.UnitContactId);
                    table.ForeignKey(
                        name: "FK_UnitContacts_Unit_UnitId",
                        column: x => x.UnitId,
                        principalSchema: "AppData",
                        principalTable: "Unit",
                        principalColumn: "UnitId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UnitAddress_UnitId",
                schema: "AppData",
                table: "UnitAddress",
                column: "UnitId");

            migrationBuilder.CreateIndex(
                name: "IX_UnitContacts_UnitId",
                schema: "AppData",
                table: "UnitContacts",
                column: "UnitId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UnitAddress",
                schema: "AppData");

            migrationBuilder.DropTable(
                name: "UnitContacts",
                schema: "AppData");

            migrationBuilder.RenameColumn(
                name: "DivisionId",
                schema: "AppData",
                table: "Unit",
                newName: "DivId");

            migrationBuilder.RenameColumn(
                name: "CompanyId",
                schema: "AppData",
                table: "Unit",
                newName: "CoId");

            migrationBuilder.AlterColumn<string>(
                name: "UnitName",
                schema: "AppData",
                table: "Unit",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)");

            migrationBuilder.AlterColumn<string>(
                name: "UnitHeadName",
                schema: "AppData",
                table: "Unit",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)");

            migrationBuilder.AlterColumn<string>(
                name: "ShortName",
                schema: "AppData",
                table: "Unit",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(10)");

            migrationBuilder.AlterColumn<byte>(
                name: "IsActive",
                schema: "AppData",
                table: "Unit",
                type: "tinyint",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AddColumn<string>(
                name: "Address1",
                schema: "AppData",
                table: "Unit",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Address2",
                schema: "AppData",
                table: "Unit",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address3",
                schema: "AppData",
                table: "Unit",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                schema: "AppData",
                table: "Unit",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Mobile",
                schema: "AppData",
                table: "Unit",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
