using MyMovieWeb.Application.DTOs.Responses;

namespace MyMovieWeb.Application.Interfaces
{
    public interface IAuth0Services
    {
        Task<Result<string>> GetAccessToken();
        Task<Result<Auth0UserDTO>> GetUser(string userId);
        Task<Result<List<Auth0UserDTO>>> GetAllUsers();
    }
}
