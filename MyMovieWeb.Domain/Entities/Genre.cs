namespace MyMovieWeb.Domain.Entities
{
    public class Genre
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsShow { get; set; } = true;
        public ICollection<MovieGenre> MovieGenres { get; set; }
    }
}
