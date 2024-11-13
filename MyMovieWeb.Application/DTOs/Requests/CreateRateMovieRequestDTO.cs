using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMovieWeb.Application.DTOs.Requests
{
    public class CreateRateMovieRequestDTO
    {
        public int Id { get; set; }
        public int RateTotal { get; set; }
    }
}
