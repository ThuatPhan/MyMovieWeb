namespace MyMovieWeb.Application.DTOs.Responses
{
    public class CommentDTO
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public Auth0UserDTO User { get; set; }
        public int MovieId { get; set; }
        public int? EpisodeId { get; set; }
        public string UserId { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
