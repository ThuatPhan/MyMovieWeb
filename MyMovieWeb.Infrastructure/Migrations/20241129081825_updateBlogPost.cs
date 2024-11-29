using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyMovieWeb.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updateBlogPost : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsShow",
                table: "BlogPosts",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsShow",
                table: "BlogPosts");
        }
    }
}
