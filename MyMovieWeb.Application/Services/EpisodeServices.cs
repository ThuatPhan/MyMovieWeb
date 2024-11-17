using AutoMapper;
using Microsoft.EntityFrameworkCore;
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
        private readonly FileUploadHelper _uploadHelper;
        private readonly IRepository<Episode> _episodeRepo;
        private readonly IRepository<Movie> _movieRepository;
        private readonly IRepository<Comment> _commentRepo;
        private readonly IRepository<WatchHistory> _watchHistoryRepo;

        public EpisodeServices(
            IMapper mapper,
            FileUploadHelper uploadHelper,
            IRepository<Movie> movieRepository,
            IRepository<Episode> episodeRepo,
            IRepository<Comment> commentRepository,
            IRepository<WatchHistory> watchHistoryRepository
        )
        {
            _mapper = mapper;
            _uploadHelper = uploadHelper;
            _episodeRepo = episodeRepo;
            _movieRepository = movieRepository;
            _commentRepo = commentRepository;
            _watchHistoryRepo = watchHistoryRepository;
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

            int episodeNumber = await _episodeRepo.CountAsync(e => e.MovieId == movieOfEpisode.Id) + 1;

            Episode newEpisode = _mapper.Map<Episode>(episodeRequestDTO);
            newEpisode.EpisodeNumber = episodeNumber;

            string videoUrl = await _uploadHelper.UploadVideoAsync(episodeRequestDTO.VideoFile);
            newEpisode.VideoUrl = videoUrl;

            string thumbnailUrl = await _uploadHelper.UploadImageAsync(episodeRequestDTO.ThumbnailFile);
            newEpisode.ThumbnailUrl = thumbnailUrl;

            Episode createdEpisode = await _episodeRepo.AddAsync(newEpisode);

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
            if (episodeRequestDTO.VideoFile != null)
            {
                await _uploadHelper.DeleteVideoFileAsync(episodeToUpdate.VideoUrl);
                string videoUrl = await _uploadHelper.UploadVideoAsync(episodeRequestDTO.VideoFile);
                episodeToUpdate.VideoUrl = videoUrl;
            }
            if (episodeRequestDTO.ThumbnailFile != null)
            {
                await _uploadHelper.DeleteImageFileAsync(episodeToUpdate.ThumbnailUrl);
                string thumbnailUrl = await _uploadHelper.UploadImageAsync(episodeRequestDTO.ThumbnailFile);
                episodeToUpdate.ThumbnailUrl = thumbnailUrl;
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

            var deleteFileTasks = Task.WhenAll(
                _uploadHelper.DeleteVideoFileAsync(episodeToDelete.VideoUrl),
                _uploadHelper.DeleteImageFileAsync(episodeToDelete.ThumbnailUrl)
            );

            var deleteDataTasks = Task.WhenAll(
                _commentRepo.RemoveRangeAsync(c => c.EpisodeId == id),
                _watchHistoryRepo.RemoveRangeAsync(wh => wh.EpisodeId == id)
            );

            await Task.WhenAll(deleteFileTasks, deleteDataTasks);
            await _episodeRepo.RemoveAsync(episodeToDelete);

            return Result<bool>.Success(true, "Episode deleted successfully");
        }

        public async Task<Result<bool>> DeleteEpisodeOfMovie(int movieId)
        {
            Movie? movieOfEpisode = await _movieRepository.GetByIdAsync(movieId);

            if (movieOfEpisode is null)
            {
                return Result<bool>.Failure($"Movie id {movieId} not found");
            }

            IEnumerable<Episode> episodesOfMovie = await _episodeRepo.FindAllAsync(e => e.MovieId == movieId);

            var deleteTasks = new List<Task>();
            foreach (var episode in episodesOfMovie)
            {
                deleteTasks.Add(_uploadHelper.DeleteImageFileAsync(episode.ThumbnailUrl));
                deleteTasks.Add(_uploadHelper.DeleteVideoFileAsync(episode.VideoUrl));
            }
            await Task.WhenAll(deleteTasks);

            await _episodeRepo.RemoveRangeAsync(e => e.MovieId == movieId);

            return Result<bool>.Success(true, "Episode deleted successfully");
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

            IQueryable<Episode> query = _episodeRepo.GetBaseQuery(predicate: e => e.MovieId == movieId);
            IEnumerable<Episode> episodesOfMovie = await query.ToListAsync();

            List<EpisodeDTO> episodeDTOs = _mapper.Map<List<EpisodeDTO>>(episodesOfMovie);

            return Result<List<EpisodeDTO>>.Success(episodeDTOs, "Episodes retrieved successfully");
        }
    }
}
