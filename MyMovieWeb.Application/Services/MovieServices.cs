using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MyMovieWeb.Application.DTOs.Requests;
using MyMovieWeb.Application.DTOs.Responses;
using MyMovieWeb.Application.Interfaces;
using MyMovieWeb.Application.Utils;
using MyMovieWeb.Domain.Entities;
using MyMovieWeb.Domain.Interfaces;
using System.Linq.Expressions;

namespace MyMovieWeb.Application.Services
{
    public class MovieServices : IMovieService
    {
        private readonly IMapper _mapper;
        private readonly IRepository<Genre> _genreRepo;
        private readonly IRepository<Movie> _movieRepo;
        private readonly IRepository<Comment> _commentRepo;
        private readonly IRepository<Order> _orderRepo;
        private readonly IRepository<FollowedMovie> _followedMovieRepo;
        private readonly IRepository<WatchHistory> _watchHistoryRepo;
        private readonly IEpisodeServices _episodeServices;
        private readonly IS3Services _s3Services;

        public MovieServices(
            IMapper mapper,
            IRepository<Genre> genreRepository,
            IRepository<Movie> movieRepo,
            IRepository<Comment> commentRepository,
            IRepository<FollowedMovie> followedMovieRepository,
            IRepository<WatchHistory> watchHistoryRepository,
            IEpisodeServices episodeServices,
            IS3Services s3Services,
            IRepository<Order> orderRepository
        )
        {
            _mapper = mapper;
            _genreRepo = genreRepository;
            _movieRepo = movieRepo;
            _commentRepo = commentRepository;
            _followedMovieRepo = followedMovieRepository;
            _watchHistoryRepo = watchHistoryRepository;
            _episodeServices = episodeServices;
            _s3Services = s3Services;
            _orderRepo = orderRepository;
        }

        public async Task<Result<MovieDTO>> CreateMovie(CreateMovieRequestDTO movieRequestDTO)
        {
            Movie movieToCreate = _mapper.Map<Movie>(movieRequestDTO);

            HashSet<int> existingGenreIds = (await _genreRepo.GetAllAsync())
                .Select(g => g.Id)
                .ToHashSet();

            List<MovieGenre> invalidGenres = movieToCreate.MovieGenres
                .Where(genre => !existingGenreIds.Contains(genre.GenreId))
                .ToList();

            if (invalidGenres.Any())
            {
                String invalidGenreIds = string.Join(", ", invalidGenres.Select(g => g.GenreId));
                return Result<MovieDTO>.Failure($"Genre id(s) {invalidGenreIds} are not defined");
            }

            string posterUrl = await _s3Services.UploadFileAsync(movieRequestDTO.PosterFile);
            movieToCreate.PosterUrl = posterUrl;

            string bannerUrl = await _s3Services.UploadFileAsync(movieRequestDTO.BannerFile);
            movieToCreate.BannerUrl = bannerUrl;

            if (!movieToCreate.IsSeries && movieRequestDTO.VideoFile != null)
            {
                string videoUrl = await _s3Services.UploadFileAsync(movieRequestDTO.VideoFile);
                movieToCreate.VideoUrl = videoUrl;
            }

            Movie createdMovie = await _movieRepo.AddAsync(movieToCreate);
            MovieDTO movieDTO = _mapper.Map<MovieDTO>(createdMovie);

            return Result<MovieDTO>.Success(movieDTO, "Movie created successfully");
        }

        public async Task<Result<MovieDTO>> UpdateMovie(int id, UpdateMovieRequestDTO movieRequestDTO)
        {
            IQueryable<Movie> query = _movieRepo.GetBaseQuery(predicate: m => m.Id == id)
                .Include(m => m.MovieGenres)
                    .ThenInclude(mg => mg.Genre);

            Movie? movieToUpdate = await query.FirstOrDefaultAsync();

            if (movieToUpdate is null)
            {
                return Result<MovieDTO>.Failure($"Movie id {id} not found");
            }

            HashSet<int> existingGenreIds = (await _genreRepo.GetAllAsync())
                .Select(g => g.Id)
                .ToHashSet();

            List<int> invalidGenres = movieRequestDTO.GenreIds
                .Where(genreId => !existingGenreIds.Contains(genreId))
                .ToList();

            if (invalidGenres.Any())
            {
                string invalidGenreIds = string.Join(", ", invalidGenres.Select(g => g).ToList());
                return Result<MovieDTO>.Failure($"Genre id(s) {invalidGenreIds} are not defined");
            }

            if (movieRequestDTO.PosterFile != null)
            {
                string posterUrl = await _s3Services.UploadFileAsync(movieRequestDTO.PosterFile);
                movieToUpdate.PosterUrl = posterUrl;
            }

            if (movieRequestDTO.BannerFile != null)
            {
                string bannerUrl = await _s3Services.UploadFileAsync(movieRequestDTO.BannerFile);
                movieToUpdate.BannerUrl = bannerUrl;
            }

            if (!movieToUpdate.IsSeries && movieRequestDTO.VideoFile != null)
            {
                string videoUrl = await _s3Services.UploadFileAsync(movieRequestDTO.VideoFile);
                movieToUpdate.VideoUrl = videoUrl;
            }

            _mapper.Map(movieRequestDTO, movieToUpdate);

            await _movieRepo.UpdateAsync(movieToUpdate);

            Movie? updatedMovie = await query
                .Include(m => m.MovieGenres)
                    .ThenInclude(mg => mg.Genre)
                    .FirstOrDefaultAsync();

            MovieDTO movieDTO = _mapper.Map<MovieDTO>(updatedMovie);

            return Result<MovieDTO>.Success(movieDTO, "Movie updated successfully");
        }

