using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WeatherSubscriptionWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CountryCodeField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CountryCode",
                table: "Subscriptions",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CountryCode",
                table: "Subscriptions");
        }
    }
}
