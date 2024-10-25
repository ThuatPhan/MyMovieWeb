using MyMovieWeb.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMovieWeb.Domain.Interfaces
{
    public interface IEpisodeRepository : IRepository<Episode>
    {
        Task<IEnumerable<Episode>> GetEpisodeByMovieIdAsync(int movieId);
    }
}
