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
        public DbSet<Post> Posts { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<PostTags> PostTags { get; set; }
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

            modelBuilder.Entity<PostTags>()
                .HasKey(mg => new { mg.PostId, mg.TagId });

            modelBuilder.Entity<PostTags>()
                .HasOne(mg => mg.Post)
                .WithMany(m => m.PostTags)
                .HasForeignKey(mg => mg.PostId);

            modelBuilder.Entity<PostTags>()
                .HasOne(mg => mg.Tag)
                .WithMany(g => g.PostTags)
                .HasForeignKey(mg => mg.TagId);
        }
    }
}
