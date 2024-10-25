using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMovieWeb.Application.DTOs.Responses
{
    public class MovieDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public string Director { get; set; }
        public List<string> Actors { get; set; }
        public string PosterUrl { get; set; }
        public List<MovieGenre> Genres { get; set; }
        public bool IsSeries { get; set; }
        public bool? IsSeriesCompleted { get; set; }
        public List<EpisodeDTO> Episodes { get; set; }
        public string? VideoUrl { get; set; }
        public int View { get; set; }
        public int RateCount { get; set; }
        public int RateTotal { get; set; }
        public bool IsShow { get; set; }
        public DateTime ReleaseDate { get; set; }
    }

    public class MovieGenre
    {
        public int GenreId { get; set; }
        public string GenreName { get; set; }
    }
}
