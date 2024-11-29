using Microsoft.EntityFrameworkCore;
using MyMovieWeb.Domain.Entities;

namespace MyMovieWeb.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<MovieGenre> MovieGenres { get; set; }
        public DbSet<Episode> Episodes { get; set; }
        public DbSet<FollowedMovie> FollowedMovies { get; set; }
        public DbSet<WatchHistory> WatchHistories { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<BlogPost> BlogPosts { get; set; }
        public DbSet<BlogTag> BlogTags { get; set; }
        public DbSet<BlogPostTag> BlogPostTags { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure many-to-many relationship
            modelBuilder.Entity<MovieGenre>()
                .HasKey(mg => new { mg.MovieId, mg.GenreId });

            modelBuilder.Entity<MovieGenre>()
                .HasOne(mg => mg.Movie)
                .WithMany(m => m.MovieGenres)
                .HasForeignKey(mg => mg.MovieId);

            modelBuilder.Entity<MovieGenre>()
                .HasOne(mg => mg.Genre)
                .WithMany(g => g.MovieGenres)
                .HasForeignKey(mg => mg.GenreId);

            modelBuilder.Entity<BlogPostTag>()
                .HasKey(mg => new { mg.BlogPostId, mg.BlogTagId });

            modelBuilder.Entity<BlogPostTag>()
                .HasOne(mg => mg.BlogPost)
                .WithMany(m => m.BlogPostTags)
                .HasForeignKey(mg => mg.BlogPostId);

            modelBuilder.Entity<BlogPostTag>()
                .HasOne(mg => mg.BlogTag)
                .WithMany(g => g.BlogPostTags)
                .HasForeignKey(mg => mg.BlogTagId);
        }
    }
}
