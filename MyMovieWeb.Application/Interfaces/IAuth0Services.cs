using MyMovieWeb.Application.DTOs.Responses;

namespace MyMovieWeb.Application.Interfaces
{
    public interface IAuth0Services
    {
        Task<string?> GetAccessToken();
        Task<Auth0UserDTO?> GetUser(string userId);
        Task<List<Auth0UserDTO>?> GetAllUsers();
    }
}
