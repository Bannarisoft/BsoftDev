using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BSOFT.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class userroleRoleNamelength : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

               migrationBuilder.AlterColumn<string>(
    name: "RoleName",
    schema: "AppSecurity",
    table: "UserRole",
    type: "varchar(50)",
    maxLength: 10,
    nullable: false,
    oldClrType: typeof(string),
    oldType: "varchar(max)");

migrationBuilder.AlterColumn<string>(
    name: "Description",
    schema: "AppSecurity",
    table: "UserRole",
    type: "varchar(250)",
    maxLength: 50,
    nullable: false,
    oldClrType: typeof(string),
    oldType: "nvarchar(max)");

     migrationBuilder.AlterColumn<bool>(
    name: "IsActive",
    schema: "AppSecurity",
    table: "UserRole",
    type: "bit",
    nullable: false,
    oldClrType: typeof(int),
    oldType: "tinyint"); 

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
