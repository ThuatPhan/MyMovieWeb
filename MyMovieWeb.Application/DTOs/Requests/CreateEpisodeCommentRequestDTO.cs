namespace MyMovieWeb.Application.DTOs.Requests
{
    public class CreateEpisodeCommentRequestDTO
    {
        public string Content { get; set; }
        public int MovieId { get; set; }
        public int EpisodeId { get; set; }
    }
}
