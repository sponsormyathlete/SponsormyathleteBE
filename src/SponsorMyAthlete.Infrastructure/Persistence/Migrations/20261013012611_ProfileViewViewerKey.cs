using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SponsorMyAthlete.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ProfileViewViewerKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ViewerKey",
                table: "ProfileViews",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileViews_AthleteProfileId_ViewerKey_ViewedAt",
                table: "ProfileViews",
                columns: new[] { "AthleteProfileId", "ViewerKey", "ViewedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ProfileViews_AthleteProfileId_ViewerKey_ViewedAt",
                table: "ProfileViews");

            migrationBuilder.DropColumn(
                name: "ViewerKey",
                table: "ProfileViews");
        }
    }
}
