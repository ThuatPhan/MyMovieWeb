using MyMovieWeb.Domain.Entities;

namespace MyMovieWeb.Application.Interfaces
{
    public interface IOrderServices
    {
        Task CreateOrder(Order order);
        Task<Result<bool>> IsPurchasedMovie(string userId, int movieId);
        Task<Result<int>> CountPurchasedMovie(string userId);
    }
}
