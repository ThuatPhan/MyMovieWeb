using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyMovieWeb.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AlterTableMovies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPaid",
                table: "Movies",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "Movies",
                type: "decimal(18,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPaid",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "Movies");
        }
    }
}
