using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyMovieWeb.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AlterTablesMovieEpisodeWatchHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CreatedDate",
                table: "Episodes",
                newName: "ReleaseDate");

            migrationBuilder.AddColumn<DateTime>(
                name: "LogDate",
                table: "WatchHistories",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LogDate",
                table: "WatchHistories");

            migrationBuilder.RenameColumn(
                name: "ReleaseDate",
                table: "Episodes",
                newName: "CreatedDate");
        }
    }
}
