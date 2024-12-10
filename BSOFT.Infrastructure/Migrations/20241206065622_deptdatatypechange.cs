using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BSOFT.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class deptdatatypechange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           migrationBuilder.AlterColumn<DateTime>(
        name: "CreatedDate", // Replace with the actual column name
        table: "Department",
        type: "datetime2", // SQL type for DateTime in SQL Server
        nullable: false,  // Adjust to true if the column should allow nulls
        oldClrType: typeof(string), // Old CLR type
        oldType: "nvarchar(max)", // Old SQL type, replace if different
        oldNullable: true);

        migrationBuilder.AlterColumn<DateTime>(
        name: "ModifiedDate", // Replace with the actual column name
        table: "Department",
        type: "datetime2", // SQL type for DateTime in SQL Server
        nullable: false,  // Adjust to true if the column should allow nulls
        oldClrType: typeof(string), // Old CLR type
        oldType: "nvarchar(max)", // Old SQL type, replace if different
        oldNullable: true);

   

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            
        }
    }
}