        public async Task<Result<bool>> DeleteMovie(int id)
        {
            Movie? movieToDelete = await _movieRepo.GetByIdAsync(id);
            if (movieToDelete is null)
            {
                return Result<bool>.Failure($"Movie id {id} not found");
            }

            var deleteDataTasks = Task.WhenAll(
                _commentRepo.RemoveRangeAsync(wh => wh.MovieId == id),
                _watchHistoryRepo.RemoveRangeAsync(wh => wh.MovieId == id),
                _followedMovieRepo.RemoveRangeAsync(wh => wh.MovieId == id),
                _orderRepo.RemoveRangeAsync(o => o.MovieId == id)
            ); 

            await _episodeServices.DeleteEpisodeOfMovie(id);

            var deleteFileTasks = Task.WhenAll(
                _s3Services.DeleteFileAsync(movieToDelete.PosterUrl),
                _s3Services.DeleteFileAsync(movieToDelete.BannerUrl)
            );

            await deleteDataTasks;
            await deleteFileTasks;

            if (movieToDelete.VideoUrl != null)
            {
                await _s3Services.DeleteFileAsync(movieToDelete.VideoUrl);
            }

            await _movieRepo.RemoveAsync(movieToDelete);

            return Result<bool>.Success(true, "Movie deleted successfully");
        }

        public async Task<Result<int>> CountMovieBy(Expression<Func<Movie, bool>> predicate)
        {
            int totalCount = await _movieRepo.CountAsync(predicate);
            return Result<int>.Success(totalCount, "Total movie count retrieved successfully");
        }

        public async Task<Result<MovieDTO>> GetMovieById(int id)
        {
            IQueryable<Movie> query = _movieRepo.GetBaseQuery(predicate: m => m.Id == id)
                .Include(m => m.MovieGenres)
                    .ThenInclude(mg => mg.Genre);

            Movie? movie = await query
                .Select(m => new Movie
                {
                    Id = m.Id,
                    Title = m.Title,
                    IsPaid = m.IsPaid,
                    Price = m.Price,
                    Description = m.Description,
                    Director = m.Director,
                    Actors = m.Actors,
                    PosterUrl = m.PosterUrl,
                    BannerUrl = m.BannerUrl,
                    IsSeries = m.IsSeries,
                    IsSeriesCompleted = m.IsSeriesCompleted,
                    VideoUrl = m.VideoUrl,
                    View = m.View,
                    RateCount = m.RateCount,
                    RateTotal = m.RateTotal,
                    IsShow = m.IsShow,
                    ReleaseDate = m.ReleaseDate,
                    Episodes = m.Episodes,
                    MovieGenres = m.MovieGenres.Where(mg => mg.Genre.IsShow).ToList()
                })
                .FirstOrDefaultAsync();

            if (movie is null)
            {
                return Result<MovieDTO>.Failure($"Movie id {id} not found");
            }

            MovieDTO movieDTO = _mapper.Map<MovieDTO>(movie);
            movieDTO.CommentCount = await _commentRepo.CountAsync(c => c.MovieId == id);

            return Result<MovieDTO>.Success(movieDTO, "Movie retrieved successfully");
        }

