namespace MyMovieWeb.Application.DTOs.Requests
{
    public class WatchEpisodeRequestDTO
    {
        public int MovieId { get; set; }
        public int EpisodeId { get; set; }
        public int WatchingAt { get; set; }
    }
}
