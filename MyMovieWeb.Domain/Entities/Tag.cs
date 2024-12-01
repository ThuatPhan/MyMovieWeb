namespace MyMovieWeb.Domain.Entities
{
    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsShow { get; set; } = true;
        public ICollection<PostTags> PostTags { get; set; }
    }
}
