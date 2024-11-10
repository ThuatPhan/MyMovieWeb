using AutoMapper;
using MyMovieWeb.Application.DTOs.Responses;
using MyMovieWeb.Application.Interfaces;
using MyMovieWeb.Domain.Entities;
using MyMovieWeb.Domain.Interfaces;

namespace MyMovieWeb.Application.Services
{
    public class UserServices : IUserServices
    {
        private readonly IMapper _mapper;
        private readonly IMovieService _movieService;
        private readonly IFollowedMovieRepository _followedMovieRepo;

        public UserServices(IMapper mapper, IMovieService movieService, IFollowedMovieRepository followedMovieRepository)
        {
            _mapper = mapper;
            _movieService = movieService;
            _followedMovieRepo = followedMovieRepository;
        }

        public async Task<Result<bool>> FollowMovie(int movieId, string userId)
        {

            Result<MovieDTO> result = await _movieService.GetMovieById(movieId);

            if (!result.IsSuccess)
            {
                return Result<bool>.Failure(result.Message);
            }

            await _followedMovieRepo.AddAsync(new FollowedMovie { MovieId = result.Data.Id, UserId = userId });

            return Result<bool>.Success(true, "Movie followed successfully");
        }

        public async Task<Result<List<FollowedMovieDTO>>> GetFollowedMovies(string userId)
        {
            IEnumerable<FollowedMovie> followedMovies = await _followedMovieRepo.GetByUserIdIncludeMovie(userId);
            List<FollowedMovieDTO> followedMovieDTOs = _mapper.Map<List<FollowedMovieDTO>>(followedMovies);

            return Result<List<FollowedMovieDTO>>.Success(followedMovieDTOs, "Followed movies retrieved successfully");
        }
    }
}
