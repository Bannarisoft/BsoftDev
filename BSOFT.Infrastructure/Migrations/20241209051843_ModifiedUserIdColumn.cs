using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BSOFT.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ModifiedUserIdColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
          // Drop the existing primary key
    migrationBuilder.DropPrimaryKey(
        name: "PK_Users",
        table: "Users");

    // Add a temporary column to hold GUID values
    migrationBuilder.AddColumn<Guid>(
        name: "TempId",
        table: "Users",
        type: "uniqueidentifier",
        nullable: false,
        defaultValueSql: "NEWID()"); // Generate new GUIDs for each row

    // Drop the old UserId column
    migrationBuilder.DropColumn(
        name: "UserId",
        table: "Users");

    // Rename TempId to Id
    migrationBuilder.RenameColumn(
        name: "TempId",
        table: "Users",
        newName: "Id");

    // Add the new primary key on the Id column
    migrationBuilder.AddPrimaryKey(
        name: "PK_Users",
        table: "Users",
        column: "Id");

}

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
      // Drop the primary key on the Id column
    migrationBuilder.DropPrimaryKey(
        name: "PK_Users",
        table: "Users");

    // Rename Id back to TempId
    migrationBuilder.RenameColumn(
        name: "Id",
        table: "Users",
        newName: "TempId");

    // Recreate the UserId column with the IDENTITY property
    migrationBuilder.AddColumn<int>(
        name: "UserId",
        table: "Users",
        type: "int",
        nullable: false,
        defaultValue: 0)
        .Annotation("SqlServer:Identity", "1, 1");

    // Drop the TempId column
    migrationBuilder.DropColumn(
        name: "TempId",
        table: "Users");

    // Restore the primary key on the UserId column
    migrationBuilder.AddPrimaryKey(
        name: "PK_Users",
        table: "Users",
        column: "UserId");


    }
}
}
