namespace MyMovieWeb.Domain.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public string SessionId { get; set; }
        public string UserId { get; set; }
        public int MovieId { get; set; }
        public DateTime CreatedDate { get; set; }
        public Movie Movie { get; set; }
    }
}
