using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SponsorMyAthlete.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddDealDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "AdminNotes",
                table: "Deals",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "Deals",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SponsorshipPackageId",
                table: "Deals",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Deals",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Deals_AthleteUserId_SponsorUserId",
                table: "Deals",
                columns: new[] { "AthleteUserId", "SponsorUserId" });

            migrationBuilder.CreateIndex(
                name: "IX_Deals_Status",
                table: "Deals",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Deals_AthleteUserId_SponsorUserId",
                table: "Deals");

            migrationBuilder.DropIndex(
                name: "IX_Deals_Status",
                table: "Deals");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "Deals");

            migrationBuilder.DropColumn(
                name: "SponsorshipPackageId",
                table: "Deals");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Deals");

            migrationBuilder.AlterColumn<string>(
                name: "AdminNotes",
                table: "Deals",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(2000)",
                oldMaxLength: 2000,
                oldNullable: true);
        }
    }
}
