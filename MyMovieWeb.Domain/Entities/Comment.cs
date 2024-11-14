namespace MyMovieWeb.Domain.Entities
{
    public class Comment
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public string UserId { get; set; }
        public int MovieId { get; set; }
        public int? EpisodeId { get; set; }
        public Movie Movie { get; set; }
        public Episode Episode { get; set; }
        public DateTime CreatedDate { get; set; } = TimeZoneInfo.ConvertTimeFromUtc(
            DateTime.UtcNow, 
            TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time")
        );
    }
}
