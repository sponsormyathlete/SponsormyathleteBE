using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SponsorMyAthlete.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SponsorOnboardingCompletion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "OnboardingCompletedAt",
                table: "SponsorProfiles",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OnboardingCompletedAt",
                table: "SponsorProfiles");
        }
    }
}
