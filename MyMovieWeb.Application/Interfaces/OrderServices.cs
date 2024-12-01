using MyMovieWeb.Domain.Entities;

namespace MyMovieWeb.Application.Interfaces
{
    public interface IOrderServices
    {
        Task CreateOrder(Order order);
        Task<Result<bool>> IsUserBoughtMovie(string userId, int movieId);
    }
}
