namespace MyMovieWeb.Application.DTOs.Requests
{
    public class CreateMovieCommentRequestDTO
    {
        public string Content { get; set; }
        public int MovieId { get; set; }
    }
}
