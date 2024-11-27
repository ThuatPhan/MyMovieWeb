namespace MyMovieWeb.Domain.Entities
{
    public class BlogPostTag
    {
        public int BlogPostId { get; set; }
        public int BlogTagId { get; set; }
        public BlogPost BlogPost { get; set; }
        public BlogTag BlogTag { get; set; }
    }
}
