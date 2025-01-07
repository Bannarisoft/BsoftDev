using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BSOFT.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UserListAddUserRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoleEntitlements_UserRole_RoleId",
                schema: "AppSecurity",
                table: "RoleEntitlements");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_UserRole_UserRoleId",
                schema: "AppSecurity",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "RoleId",
                schema: "AppSecurity",
                table: "RoleEntitlements",
                newName: "UserRoleId");

            migrationBuilder.RenameIndex(
                name: "IX_RoleEntitlements_RoleId",
                schema: "AppSecurity",
                table: "RoleEntitlements",
                newName: "IX_RoleEntitlements_UserRoleId");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                schema: "AppSecurity",
                table: "UserRole",
                type: "int",
                nullable: false,
                defaultValue: 0);  

            migrationBuilder.CreateIndex(
                name: "IX_UserRole_UserId",
                schema: "AppSecurity",
                table: "UserRole",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_RoleEntitlements_UserRole_UserRoleId",
                schema: "AppSecurity",
                table: "RoleEntitlements",
                column: "UserRoleId",
                principalSchema: "AppSecurity",
                principalTable: "UserRole",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRole_Users_UserId",
                schema: "AppSecurity",
                table: "UserRole",
                column: "UserId",
                principalSchema: "AppSecurity",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {   

            migrationBuilder.DropForeignKey(
                name: "FK_RoleEntitlements_UserRole_UserRoleId",
                schema: "AppSecurity",
                table: "RoleEntitlements");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRole_Users_UserId",
                schema: "AppSecurity",
                table: "UserRole");
           
            migrationBuilder.DropIndex(
                name: "IX_UserRole_UserId",
                schema: "AppSecurity",
                table: "UserRole");

            migrationBuilder.DropColumn(
                name: "UserId",
                schema: "AppSecurity",
                table: "UserRole");

            migrationBuilder.RenameColumn(
                name: "UserRoleId",
                schema: "AppSecurity",
                table: "RoleEntitlements",
                newName: "RoleId");

            migrationBuilder.RenameIndex(
                name: "IX_RoleEntitlements_UserRoleId",
                schema: "AppSecurity",
                table: "RoleEntitlements",
                newName: "IX_RoleEntitlements_RoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_RoleEntitlements_UserRole_RoleId",
                schema: "AppSecurity",
                table: "RoleEntitlements",
                column: "RoleId",
                principalTable: "UserRole",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_UserRole_UserRoleId",
                schema: "AppSecurity",
                table: "Users",
                column: "UserRoleId",
                principalTable: "UserRole",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
