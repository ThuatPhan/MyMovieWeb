namespace MyMovieWeb.Application.DTOs.Responses
{
    public class PostDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public List<PostTagDTO> Tags { get; set; }
        public string Thumbnail { get; set; }
        public bool IsShow { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}
