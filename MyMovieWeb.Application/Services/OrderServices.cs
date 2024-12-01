using MyMovieWeb.Application.Interfaces;
using MyMovieWeb.Domain.Entities;
using MyMovieWeb.Domain.Interfaces;

namespace MyMovieWeb.Application.Services
{
    public class OrderServices : IOrderServices
    {
        private readonly IRepository<Order> _orderRepo;

        public OrderServices(IRepository<Order> orderRepository)
        {
            _orderRepo = orderRepository;
        }

        public async Task CreateOrder(Order order)
        {
            await _orderRepo.AddAsync(order);
        }

        public async Task<Result<bool>> IsUserBoughtMovie(string userId, int movieId)
        {
            var result = await _orderRepo.FindOneAsync(o => o.UserId == userId && o.MovieId == movieId);
            return Result<bool>.Success(result != null, "Bought status retrieved successfully");
        }
    }
}
