using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HouseRentals.Migrations
{
    /// <inheritdoc />
    public partial class _3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPaid",
                table: "Rentals",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<double>(
                name: "TotalAmount",
                table: "Rentals",
                type: "DOUBLE",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPaid",
                table: "Rentals");

            migrationBuilder.DropColumn(
                name: "TotalAmount",
                table: "Rentals");
        }
    }
}
