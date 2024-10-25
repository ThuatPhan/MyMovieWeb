using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMovieWeb.Domain.Entities
{
    public class Episode
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public int EpisodeNumber { get; set; }
        public string VideoUrl { get; set; }
        public bool IsShow { get; set; } = true;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public int MovieId { get; set; }
        public Movie Movie { get; set; }
    }
}
