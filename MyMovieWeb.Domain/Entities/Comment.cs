using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
