using MyMovieWeb.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMovieWeb.Domain.Interfaces
{
    public interface IMovieRepository : IRepository<Movie>
    {
        Task<Movie?> GetByIdIncludeGenresAsync(int id);
        Task<IEnumerable<Movie>> GetAllIncludeGenresAsync();
    }
}
