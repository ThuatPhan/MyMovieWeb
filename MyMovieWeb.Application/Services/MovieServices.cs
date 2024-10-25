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
        private readonly IMovieRepository _movieRepo;
        private readonly FileUploadHelper _uploadHelper;
        private readonly IMapper _mapper;

        public MovieServices(IMovieRepository movieRepo, FileUploadHelper uploadHelper, IMapper mapper)
        {
            _movieRepo = movieRepo;
            _uploadHelper = uploadHelper;
            _mapper = mapper;
        }

        public async Task<Result<MovieDTO>> CreateMovie(CreateMovieRequestDTO movieRequestDTO)
        {
            Movie movieToCreate = _mapper.Map<Movie>(movieRequestDTO);

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
            Movie? movieToUpdate = await _movieRepo.GetByIdAsync(id);

            if (movieToUpdate is null)
            {
                return Result<MovieDTO>.Failure($"Movie id {id} not found");
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

            
            movieToUpdate.MovieGenres.Clear();

            _mapper.Map(movieRequestDTO, movieToUpdate);

            await _movieRepo.UpdateAsync(movieToUpdate);

            Movie? updatedMovie = await _movieRepo.GetByIdIncludeGenresAsync(id);
            MovieDTO movieDTO = _mapper.Map<MovieDTO>(updatedMovie);

            return Result<MovieDTO>.Success(movieDTO, "Movie updated successfully");
        }

        public Task<Result<bool>> DeleteMovie(int id)
        {
            throw new NotImplementedException();
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
    }
}
