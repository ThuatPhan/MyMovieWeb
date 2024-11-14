using AutoMapper;
using MyMovieWeb.Application.DTOs.Requests;
using MyMovieWeb.Application.DTOs.Responses;
using MyMovieWeb.Application.Helper;
using MyMovieWeb.Application.Interfaces;
using MyMovieWeb.Domain.Entities;
using MyMovieWeb.Domain.Interfaces;
using System.Linq.Expressions;

namespace MyMovieWeb.Application.Services
{
    public class MovieServices : IMovieService
    {
        private readonly IMapper _mapper;
        private readonly IGenreRepository _genreRepo;
        private readonly IMovieRepository _movieRepo;
        private readonly IWatchHistoryRepository _watchHistoryRepo;
        private readonly IEpisodeServices _episodeServices;
        private readonly FileUploadHelper _uploadHelper;

        public MovieServices(IMapper mapper, IGenreRepository genreRepository, IMovieRepository movieRepo, IWatchHistoryRepository watchHistoryRepo, IEpisodeServices episodeServices, FileUploadHelper uploadHelper)
        {
            _mapper = mapper;
            _uploadHelper = uploadHelper;
            _movieRepo = movieRepo;
            _genreRepo = genreRepository;
            _watchHistoryRepo = watchHistoryRepo;
            _episodeServices = episodeServices;
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

            await _movieRepo.AddAsync(movieToCreate);
            Movie? createdMovie = await _movieRepo.GetByIdIncludeGenresAsync(movieToCreate.Id);
            MovieDTO movieDTO = _mapper.Map<MovieDTO>(createdMovie);

            return Result<MovieDTO>.Success(movieDTO, "Movie created successfully");
        }

        public async Task<Result<MovieDTO>> UpdateMovie(int id, UpdateMovieRequestDTO movieRequestDTO)
        {
            Movie? movieToUpdate = await _movieRepo.GetByIdIncludeGenresAsync(id);

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
                string bannerUrl = await _uploadHelper.UploadVideoAsync(movieRequestDTO.BannerFile);
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
            Movie? updatedMovie = await _movieRepo.GetByIdIncludeGenresAsync(id);
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

            await _watchHistoryRepo.RemoveRangeAsync(wh => wh.MovieId == id);
            await _episodeServices.DeleteEpisodeOfMovie(id);

            var deleteTasks = new List<Task>
            {
                _uploadHelper.DeleteImageFileAsync(movieToDelete.PosterUrl),
                _uploadHelper.DeleteImageFileAsync(movieToDelete.BannerUrl)
            };

            await Task.WhenAll(deleteTasks);

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
            return Result<int>.Success(totalCount, "Total count retrieved successfully");
        }

        public async Task<Result<MovieDTO>> GetMovieById(int id)
        {
            Movie? movie = await _movieRepo.GetByIdIncludeGenresAsync(id);

            if (movie is null)
            {
                return Result<MovieDTO>.Failure($"Movie id {id} not found");
            }

            MovieDTO movieDTO = _mapper.Map<MovieDTO>(movie);

            return Result<MovieDTO>.Success(movieDTO, "Movie retrieved successfully");
        }

        public async Task<Result<List<MovieDTO>>> FindAllMovies(
            int pageNumber,
            int pageSize,
            Expression<Func<Movie, bool>> predicate,
            Func<IQueryable<Movie>, IOrderedQueryable<Movie>>? orderBy = null)
        {
            IEnumerable<Movie> movies = await _movieRepo.FindAllIncludeGenresAsync(pageNumber, pageSize, predicate, orderBy);

            List<MovieDTO> movieDTOs = _mapper.Map<List<MovieDTO>>(movies);

            return Result<List<MovieDTO>>.Success(movieDTOs, "Movies retrieved successfully");
        }

        public async Task<Result<List<MovieDTO>>> GetMoviesSameGenreOfMovie(int movieId, int pageNumber, int pageSize)
        {
            Movie? movie = await _movieRepo.GetByIdIncludeGenresAsync(movieId);
            if (movie is null)
            {
                return Result<List<MovieDTO>>.Failure($"Movie id {movieId} not found");
            }

            List<int> genreIds = movie.MovieGenres.Select(mg => mg.GenreId).ToList();

            IEnumerable<Movie> movies = await _movieRepo.FindAllIncludeGenresAsync(
                pageNumber,
                pageSize,
                m => m.Id != movieId && m.MovieGenres.Any(mg => genreIds.Contains(mg.GenreId))
            );

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
            movieToRate.RateTotal = rateMovieRequestDTO.RateTotal;

            await _movieRepo.UpdateAsync(movieToRate);

            return Result<bool>.Success(true, "Rate created successfully");
        }
    }
}
