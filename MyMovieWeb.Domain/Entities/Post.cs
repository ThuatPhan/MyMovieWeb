namespace MyMovieWeb.Domain.Entities
{
    public class Post
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Thumbnail { get; set; }
        public bool IsShow { get; set; } = true;
        public DateTime CreatedDate { get; set; } = TimeZoneInfo.ConvertTimeFromUtc(
            DateTime.UtcNow,
            TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time")
        );
        public ICollection<PostTags> PostTags { get; set; } = new List<PostTags>();
    }
}
