using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BSOFT.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RoleModuleMenuTblCr : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MenuPermission",
                schema: "AppSecurity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RoleEntitlement",
                schema: "AppSecurity",
                table: "RoleEntitlement");

            migrationBuilder.RenameTable(
                name: "RoleEntitlement",
                schema: "AppSecurity",
                newName: "RoleEntitlements",
                newSchema: "AppSecurity");

            migrationBuilder.RenameColumn(
                name: "UserRoleId",
                schema: "AppSecurity",
                table: "RoleEntitlements",
                newName: "RoleId");

            migrationBuilder.RenameColumn(
                name: "RoleEntitlementId",
                schema: "AppSecurity",
                table: "RoleEntitlements",
                newName: "Id");

           migrationBuilder.AlterColumn<string>(
                name: "CreatedIP",
                schema: "AppSecurity",
                table: "RoleEntitlements",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedByName",
                schema: "AppSecurity",
                table: "RoleEntitlements",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CreatedBy",
                schema: "AppSecurity",
                table: "RoleEntitlements",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<bool>(
                name: "CanAdd",
                schema: "AppSecurity",
                table: "RoleEntitlements",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CanDelete",
                schema: "AppSecurity",
                table: "RoleEntitlements",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CanExport",
                schema: "AppSecurity",
                table: "RoleEntitlements",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CanUpdate",
                schema: "AppSecurity",
                table: "RoleEntitlements",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CanView",
                schema: "AppSecurity",
                table: "RoleEntitlements",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<byte>(
                name: "IsActive",
                schema: "AppSecurity",
                table: "RoleEntitlements",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<int>(
                name: "MenuId",
                schema: "AppSecurity",
                table: "RoleEntitlements",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ModuleId",
                schema: "AppSecurity",
                table: "RoleEntitlements",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_RoleEntitlements",
                schema: "AppSecurity",
                table: "RoleEntitlements",
                column: "Id");

            // migrationBuilder.CreateTable(
            //     name: "Modules",
            //     columns: table => new
            //     {
            //         Id = table.Column<int>(type: "int", nullable: false)
            //             .Annotation("SqlServer:Identity", "1, 1"),
            //         ModuleName = table.Column<string>(type: "nvarchar(max)", nullable: false)
            //     },
            //     constraints: table =>
            //     {
            //         table.PrimaryKey("PK_Modules", x => x.Id);
            //     });

            // migrationBuilder.CreateTable(
            //     name: "Menus",
            //     columns: table => new
            //     {
            //         Id = table.Column<int>(type: "int", nullable: false)
            //             .Annotation("SqlServer:Identity", "1, 1"),
            //         MenuName = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //         ModuleId = table.Column<int>(type: "int", nullable: false)
            //     },
            //     constraints: table =>
            //     {
            //         table.PrimaryKey("PK_Menus", x => x.Id);
            //         table.ForeignKey(
            //             name: "FK_Menus_Modules_ModuleId",
            //             column: x => x.ModuleId,
            //             principalTable: "Modules",
            //             principalColumn: "Id",
            //             onDelete: ReferentialAction.Cascade);
            //     });

            // migrationBuilder.CreateIndex(
            //     name: "IX_RoleEntitlements_MenuId",
            //     schema: "AppSecurity",
            //     table: "RoleEntitlements",
            //     column: "MenuId");

            // migrationBuilder.CreateIndex(
            //     name: "IX_RoleEntitlements_ModuleId",
            //     schema: "AppSecurity",
            //     table: "RoleEntitlements",
            //     column: "ModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleEntitlements_RoleId",
                schema: "AppSecurity",
                table: "RoleEntitlements",
                column: "RoleId");

            // migrationBuilder.CreateIndex(
            //     name: "IX_Menus_ModuleId",
            //     table: "Menus",
            //     column: "ModuleId");

            // migrationBuilder.AddForeignKey(
            //     name: "FK_RoleEntitlements_Menus_MenuId",
            //     schema: "AppSecurity",
            //     table: "RoleEntitlements",
            //     column: "MenuId",
            //     principalTable: "Menus",
            //     principalColumn: "Id",
            //     onDelete: ReferentialAction.Restrict);

            // migrationBuilder.AddForeignKey(
            //     name: "FK_RoleEntitlements_Modules_ModuleId",
            //     schema: "AppSecurity",
            //     table: "RoleEntitlements",
            //     column: "ModuleId",
            //     principalTable: "Modules",
            //     principalColumn: "Id",
            //     onDelete: ReferentialAction.Restrict);

            // migrationBuilder.AddForeignKey(
            //     name: "FK_RoleEntitlements_UserRole_RoleId",
            //     schema: "AppSecurity",
            //     table: "RoleEntitlements",
            //     column: "RoleId",
            //     principalSchema: "AppSecurity",
            //     principalTable: "UserRole",
            //     principalColumn: "Id",
            //     onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoleEntitlements_Menus_MenuId",
                schema: "AppSecurity",
                table: "RoleEntitlements");

            migrationBuilder.DropForeignKey(
                name: "FK_RoleEntitlements_Modules_ModuleId",
                schema: "AppSecurity",
                table: "RoleEntitlements");

            migrationBuilder.DropForeignKey(
                name: "FK_RoleEntitlements_UserRole_RoleId",
                schema: "AppSecurity",
                table: "RoleEntitlements");

            migrationBuilder.DropTable(
                name: "Menus");

            migrationBuilder.DropTable(
                name: "UserRole",
                schema: "AppSecurity");

            migrationBuilder.DropTable(
                name: "Modules");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RoleEntitlements",
                schema: "AppSecurity",
                table: "RoleEntitlements");

            migrationBuilder.DropIndex(
                name: "IX_RoleEntitlements_MenuId",
                schema: "AppSecurity",
                table: "RoleEntitlements");

            migrationBuilder.DropIndex(
                name: "IX_RoleEntitlements_ModuleId",
                schema: "AppSecurity",
                table: "RoleEntitlements");

            migrationBuilder.DropIndex(
                name: "IX_RoleEntitlements_RoleId",
                schema: "AppSecurity",
                table: "RoleEntitlements");

            migrationBuilder.DropColumn(
                name: "CanAdd",
                schema: "AppSecurity",
                table: "RoleEntitlements");

            migrationBuilder.DropColumn(
                name: "CanDelete",
                schema: "AppSecurity",
                table: "RoleEntitlements");

            migrationBuilder.DropColumn(
                name: "CanExport",
                schema: "AppSecurity",
                table: "RoleEntitlements");

            migrationBuilder.DropColumn(
                name: "CanUpdate",
                schema: "AppSecurity",
                table: "RoleEntitlements");

            migrationBuilder.DropColumn(
                name: "CanView",
                schema: "AppSecurity",
                table: "RoleEntitlements");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "AppSecurity",
                table: "RoleEntitlements");

            migrationBuilder.DropColumn(
                name: "MenuId",
                schema: "AppSecurity",
                table: "RoleEntitlements");

            migrationBuilder.DropColumn(
                name: "ModuleId",
                schema: "AppSecurity",
                table: "RoleEntitlements");

            migrationBuilder.RenameTable(
                name: "RoleEntitlements",
                schema: "AppSecurity",
                newName: "RoleEntitlement",
                newSchema: "AppSecurity");

            migrationBuilder.RenameColumn(
                name: "CompanyId",
                schema: "AppData",
                table: "Department",
                newName: "CoId");

            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "AppData",
                table: "Department",
                newName: "DeptId");

            migrationBuilder.RenameColumn(
                name: "RoleId",
                schema: "AppSecurity",
                table: "RoleEntitlement",
                newName: "UserRoleId");

            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "AppSecurity",
                table: "RoleEntitlement",
                newName: "RoleEntitlementId");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedIP",
                schema: "AppSecurity",
                table: "RoleEntitlement",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedByName",
                schema: "AppSecurity",
                table: "RoleEntitlement",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                schema: "AppSecurity",
                table: "RoleEntitlement",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RoleEntitlement",
                schema: "AppSecurity",
                table: "RoleEntitlement",
                column: "RoleEntitlementId");

            migrationBuilder.CreateTable(
                name: "MenuPermission",
                schema: "AppSecurity",
                columns: table => new
                {
                    MenuPermissionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CanAdd = table.Column<bool>(type: "bit", nullable: false),
                    CanApprove = table.Column<bool>(type: "bit", nullable: false),
                    CanDelete = table.Column<bool>(type: "bit", nullable: false),
                    CanExport = table.Column<bool>(type: "bit", nullable: false),
                    CanUpdate = table.Column<bool>(type: "bit", nullable: false),
                    CanView = table.Column<bool>(type: "bit", nullable: false),
                    MenuName = table.Column<string>(type: "nvarchar(max)", nullable: false),
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

            migrationBuilder.CreateTable(
                name: "Role",
                schema: "AppSecurity",
                columns: table => new
                {
                    RoleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CoId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedByName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedIP = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<byte>(type: "tinyint", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<int>(type: "int", nullable: false),
                    ModifiedByName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedIP = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.RoleId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MenuPermission_RoleEntitlementId",
                schema: "AppSecurity",
                table: "MenuPermission",
                column: "RoleEntitlementId");
        }
    }
}
