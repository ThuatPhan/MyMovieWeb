namespace MyMovieWeb.Application.DTOs.Responses
{
    public class WatchHistoryDTO
    {
        public int Id { get; set; }
        public MovieDTO Movie { get; set; }
        public List<EpisodeDTO>? Episodes { get; set; }
        public EpisodeDTO? CurrentEpisode { get; set; }
        public TimeSpan CurrentWatching { get; set; }
    }
}
