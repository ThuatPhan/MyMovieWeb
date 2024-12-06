using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MyMovieWeb.Application.DTOs.Responses;
using MyMovieWeb.Application.Interfaces;
using MyMovieWeb.Domain.Entities;
using MyMovieWeb.Domain.Interfaces;

namespace MyMovieWeb.Application.Services
{
    public class StatisticServices : IStatisticServices
    {
        private readonly IRepository<WatchHistory> _watchHistoryRepo;
        private readonly IRepository<Order> _orderRepo;
        private readonly IRepository<Movie> _movieRepo;
        private readonly IMapper _mapper;

        public StatisticServices(
            IRepository<WatchHistory> watchHistoryRepository,
            IRepository<Order> orderRepository,
            IRepository<Movie> movieRepository,
            IMapper mapper
        )
        {
            _watchHistoryRepo = watchHistoryRepository;
            _movieRepo = movieRepository;
            _orderRepo = orderRepository;
            _mapper = mapper;
        }

        public async Task<Result<List<ViewChartDTO>>> GetMovieViewChartData(DateTime startDate, DateTime endDate, int numberOfMovie)
        {
            var query = _watchHistoryRepo
                .GetBaseQuery(wh => wh.LogDate >= startDate && wh.LogDate <= endDate)
                .GroupBy(wh => wh.MovieId)
                .Select(group => new
                {
                    MovieId = group.Key,
                    ViewCount = group.Count(),
                })
                .OrderBy(wh => wh.MovieId)
                .Take(numberOfMovie);

            var viewsData = await query.ToListAsync();
            List<int> movieIds = viewsData.Select(v => v.MovieId).ToList();

            IQueryable<Movie> movieQuery = _movieRepo.GetBaseQuery(predicate: m => movieIds.Contains(m.Id));

            var moviesData = await movieQuery.ToListAsync();

            List<ViewChartDTO> viewChartDTOs = moviesData.Select(m => new ViewChartDTO
            {
                Movie = _mapper.Map<MovieDTO>(m),
                ViewCount = viewsData.First(v => v.MovieId == m.Id).ViewCount,
            })
            .ToList();

            return Result<List<ViewChartDTO>>.Success(viewChartDTOs, "Chart view data retrieved successfully");
        }

        public async Task<Result<List<MovieRevenueChartDTO>>> GetMovieRevenueChartData(DateTime startDate, DateTime endDate, int numberOfMovie)
        {
            var query = _orderRepo
                .GetBaseQuery(predicate: o => o.CreatedDate >= startDate.Date && o.CreatedDate <= endDate.Date)
                .GroupBy(o => o.MovieId)
                .Select(group => new
                {
                    MovieId = group.Key,
                    TotalRevenue = group.Sum(group => group.TotalAmount)
                })
                .OrderBy(o => o.MovieId);

            var data = await query.Take(numberOfMovie).ToListAsync();
            var movieIds = data.Select(m => m.MovieId).ToList();

            var movieQuery = _movieRepo.GetBaseQuery(predicate: m => movieIds.Contains(m.Id));
            var movies = await movieQuery.ToListAsync();

            var revenuesData = movies.Select(m => new MovieRevenueChartDTO
            {
                Movie = _mapper.Map<MovieDTO>(m),
                TotalRevenue = data.First(d => d.MovieId == m.Id).TotalRevenue,
            })
            .ToList();

            return Result<List<MovieRevenueChartDTO>>.Success(revenuesData, "Movie revenues retrived successfully");

        }

        public async Task<Result<List<RevenueChartDTO>>> GetRevenueChartData(DateTime startDate, DateTime endDate)
        {
            var allDates = Enumerable.Range(0, (endDate.Date - startDate.Date).Days + 1)
                                     .Select(offset => startDate.Date.AddDays(offset))
                                     .ToList();

            var query = await _orderRepo
                .GetBaseQuery(predicate: o => o.CreatedDate >= startDate.Date && o.CreatedDate < endDate.AddDays(1))
                .GroupBy(o => o.CreatedDate.Date)
                .Select(group => new
                {
                    Date = group.Key,
                    TotalRevenue = group.Sum(g => g.TotalAmount),
                })
                .ToListAsync();

            var revenueData = allDates.GroupJoin(
                query,
                date => date,
                revenue => revenue.Date,
                (date, revenues) => new RevenueChartDTO
                {
                    Date = date,
                    TotalRevenue = revenues.FirstOrDefault()?.TotalRevenue ?? 0
                }
            ).ToList();

            return Result<List<RevenueChartDTO>>.Success(revenueData, "Revenues data retrieved successfully");
        }

    }
}
