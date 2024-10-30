namespace MyMovieWeb.Domain.Entities
{
    public class Episode
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public int EpisodeNumber { get; set; }
        public string VideoUrl { get; set; }
        public bool IsShow { get; set; } = true;
        public DateTime ReleaseDate { get; set; } = DateTime.UtcNow;
        public int MovieId { get; set; }
        public Movie Movie { get; set; }
    }
}
