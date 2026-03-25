using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IamPlatform.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixInvitationRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invitations_Users_TenantId",
                table: "Invitations");

            migrationBuilder.AddForeignKey(
                name: "FK_Invitations_Tenants_TenantId",
                table: "Invitations",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invitations_Tenants_TenantId",
                table: "Invitations");

            migrationBuilder.AddForeignKey(
                name: "FK_Invitations_Users_TenantId",
                table: "Invitations",
                column: "TenantId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
