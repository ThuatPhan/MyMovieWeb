namespace MyMovieWeb.Domain.Entities
{
    public class Notification
    {
        public int Id { get; set; }
        public required string Message { get; set; }
        public string? Url { get; set; }
        public bool IsRead { get; set; } = false;
        public DateTime CreatedAt { get; set; } = TimeZoneInfo.ConvertTimeFromUtc(
            DateTime.UtcNow,
            TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time")
        );
        public required string UserId { get; set; }
    }
}
