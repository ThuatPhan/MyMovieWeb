using System.ComponentModel.DataAnnotations;

namespace MyMovieWeb.Application.DTOs.Requests
{
    public class CreatePaymentRequest
    {
        [Range(19000, double.MaxValue, ErrorMessage = "Amount must be at least 19000 VND.")]
        public decimal Amount { get; set; }
        public Dictionary<string, string> Metadata { get; set; }
    }
}
