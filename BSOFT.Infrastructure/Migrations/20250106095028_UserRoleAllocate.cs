using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BSOFT.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UserRoleAllocate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
         
            migrationBuilder.CreateTable(
                name: "UserRoleAllocation",
                schema: "AppSecurity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserRoleId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<byte>(type: "tinyint", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedIP = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedBy = table.Column<int>(type: "int", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedByName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedIP = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoleAllocations_Id", x => x.Id)
                        .Annotation("SqlServer:Clustered", true);
                    table.ForeignKey(
                        name: "FK_UserRoleAllocation_UserRole_UserRoleId",
                        column: x => x.UserRoleId,
                        principalSchema: "AppSecurity",
                        principalTable: "UserRole",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoleAllocation_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "AppSecurity",
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserRoleAllocation_UserId",
                schema: "AppSecurity",
                table: "UserRoleAllocation",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoleAllocation_UserRoleId",
                schema: "AppSecurity",
                table: "UserRoleAllocation",
                column: "UserRoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            
           migrationBuilder.DropTable(
                name: "UserRoleAllocation");

          
        }
    }
}
