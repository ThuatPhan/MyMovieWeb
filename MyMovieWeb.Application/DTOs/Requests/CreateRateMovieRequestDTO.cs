using System.ComponentModel.DataAnnotations;

namespace MyMovieWeb.Application.DTOs.Requests
{
    public class CreateRateMovieRequestDTO
    {
        public int MovieId { get; set; }
        [Range(1, 5, ErrorMessage = "Rate total must be in range 1 - 5")]
        public int RateTotal { get; set; }
    }
}