        public async Task<Result<List<MovieDTO>>> FindAllMovies(
            int pageNumber,
            int pageSize,
            Expression<Func<Movie, bool>> predicate,
            Func<IQueryable<Movie>, IOrderedQueryable<Movie>>? orderBy = null
        )
        {
            IQueryable<Movie> query = _movieRepo.GetBaseQuery(predicate)
                .Include(m => m.MovieGenres)
                    .ThenInclude(mg => mg.Genre);


            if (orderBy != null)
            {
                query = orderBy(query);
            }

            IEnumerable<Movie> movies = await query
                .Select(m => new Movie
                {
                    Id = m.Id,
                    Title = m.Title,
                    IsPaid = m.IsPaid,
                    Price = m.Price,
                    Description = m.Description,
                    Director = m.Director,
                    Actors = m.Actors,
                    PosterUrl = m.PosterUrl,
                    BannerUrl = m.BannerUrl,
                    IsSeries = m.IsSeries,
                    IsSeriesCompleted = m.IsSeriesCompleted,
                    VideoUrl = m.VideoUrl,
                    View = m.View,
                    RateCount = m.RateCount,
                    RateTotal = m.RateTotal,
                    IsShow = m.IsShow,
                    ReleaseDate = m.ReleaseDate,
                    Episodes = m.Episodes,
                    MovieGenres = m.MovieGenres.Where(mg => mg.Genre.IsShow).ToList()
                })
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            List<MovieDTO> movieDTOs = _mapper.Map<List<MovieDTO>>(movies);

            var commentData = _commentRepo.GetBaseQuery(_ => true)
                .GroupBy(c => c.MovieId)
                .Select(group => new
                {
                    MovieId = group.Key,
                    CommentCount = group.Count(),
                })
                .ToList();


            foreach (var movieDTO in movieDTOs)
            {
                int? commentCount = commentData.FirstOrDefault(c => c.MovieId == movieDTO.Id)?.CommentCount;
                movieDTO.CommentCount = commentCount ?? 0;
            }

            return Result<List<MovieDTO>>.Success(movieDTOs, "Movies retrieved successfully");
        }

