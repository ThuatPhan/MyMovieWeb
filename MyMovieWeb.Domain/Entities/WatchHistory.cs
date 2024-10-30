namespace MyMovieWeb.Domain.Entities
{
    public class WatchHistory
    {
        public int Id { get; set; }
        public TimeSpan CurrentWatching { get; set; }
        public string UserId { get; set; }
        public int MovieId { get; set; }
        public int? EpisodeId { get; set; }
        public Movie Movie { get; set; }
        public DateTime LogDate { get; set; } = DateTime.UtcNow;
    }
}
