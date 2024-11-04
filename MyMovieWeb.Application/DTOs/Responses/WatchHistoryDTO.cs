namespace MyMovieWeb.Application.DTOs.Responses
{
    public class WatchHistoryDTO
    {
        public int MovieId { get; set; }
        public int? EpisodeId { get; set; }
        public MovieDTO Movie { get; set; }
        public TimeSpan CurrentWatching { get; set; }
    }
}