        public async Task<Result<List<MovieDTO>>> GetMoviesSameGenreOfMovie(int movieId, int pageNumber, int pageSize)
        {
            IQueryable<Movie> query = _movieRepo.GetBaseQuery(predicate: m => m.Id == movieId);

            Movie? movie = await query
                .Include(m => m.MovieGenres)
                    .ThenInclude(mg => mg.Genre)
                    .FirstOrDefaultAsync();

            if (movie is null)
            {
                return Result<List<MovieDTO>>.Failure($"Movie id {movieId} not found");
            }

            List<int> genreIds = movie.MovieGenres.Select(mg => mg.GenreId).ToList();

            IQueryable<Movie> moviesQuery = _movieRepo
                .GetBaseQuery(
                    predicate: m => m.Id != movieId && m.IsShow == true && m.MovieGenres.Any(mg => genreIds.Contains(mg.GenreId))
                );

            IEnumerable<Movie> movies = await moviesQuery
                .Include(m => m.MovieGenres)
                    .ThenInclude(mg => mg.Genre)
                .Select(m => new Movie
                {
                    Id = m.Id,
                    Title = m.Title,
                    IsPaid = m.IsPaid,
                    Price = m.Price,
                    Description = m.Description,
                    Director = m.Director,
                    Actors = m.Actors,
                    PosterUrl = m.PosterUrl,
                    BannerUrl = m.BannerUrl,
                    IsSeries = m.IsSeries,
                    IsSeriesCompleted = m.IsSeriesCompleted,
                    VideoUrl = m.VideoUrl,
                    View = m.View,
                    RateCount = m.RateCount,
                    RateTotal = m.RateTotal,
                    IsShow = m.IsShow,
                    ReleaseDate = m.ReleaseDate,
                    Episodes = m.Episodes,
                    MovieGenres = m.MovieGenres.Where(mg => mg.Genre.IsShow).ToList()

                })
                .OrderBy(m => m.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            List<MovieDTO> movieDTOs = _mapper.Map<List<MovieDTO>>(movies);

            return Result<List<MovieDTO>>.Success(movieDTOs, "Movies retrieved successfully");
        }

        public async Task<Result<bool>> IncreaseView(int id, int view)
        {
            Movie? movie = await _movieRepo.GetByIdAsync(id);
            if (movie is null)
            {
                return Result<bool>.Failure($"Movie id {id} not found");
            }
            movie.View += view;
            await _movieRepo.UpdateAsync(movie);

            return Result<bool>.Success(true, "Increase view for movie successfully");
        }

        public async Task<Result<bool>> CreateRate(CreateRateMovieRequestDTO rateMovieRequestDTO)
        {
            Movie? movieToRate = await _movieRepo.GetByIdAsync(rateMovieRequestDTO.MovieId);
            if (movieToRate is null)
            {
                return Result<bool>.Failure($"Movie id {rateMovieRequestDTO.MovieId} not found");
            }
            movieToRate.RateCount += 1;
            movieToRate.RateTotal += rateMovieRequestDTO.RateTotal;

            await _movieRepo.UpdateAsync(movieToRate);

            return Result<bool>.Success(true, "Rate created successfully");
        }

        public async Task<Result<List<MovieDTO>>> GetTopView(TimePeriod timePeriod, int topCount)
        {
            DateTime startDate, endDate;
            switch (timePeriod)
            {
                case TimePeriod.Today:
                    startDate = DateTime.Today;
                    endDate = DateTime.Today.AddDays(1).AddTicks(-1);
                    break;

                case TimePeriod.ThisWeek:
                    startDate = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
                    endDate = startDate.AddDays(7).AddTicks(-1);
                    break;

                case TimePeriod.ThisMonth:
                    startDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                    endDate = startDate.AddMonths(1).AddTicks(-1);
                    break;

                case TimePeriod.ThisYear:
                    startDate = new DateTime(DateTime.Today.Year, 1, 1);
                    endDate = startDate.AddYears(1).AddTicks(-1);
                    break;

                default:
                    throw new NotSupportedException("Unsupported TimePeriod");
            }

            var query = _watchHistoryRepo
                .GetBaseQuery(wh => wh.LogDate >= startDate && wh.LogDate <= endDate)
                .GroupBy(wh => wh.MovieId)
                .Select(group => new
                {
                    MovieId = group.Key,
                    ViewCount = group.Count()
                })
                .OrderByDescending(group => group.ViewCount)
                .Take(topCount);
            var topMoviesData = await query
                .ToListAsync();

            var movieIds = topMoviesData
                .Select(m => m.MovieId)
                .ToList();

            var topViewQuery = _movieRepo.GetBaseQuery(predicate: m => movieIds.Contains(m.Id) && m.IsShow);
            var topMovies = await topViewQuery.ToListAsync();

            topMovies = topMovies.OrderBy(m => movieIds.IndexOf(m.Id)).ToList();

            List<MovieDTO> movieDTOs = _mapper.Map<List<MovieDTO>>(topMovies);

            return Result<List<MovieDTO>>.Success(movieDTOs, "Top view retrieved successfully");
        }

        public async Task<Result<int>> CountTredingInDay()
        {
            DateTime startDate, endDate;
            startDate = DateTime.Today;
            endDate = DateTime.Today.AddDays(1).AddTicks(-1);

            var query = _watchHistoryRepo
                .GetBaseQuery(wh => wh.LogDate >= startDate && wh.LogDate <= endDate)
                .GroupBy(wh => wh.MovieId)
                .Select(group => new
                {
                    MovieId = group.Key,
                    ViewCount = group.Count()
                })
                .OrderByDescending(group => group.ViewCount);

            var trendingMovies = await query.ToListAsync();
            int totalCount = trendingMovies.Count();
            return Result<int>.Success(totalCount, "Trending movies count successfully");
        }

        public async Task<Result<List<MovieDTO>>> GetTrendingInDay(int pageNumber, int pageSize)
        {
            DateTime startDate, endDate;
            startDate = DateTime.Today;
            endDate = DateTime.Today.AddDays(1).AddTicks(-1);

            var query = _watchHistoryRepo
                .GetBaseQuery(wh => wh.LogDate >= startDate && wh.LogDate <= endDate)
                .GroupBy(wh => wh.MovieId)
                .Select(group => new
                {
                    MovieId = group.Key,
                    ViewCount = group.Count()
                })
                .OrderByDescending(group => group.ViewCount)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            var trendingMoviesData = await query.ToListAsync();

            List<int> movieIds = trendingMoviesData.Select(m => m.MovieId).ToList();

            IEnumerable<Movie> trendingMovies = await _movieRepo
                .GetBaseQuery(m => movieIds.Contains(m.Id) && m.IsShow)
                .Include(m => m.MovieGenres)
                    .ThenInclude(mg => mg.Genre)
                .Select(m => new Movie
                {
                    Id = m.Id,
                    Title = m.Title,
                    IsPaid = m.IsPaid,
                    Price = m.Price,
                    Description = m.Description,
                    Director = m.Director,
                    Actors = m.Actors,
                    PosterUrl = m.PosterUrl,
                    BannerUrl = m.BannerUrl,
                    IsSeries = m.IsSeries,
                    IsSeriesCompleted = m.IsSeriesCompleted,
                    VideoUrl = m.VideoUrl,
                    View = m.View,
                    RateCount = m.RateCount,
                    RateTotal = m.RateTotal,
                    IsShow = m.IsShow,
                    ReleaseDate = m.ReleaseDate,
                    Episodes = m.Episodes,
                    MovieGenres = m.MovieGenres.Where(mg => mg.Genre.IsShow).ToList()
                })
                .ToListAsync();

            trendingMovies = trendingMovies.OrderBy(m => movieIds.IndexOf(m.Id)).ToList();
            List<MovieDTO> movieDTOs = _mapper.Map<List<MovieDTO>>(trendingMovies);

            foreach (var movie in movieDTOs)
            {
                int commentCount = await _commentRepo.CountAsync(c => c.MovieId == movie.Id);
                movie.CommentCount = commentCount;
            }

            return Result<List<MovieDTO>>.Success(movieDTOs, "Trending movies retrieved successfully");
        }

        public async Task<Result<List<MovieDTO>>> SearchMovieByName(string keyword)
        {
            IEnumerable<Movie> movies = await _movieRepo.FindAllAsync(m => m.IsShow && m.Title.ToLower().Trim().Contains(keyword.ToLower().Trim()));
            List<MovieDTO> movieDTOs = _mapper.Map<List<MovieDTO>>(movies);
            return Result<List<MovieDTO>>.Success(movieDTOs, "Movies retrieved successfully");
        }

        public async Task<Result<List<MovieDTO>>> GetNewComment(int topCount)
        {
            var latestCommentsQuery = _commentRepo
                .GetBaseQuery(c => true)
                .OrderByDescending(c => c.CreatedDate)
                .GroupBy(c => c.MovieId)
                .Select(group => new
                {
                    MovieId = group.Key,
                    LatestCommentDate = group.Max(c => c.CreatedDate)
                })
                .OrderByDescending(c => c.LatestCommentDate)
                .Take(topCount);

            var latestComments = await latestCommentsQuery.ToListAsync();

            var movieIds = latestComments.Select(c => c.MovieId).ToList();

            var moviesQuery = _movieRepo
                .GetBaseQuery(m => movieIds.Contains(m.Id) && m.IsShow);
            var movies = await moviesQuery.ToListAsync();

            var sortedMovies = movies
                .OrderBy(m => movieIds.IndexOf(m.Id))
                .ToList();

            var movieDTOs = _mapper.Map<List<MovieDTO>>(sortedMovies);
            return Result<List<MovieDTO>>.Success(movieDTOs, "Movies with latest comments retrieved successfully.");
        }

        public async Task<Result<List<MovieDTO>>> GetRecommendedMovies(int movieId, int topMovie)
        {
            try
            {

                var userLogDates = await _watchHistoryRepo
                    .GetBaseQuery(wh => wh.MovieId == movieId)
                    .Select(wh => new { wh.UserId, wh.LogDate })
                    .ToListAsync();

                if (!userLogDates.Any())
                {
                    return Result<List<MovieDTO>>.Success(new List<MovieDTO>(), "No movies were found.");
                }

                var watchHistories = await _watchHistoryRepo
                    .GetBaseQuery(wh => wh.MovieId != movieId)
                    .ToListAsync();

                var watchedAfterMovies = watchHistories
                    .Where(wh => userLogDates.Any(u => u.UserId == wh.UserId && wh.LogDate > u.LogDate))
                    .GroupBy(wh => wh.MovieId)
                    .Select(group => new
                    {
                        MovieId = group.Key,
                        WatchCount = group.Count(),
                        LastWatched = group.Max(wh => wh.LogDate)
                    })
                    .OrderByDescending(group => group.WatchCount)
                    .ThenByDescending(group => group.LastWatched)
                    .Take(topMovie)
                    .ToList();

                if (!watchedAfterMovies.Any())
                {
                    return Result<List<MovieDTO>>.Failure("No movies were watched after the current movie.");
                }

                var movieIds = watchedAfterMovies.Select(m => m.MovieId).ToList();
                var moviesQuery = _movieRepo
                    .GetBaseQuery(m => movieIds.Contains(m.Id));
                var watchedAfterMoviesDetails = await moviesQuery.ToListAsync();

                var sortedMovies = watchedAfterMoviesDetails
                    .OrderBy(m => movieIds.IndexOf(m.Id))
                    .ToList();

                var movieDTOs = _mapper.Map<List<MovieDTO>>(sortedMovies);
                return Result<List<MovieDTO>>.Success(movieDTOs, "Movies watched after the current movie retrieved successfully.");
            }
            catch (Exception ex)
            {
                return Result<List<MovieDTO>>.Failure($"An error occurred while retrieving movies: {ex.Message}");
            }
        }
    }
}
