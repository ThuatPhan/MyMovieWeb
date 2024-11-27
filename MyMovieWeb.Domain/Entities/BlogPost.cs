namespace MyMovieWeb.Domain.Entities
{
    public class BlogPost
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Thumbnail { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public ICollection<BlogPostTag> BlogPostTags { get; set; } = new List<BlogPostTag>();
    }
}
