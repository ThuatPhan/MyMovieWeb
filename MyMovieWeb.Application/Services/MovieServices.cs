using AutoMapper;
using MyMovieWeb.Application.DTOs.Requests;
using MyMovieWeb.Application.DTOs.Responses;
using MyMovieWeb.Application.Helper;
using MyMovieWeb.Application.Interfaces;
using MyMovieWeb.Domain.Entities;
using MyMovieWeb.Domain.Interfaces;

namespace MyMovieWeb.Application.Services
{
    public class MovieServices : IMovieService
    {
        private readonly IMapper _mapper;
        private readonly IGenreRepository _genreRepo;
        private readonly IMovieRepository _movieRepo;
        private readonly IEpisodeRepository _episodeRepo;
        private readonly FileUploadHelper _uploadHelper;

        public MovieServices(IMapper mapper, IGenreRepository genreRepository, IMovieRepository movieRepo, IEpisodeRepository episodeRepo, FileUploadHelper uploadHelper)
        {
            _genreRepo = genreRepository;
            _movieRepo = movieRepo;
            _episodeRepo = episodeRepo;
            _uploadHelper = uploadHelper;
            _mapper = mapper;
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

            List<MovieGenre> invalidGenres = movieToUpdate.MovieGenres
                .Where(genre => !existingGenreIds.Contains(genre.GenreId))
                .ToList();

            if (invalidGenres.Any())
            {
                String invalidGenreIds = string.Join(", ", invalidGenres.Select(g => g.GenreId));
                return Result<MovieDTO>.Failure($"Genre id(s) {invalidGenreIds} are not defined");
            }

            if (movieRequestDTO.PosterFile != null)
            {
                string posterUrl = await _uploadHelper.UploadImageAsync(movieRequestDTO.PosterFile);
                movieToUpdate.PosterUrl = posterUrl;
            }

            if (movieRequestDTO.BannerFile != null)
            {
                string bannerUrl = await _uploadHelper.UploadVideoAsync(movieRequestDTO.BannerFile);
                movieToUpdate.BannerUrl = bannerUrl;
            }

            if (!movieToUpdate.IsSeries && movieRequestDTO.VideoFile != null)
            {
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
            await _episodeRepo.RemoveRangeAsync(e => e.MovieId == id);
            await _movieRepo.RemoveAsync(movieToDelete);

            return Result<bool>.Success(true, "Movie deleted successfully");
        }

        public async Task<Result<int>> CountMovie()
        {
            int totalCount = await _movieRepo.CountAsync();
            return Result<int>.Success(totalCount, "Total count retrieved successfully");
        }

        public async Task<Result<int>> CountMovieByGenre(int genreId)
        {
            Genre? genreOfMovie = await _genreRepo.GetByIdAsync(genreId);

            if (genreOfMovie is null)
            {
                return Result<int>.Failure($"Genre id {genreId} not found");
            }

            int totalCount = await _movieRepo.CountByGenreAsync(genreId);
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

        public async Task<Result<List<MovieDTO>>> GetAllMovies()
        {
            IEnumerable<Movie> movies = await _movieRepo.GetAllIncludeGenresAsync();
            List<MovieDTO> movieDTOs = _mapper.Map<List<MovieDTO>>(movies);

            return Result<List<MovieDTO>>.Success(movieDTOs, "Movies retrieved successfully");
        }

        public async Task<Result<List<MovieDTO>>> GetPagedMovies(int pageNumber, int pageSize)
        {
            IEnumerable<Movie> movies = await _movieRepo.GetPagedMoviesAsync(pageNumber, pageSize);
            List<MovieDTO> movieDTOs = _mapper.Map<List<MovieDTO>>(movies);
            return Result<List<MovieDTO>>.Success(movieDTOs, "Movies retrieved successfully");
        }

        public async Task<Result<List<MovieDTO>>> GetPagedMoviesByGenre(int genreId, int pageNumber, int pageSize)
        {
            Genre? genreOfMovie = await _genreRepo.GetByIdAsync(genreId);

            if (genreOfMovie is null)
            {
                return Result<List<MovieDTO>>.Failure($"Genre id {genreId} not found");
            }

            IEnumerable<Movie> movies = await _movieRepo.GetPagedMoviesByGenreAsync(genreId, pageNumber, pageSize);
            List<MovieDTO> movieDTOs = _mapper.Map<List<MovieDTO>>(movies);

            return Result<List<MovieDTO>>.Success(movieDTOs, "Movies retrieved successfully");
        }
    }
}
