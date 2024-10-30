using AutoMapper;
using MyMovieWeb.Application.DTOs.Requests;
using MyMovieWeb.Application.DTOs.Responses;
using MyMovieWeb.Application.Helper;
using MyMovieWeb.Application.Interfaces;
using MyMovieWeb.Domain.Entities;
using MyMovieWeb.Domain.Interfaces;

namespace MyMovieWeb.Application.Services
{
    public class EpisodeServices : IEpisodeServices
    {
        private readonly IMapper _mapper;
        private readonly IMovieRepository _movieRepository;
        private readonly IEpisodeRepository _episodeRepo;
        private readonly FileUploadHelper _uploadHelper;

        public EpisodeServices(IMapper mapper, IMovieRepository movieRepository, IEpisodeRepository episodeRepo, FileUploadHelper uploadHelper)
        {
            _mapper = mapper;
            _movieRepository = movieRepository;
            _episodeRepo = episodeRepo;
            _uploadHelper = uploadHelper;
        }

        public async Task<Result<EpisodeDTO>> CreateEpisode(CreateEpisodeRequestDTO episodeRequestDTO)
        {
            Movie? movieOfEpisode = await _movieRepository.GetByIdAsync(episodeRequestDTO.MovieId);

            if (movieOfEpisode is null)
            {
                return Result<EpisodeDTO>.Failure($"Movie id {episodeRequestDTO.MovieId} not found");
            }
            if (!movieOfEpisode.IsSeries)
            {
                return Result<EpisodeDTO>.Failure($"Movie id {episodeRequestDTO.MovieId} is not a series");
            }

            int episodeNumber = await _episodeRepo.GetTotalEpisodeCountAsync(movieOfEpisode.Id) + 1;

            Episode newMovie = _mapper.Map<Episode>(episodeRequestDTO);
            newMovie.EpisodeNumber = episodeNumber;

            string videoUrl = await _uploadHelper.UploadVideoAsync(episodeRequestDTO.videoFile);
            newMovie.VideoUrl = videoUrl;

            Episode createdEpisode = await _episodeRepo.AddAsync(newMovie);

            EpisodeDTO episodeDTO = _mapper.Map<EpisodeDTO>(createdEpisode);

            return Result<EpisodeDTO>.Success(episodeDTO, "Create episode successfully");
        }

        public async Task<Result<EpisodeDTO>> UpdateEpisode(int id, UpdateEpisodeRequestDTO episodeRequestDTO)
        {
            Episode? episodeToUpdate = await _episodeRepo.GetByIdAsync(id);

            if (episodeToUpdate is null)
            {
                return Result<EpisodeDTO>.Failure($"Episode id {id} not found");
            }
            if (episodeRequestDTO.videoFile != null)
            {
                string videoUrl = await _uploadHelper.UploadVideoAsync(episodeRequestDTO.videoFile);
                episodeToUpdate.VideoUrl = videoUrl;
            }

            _mapper.Map(episodeRequestDTO, episodeToUpdate);
            Episode updatedEpisode = await _episodeRepo.UpdateAsync(episodeToUpdate);
            EpisodeDTO episodeDTO = _mapper.Map<EpisodeDTO>(updatedEpisode);

            return Result<EpisodeDTO>.Success(episodeDTO, "Episode updated successfully");
        }

        public async Task<Result<bool>> DeleteEpisode(int id)
        {
            Episode? episodeToDelete = await _episodeRepo.GetByIdAsync(id);
            if (episodeToDelete is null)
            {
                return Result<bool>.Failure($"Episode id {id} not found");
            }

            await _episodeRepo.RemoveAsync(episodeToDelete);

            return Result<bool>.Success(true, "Episode deleted successfully");
        }

        public async Task<Result<int>> CountEpisode(int movieId)
        {
            Movie? movieOfEpisodes = await _movieRepository.GetByIdAsync(movieId);

            if (movieOfEpisodes is null)
            {
                return Result<int>.Failure($"Movie id {movieId} not found");
            }

            if (!movieOfEpisodes.IsSeries)
            {
                return Result<int>.Failure($"Movie id {movieId} is not a series");
            }

            int totalEpisodeCount = await _episodeRepo.GetTotalEpisodeCountAsync(movieId);

            return Result<int>.Success(totalEpisodeCount, "Total episode count retrieved successfully");
        }

        public async Task<Result<EpisodeDTO>> GetEpisodeById(int id)
        {
            Episode? episode = await _episodeRepo.GetByIdAsync(id);

            if (episode is null)
            {
                return Result<EpisodeDTO>.Failure($"Episode id {id} not found");
            }

            EpisodeDTO episodeDTO = _mapper.Map<EpisodeDTO>(episode);

            return Result<EpisodeDTO>.Success(episodeDTO, "Episode retrived successfully");
        }

        public async Task<Result<List<EpisodeDTO>>> GetEpisodesOfMovie(int movieId)
        {
            Movie? movieOfEpisodes = await _movieRepository.GetByIdAsync(movieId);

            if (movieOfEpisodes is null)
            {
                return Result<List<EpisodeDTO>>.Failure($"Movie id {movieId} not found");
            }
            if (!movieOfEpisodes.IsSeries)
            {
                return Result<List<EpisodeDTO>>.Failure($"Movie id {movieId} is not a series");
            }

            IEnumerable<Episode> episodesOfMovie = await _episodeRepo.GetEpisodesAsync(movieId);
            List<EpisodeDTO> episodeDTOs = _mapper.Map<List<EpisodeDTO>>(episodesOfMovie);

            return Result<List<EpisodeDTO>>.Success(episodeDTOs, "Episodes retrieved successfully");
        }

        public async Task<Result<List<EpisodeDTO>>> GetPagedEpisodesOfMovie(int movieId, int pageNumber, int pageSize)
        {
            Movie? movieOfEpisodes = await _movieRepository.GetByIdAsync(movieId);

            if (movieOfEpisodes is null)
            {
                return Result<List<EpisodeDTO>>.Failure($"Movie id {movieId} not found");
            }
            if (!movieOfEpisodes.IsSeries)
            {
                return Result<List<EpisodeDTO>>.Failure($"Movie id {movieId} is not a series");
            }

            IEnumerable<Episode> episodesOfMovie = await _episodeRepo.GetPagedEpisodesAsync(movieId, pageNumber, pageSize);
            List<EpisodeDTO> episodeDTOs = _mapper.Map<List<EpisodeDTO>>(episodesOfMovie);

            return Result<List<EpisodeDTO>>.Success(episodeDTOs, "Episodes retrieved successfully");
        }
    }
}
