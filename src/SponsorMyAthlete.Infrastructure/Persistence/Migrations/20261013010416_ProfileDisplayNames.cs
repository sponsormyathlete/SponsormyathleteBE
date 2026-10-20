using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SponsorMyAthlete.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ProfileDisplayNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "SponsorProfiles",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "AthleteProfiles",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "SponsorProfiles");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "AthleteProfiles");
        }
    }
}
