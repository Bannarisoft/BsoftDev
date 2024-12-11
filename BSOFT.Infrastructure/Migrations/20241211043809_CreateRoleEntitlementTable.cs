using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BSOFT.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CreateRoleEntitlementTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            
            migrationBuilder.CreateTable(
                name: "RoleEntitlement",
                schema: "AppSecurity",
                columns: table => new
                {
                    RoleEntitlementId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleEntitlement", x => x.RoleEntitlementId);
                });

            migrationBuilder.CreateTable(
                name: "MenuPermission",
                schema: "AppSecurity",
                columns: table => new
                {
                    MenuPermissionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MenuName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CanView = table.Column<bool>(type: "bit", nullable: false),
                    CanAdd = table.Column<bool>(type: "bit", nullable: false),
                    CanUpdate = table.Column<bool>(type: "bit", nullable: false),
                    CanDelete = table.Column<bool>(type: "bit", nullable: false),
                    CanExport = table.Column<bool>(type: "bit", nullable: false),
                    CanApprove = table.Column<bool>(type: "bit", nullable: false),
                    RoleEntitlementId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuPermission", x => x.MenuPermissionId);
                    table.ForeignKey(
                        name: "FK_MenuPermission_RoleEntitlement_RoleEntitlementId",
                        column: x => x.RoleEntitlementId,
                        principalSchema: "AppSecurity",
                        principalTable: "RoleEntitlement",
                        principalColumn: "RoleEntitlementId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MenuPermission_RoleEntitlementId",
                schema: "AppSecurity",
                table: "MenuPermission",
                column: "RoleEntitlementId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MenuPermission",
                schema: "AppSecurity");

            migrationBuilder.DropTable(
                name: "RoleEntitlement",
                schema: "AppSecurity");          
        }
    }
}
