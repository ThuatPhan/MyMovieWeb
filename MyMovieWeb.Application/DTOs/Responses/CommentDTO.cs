using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMovieWeb.Application.DTOs.Responses
{
    public class CommentDTO
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public int MovieId { get; set; }
        public int? EpisodeId { get; set; }
    }
}
