namespace MyMovieWeb.Application.DTOs.Responses
{
    public class NotificationDTO
    {
        public int Id { get; set; }
        public required string Message { get; set; }
        public string? Url { get; set; }
        public bool IsRead { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
