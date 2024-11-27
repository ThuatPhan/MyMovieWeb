using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyMovieWeb.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AlterTableWatchHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsWatched",
                table: "WatchHistories",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsWatched",
                table: "WatchHistories");
        }
    }
}
