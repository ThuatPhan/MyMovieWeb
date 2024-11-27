namespace MyMovieWeb.Domain.Entities
{
    public class Movie
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public bool IsPaid { get; set; } = false;
        public decimal? Price { get; set; }
        public string? Description { get; set; }
        public string Director { get; set; }
        public string Actors { get; set; }
        public string PosterUrl { get; set; }
        public string BannerUrl { get; set; }
        public bool IsSeries { get; set; } = false;
        public bool? IsSeriesCompleted { get; set; }
        public string? VideoUrl { get; set; }
        public int View { get; set; } = 0;
        public int RateCount { get; set; } = 0;
        public int RateTotal { get; set; } = 0;
        public bool IsShow { get; set; } = true;
        public DateTime ReleaseDate { get; set; } = TimeZoneInfo.ConvertTimeFromUtc(
            DateTime.UtcNow,
            TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time")
        );
        public ICollection<Episode> Episodes { get; set; }
        public ICollection<MovieGenre> MovieGenres { get; set; } = new List<MovieGenre>();
    }
}
