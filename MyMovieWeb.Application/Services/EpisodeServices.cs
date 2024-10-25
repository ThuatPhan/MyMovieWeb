using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using MyMovieWeb.Application.Interfaces;
using MyMovieWeb.Application.DTOs.Responses;
using MyMovieWeb.Application.DTOs.Requests;
using MyMovieWeb.Application.Helper;
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

            IEnumerable<Episode> episodes = await _episodeRepo.GetEpisodeByMovieIdAsync(movieOfEpisode.Id);
            int episodeNumber = episodes.Any() ? episodes.Max(e => e.EpisodeNumber) + 1 : 1;

            Episode newMovie = _mapper.Map<Episode>(episodeRequestDTO);
            newMovie.EpisodeNumber = episodeNumber;

            string videoUrl = await _uploadHelper.UploadVideoAsync(episodeRequestDTO.videoFile);
            newMovie.VideoUrl = videoUrl;

            Episode createdEpisode = await _episodeRepo.AddAsync(newMovie);

            EpisodeDTO episodeDTO = _mapper.Map<EpisodeDTO>(createdEpisode);

            return Result<EpisodeDTO>.Success(episodeDTO, "Create episode successfully");
        }

        public Task<Result<EpisodeDTO>> UpdateEpisode(int id, UpdateEpisodeRequestDTO episodeRequestDTO)
        {
            throw new NotImplementedException();
        }

        public Task<Result<bool>> DeleteEpisode(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<Result<EpisodeDTO>> GetById(int id)
        {
            Episode? episode = await _episodeRepo.GetByIdAsync(id);

            if (episode is null)
            {
                return Result<EpisodeDTO>.Failure($"Episode id {id} not found");
            }

            EpisodeDTO episodeDTO = _mapper.Map<EpisodeDTO>(episode);

            return Result<EpisodeDTO>.Success(episodeDTO, "Episode retrived successfully");
        }
    }
}
