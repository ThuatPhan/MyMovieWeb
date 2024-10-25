using MyMovieWeb.Application.DTOs.Requests;
using MyMovieWeb.Application.DTOs.Responses;

namespace MyMovieWeb.Application.Interfaces
{
    public interface IEpisodeServices
    {
        Task<Result<EpisodeDTO>> CreateEpisode(CreateEpisodeRequestDTO episodeRequestDTO);
        Task<Result<EpisodeDTO>> UpdateEpisode(int id, UpdateEpisodeRequestDTO episodeRequestDTO);
        Task<Result<bool>> DeleteEpisode(int id);
        Task<Result<EpisodeDTO>> GetById(int id);
    }
}
