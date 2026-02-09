using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HouseRentals.Migrations
{
    /// <inheritdoc />
    public partial class _5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "available",
                table: "Houses",
                newName: "Available");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Available",
                table: "Houses",
                newName: "available");
        }
    }
}
