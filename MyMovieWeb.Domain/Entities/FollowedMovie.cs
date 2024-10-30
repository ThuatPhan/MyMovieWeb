namespace MyMovieWeb.Domain.Entities
{
    public class FollowedMovie
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int MovieId { get; set; }
        public Movie Movie { get; set; }
    }
}
