namespace MyMovieWeb.Domain.Entities
{
    public class Episode
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public int EpisodeNumber { get; set; }
        public string VideoUrl { get; set; }
        public string ThumbnailUrl { get; set; }
        public bool IsShow { get; set; } = true;
        public DateTime ReleaseDate { get; set; } = TimeZoneInfo.ConvertTimeFromUtc(
            DateTime.UtcNow,
            TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time")
        );
        public int MovieId { get; set; }
        public Movie Movie { get; set; }
    }
}
