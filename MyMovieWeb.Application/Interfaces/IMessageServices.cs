namespace MyMovieWeb.Application.Interfaces
{
    public interface IMessageServices
    {
        Task<Result<bool>> SendMessage(string message);
    }
}
