using MyMovieWeb.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMovieWeb.Application.DTOs.Responses
{
    public class BlogPostDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public List<BlogPostTagDTO> Tags { get; set; }
        public string Thumbnail { get; set; }
        public bool IsShow { get; set; }
        public DateTime CreatedDate { get; set; } =  DateTime.UtcNow;
       
    }
}
