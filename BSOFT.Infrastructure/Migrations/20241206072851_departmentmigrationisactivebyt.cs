using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BSOFT.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class departmentmigrationisactivebyt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
             migrationBuilder.AlterColumn<byte>(
        name: "IsActive", // Replace with the actual column name
        table: "Department",
        type: "tinyint", // SQL type for DateTime in SQL Server
        nullable: false,  // Adjust to true if the column should allow nulls
        oldClrType: typeof(string), // Old CLR type
        oldType: "nvarchar(max)");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        migrationBuilder.AlterColumn<byte>(
        name: "IsActive", // Replace with the actual column name
        table: "Department",
        type: "tinyint", // SQL type for DateTime in SQL Server
        nullable: false,  // Adjust to true if the column should allow nulls
        oldClrType: typeof(string), // Old CLR type
        oldType: "nvarchar(max)" 
      );

        }
    }
}
