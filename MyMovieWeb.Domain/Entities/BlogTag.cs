namespace MyMovieWeb.Domain.Entities
{
    public class BlogTag
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<BlogPostTag> BlogPostTags { get; set; }
    }
}
