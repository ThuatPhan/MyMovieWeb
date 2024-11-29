using Microsoft.AspNetCore.Http;

namespace MyMovieWeb.Application.DTOs.Requests
{
    public class CreateBlogRequestDTO
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public List<int> TagIds { get; set; }
        public bool? IsShow { get; set; } = true;

        public required IFormFile Thumbnail { get; set; }
    }
}
