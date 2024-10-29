using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMovieWeb.Application.DTOs.Responses
{
    public class EpisodeDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public int EpisodeNumber { get; set; }
        public string VideoUrl { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsShow { get; set; }
        public int MovieId { get; set; }
    }
}
