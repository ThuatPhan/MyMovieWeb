using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using MyMovieWeb.Application.DTOs.Requests;
using MyMovieWeb.Application.DTOs.Responses;
using MyMovieWeb.Application.Helper;
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
        private readonly IRepository<FollowedMovie> _followedMovieRepo;
        private readonly IRepository<WatchHistory> _watchHistoryRepo;
        private readonly IEpisodeServices _episodeServices;
        private readonly FileUploadHelper _uploadHelper;
        private readonly IMessageServices _messageServices;

        public MovieServices(
            IMapper mapper,
            IRepository<Genre> genreRepository,
            IRepository<Movie> movieRepo,
            IRepository<Comment> commentRepository,
            IRepository<FollowedMovie> followedMovieRepository,
            IRepository<WatchHistory> watchHistoryRepository,
            IEpisodeServices episodeServices,
            FileUploadHelper uploadHelper,
            IMessageServices messageServices
        )
        {
            _mapper = mapper;
            _uploadHelper = uploadHelper;
            _genreRepo = genreRepository;
            _movieRepo = movieRepo;
            _commentRepo = commentRepository;
            _followedMovieRepo = followedMovieRepository;
            _watchHistoryRepo = watchHistoryRepository;
            _episodeServices = episodeServices;
            _messageServices = messageServices;
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

            string posterUrl = await _uploadHelper.UploadImageAsync(movieRequestDTO.PosterFile);
            movieToCreate.PosterUrl = posterUrl;

            string bannerUrl = await _uploadHelper.UploadImageAsync(movieRequestDTO.BannerFile);
            movieToCreate.BannerUrl = bannerUrl;

            if (!movieToCreate.IsSeries && movieRequestDTO.VideoFile != null)
            {
                string videoUrl = await _uploadHelper.UploadVideoAsync(movieRequestDTO.VideoFile);
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
                await _uploadHelper.DeleteImageFileAsync(movieToUpdate.PosterUrl);
                string posterUrl = await _uploadHelper.UploadImageAsync(movieRequestDTO.PosterFile);
                movieToUpdate.PosterUrl = posterUrl;
            }

            if (movieRequestDTO.BannerFile != null)
            {
                await _uploadHelper.DeleteImageFileAsync(movieToUpdate.BannerUrl);
                string bannerUrl = await _uploadHelper.UploadImageAsync(movieRequestDTO.BannerFile);
                movieToUpdate.BannerUrl = bannerUrl;
            }

            if (!movieToUpdate.IsSeries && movieRequestDTO.VideoFile != null)
            {
                await _uploadHelper.DeleteVideoFileAsync(movieToUpdate.VideoUrl!);
                string videoUrl = await _uploadHelper.UploadVideoAsync(movieRequestDTO.VideoFile);
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
                _followedMovieRepo.RemoveRangeAsync(wh => wh.MovieId == id)
            );

            await _episodeServices.DeleteEpisodeOfMovie(id);

            var deleteFileTasks = Task.WhenAll(
                _uploadHelper.DeleteImageFileAsync(movieToDelete.PosterUrl),
                _uploadHelper.DeleteImageFileAsync(movieToDelete.BannerUrl)
            );

            await deleteDataTasks;
            await deleteFileTasks;

            if (movieToDelete.VideoUrl != null)
            {
                await _uploadHelper.DeleteVideoFileAsync(movieToDelete.VideoUrl);
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
            IQueryable<Movie> query = _movieRepo.GetBaseQuery(predicate: m => m.Id == id);

            Movie? movie = await query
                .Include(m => m.MovieGenres)
                    .ThenInclude(mg => mg.Genre)
                .FirstOrDefaultAsync();

            if (movie is null)
            {
                return Result<MovieDTO>.Failure($"Movie id {id} not found");
            }

            MovieDTO movieDTO = _mapper.Map<MovieDTO>(movie);
            int commentCount = await _commentRepo.CountAsync(c => c.MovieId == id);
            movieDTO.CommentCount = commentCount;

            return Result<MovieDTO>.Success(movieDTO, "Movie retrieved successfully");
        }

        public async Task<Result<MovieDTO>> GetMovieToWatch(int id, bool isAuthenticated)
        {
            IQueryable<Movie> query = _movieRepo.GetBaseQuery(predicate: m => m.Id == id);

            Movie? movie = await query.FirstOrDefaultAsync();

            if (movie is null)
            {
                return Result<MovieDTO>.Failure($"Movie id {id} not found");
            }

            MovieDTO movieDTO = _mapper.Map<MovieDTO>(movie);

            if (isAuthenticated)
            {
                await _messageServices.SendMessage($"Has a user watching movie {movieDTO.Title}");
            }

            return Result<MovieDTO>.Success(movieDTO, "Movie retrieved successfully");
        }

        public async Task<Result<List<MovieDTO>>> FindAllMovies(
            int pageNumber,
            int pageSize,
            Expression<Func<Movie, bool>> predicate,
            Func<IQueryable<Movie>, IOrderedQueryable<Movie>>? orderBy = null
        )
        {
            IQueryable<Movie> query = _movieRepo.GetBaseQuery(predicate);
            if (orderBy != null)
            {
                query = orderBy(query);
            }

            IEnumerable<Movie> movies = await query
                .Include(m => m.MovieGenres)
                    .ThenInclude(mg => mg.Genre)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            List<MovieDTO> movieDTOs = _mapper.Map<List<MovieDTO>>(movies);

            foreach (var movieDTO in movieDTOs)
            {
                int commentCount = await _commentRepo.CountAsync(c => c.MovieId == movieDTO.Id);
                movieDTO.CommentCount = commentCount;
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
            var topMoviesData = await query.ToListAsync();

            var movieIds = topMoviesData
                .Select(m => m.MovieId)
                .ToList();

            var topViewQuery = _movieRepo
                .GetBaseQuery(predicate: m => movieIds.Contains(m.Id));
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
                .FindAllAsync(m => movieIds.Contains(m.Id));

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
            IEnumerable<Movie> movies = await _movieRepo.FindAllAsync(m => m.Title.ToLower().Trim().Contains(keyword.ToLower().Trim()));
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
                .GetBaseQuery(m => movieIds.Contains(m.Id));
            var movies = await moviesQuery.ToListAsync();

            var sortedMovies = movies
                .OrderBy(m => movieIds.IndexOf(m.Id))
                .ToList();

            var movieDTOs = _mapper.Map<List<MovieDTO>>(sortedMovies);
            return Result<List<MovieDTO>>.Success(movieDTOs, "Movies with latest comments retrieved successfully.");
        }
    }
}
