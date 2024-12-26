using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MyMovieWeb.Application.DTOs.Requests;
using MyMovieWeb.Application.DTOs.Responses;
using MyMovieWeb.Application.Interfaces;
using MyMovieWeb.Domain.Entities;
using MyMovieWeb.Domain.Interfaces;

namespace MyMovieWeb.Application.Services
{
    public class EpisodeServices : IEpisodeServices
    {
        private readonly IMapper _mapper;
        private readonly IRepository<Episode> _episodeRepo;
        private readonly IRepository<Movie> _movieRepository;
        private readonly IRepository<Comment> _commentRepo;
        private readonly IRepository<WatchHistory> _watchHistoryRepo;
        private readonly IRepository<FollowedMovie> _followedMovieRepo;
        private readonly INotificationServices _notificationServices;
        private readonly IS3Services _s3Service;

        public EpisodeServices(
            IMapper mapper,
            IRepository<Movie> movieRepository,
            IRepository<Episode> episodeRepo,
            IRepository<Comment> commentRepository,
            IRepository<WatchHistory> watchHistoryRepository,
            IRepository<FollowedMovie> followedMovieRepository,
            INotificationServices notificationServices,
            IS3Services s3Services

        )
        {
            _mapper = mapper;
            _episodeRepo = episodeRepo;
            _movieRepository = movieRepository;
            _commentRepo = commentRepository;
            _watchHistoryRepo = watchHistoryRepository;
            _followedMovieRepo = followedMovieRepository;
            _notificationServices = notificationServices;
            _s3Service = s3Services;
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

            string videoUrl = await _s3Service.UploadFileAsync(episodeRequestDTO.VideoFile);
            newEpisode.VideoUrl = videoUrl;

            string thumbnailUrl = await _s3Service.UploadFileAsync(episodeRequestDTO.ThumbnailFile);
            newEpisode.ThumbnailUrl = thumbnailUrl;

            Episode createdEpisode = await _episodeRepo.AddAsync(newEpisode);

            EpisodeDTO episodeDTO = _mapper.Map<EpisodeDTO>(createdEpisode);

            List<string> userIds = (await _followedMovieRepo.FindAllAsync(fm => fm.MovieId == movieOfEpisode.Id))
                .Select(fm => fm.UserId)
                .ToList();

            await _notificationServices.AddNotifications(userIds, $"Phim {movieOfEpisode.Title} đã có tập mới: {episodeDTO.Title}", $"/details/{movieOfEpisode.Id}");

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
                string videoUrl = await _s3Service.UploadFileAsync(episodeRequestDTO.VideoFile);
                episodeToUpdate.VideoUrl = videoUrl;
            }
            if (episodeRequestDTO.ThumbnailFile != null)
            {
                string thumbnailUrl = await _s3Service.UploadFileAsync(episodeRequestDTO.ThumbnailFile);
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
                _s3Service.DeleteFileAsync(episodeToDelete.VideoUrl),
                _s3Service.DeleteFileAsync(episodeToDelete.ThumbnailUrl)
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
                deleteTasks.Add(_s3Service.DeleteFileAsync(episode.ThumbnailUrl));
                deleteTasks.Add(_s3Service.DeleteFileAsync(episode.VideoUrl));
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
