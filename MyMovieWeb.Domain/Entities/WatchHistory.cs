namespace MyMovieWeb.Domain.Entities
{
    public class WatchHistory
    {
        public int Id { get; set; }
        public TimeSpan CurrentWatching { get; set; }
        public string UserId { get; set; }
        public int MovieId { get; set; }
        public int? EpisodeId { get; set; }
        public bool IsWatched { get; set; } = false;
        public Movie Movie { get; set; }
        public DateTime LogDate { get; set; } = TimeZoneInfo.ConvertTimeFromUtc(
            DateTime.UtcNow,
            TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time")
        );
    }
}
