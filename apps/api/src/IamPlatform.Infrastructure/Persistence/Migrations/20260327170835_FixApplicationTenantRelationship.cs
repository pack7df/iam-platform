using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IamPlatform.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixApplicationTenantRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Applications_Users_TenantId",
                table: "Applications");

            migrationBuilder.AddForeignKey(
                name: "FK_Applications_Tenants_TenantId",
                table: "Applications",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Applications_Tenants_TenantId",
                table: "Applications");

            migrationBuilder.AddForeignKey(
                name: "FK_Applications_Users_TenantId",
                table: "Applications",
                column: "TenantId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
