using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MyMovieWeb.Application.DTOs.Responses;
using MyMovieWeb.Application.Interfaces;
using MyMovieWeb.Domain.Entities;
using MyMovieWeb.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMovieWeb.Application.Services
{
    public class StatisticServices : IStatisticServices
    {
        private readonly IRepository<WatchHistory> _watchHistoryRepo;
        private readonly IRepository<Movie> _movieRepo;
        private readonly IMapper _mapper;

        public StatisticServices(IRepository<WatchHistory> watchHistoryRepository, IRepository<Movie> movieRepository, IMapper mapper)
        {
            _watchHistoryRepo = watchHistoryRepository;
            _movieRepo = movieRepository;
            _mapper = mapper;
        }

        public async Task<Result<List<ViewChartDTO>>> GetChartMovieViewData(DateTime startDate, DateTime endDate, int numberOfMovie)
        {
            var query = _watchHistoryRepo.GetBaseQuery(wh => wh.LogDate >= startDate && wh.LogDate <= endDate)
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
                ViewCount = viewsData.First(v => v.MovieId ==  m.Id).ViewCount,
            })
            .ToList();

            return Result<List<ViewChartDTO>>.Success(viewChartDTOs, "Chart view data retrieved successfully");
        }
    }
}
