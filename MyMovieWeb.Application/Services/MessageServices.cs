using Microsoft.AspNetCore.SignalR;
using MyMovieWeb.Application.Helper;
using MyMovieWeb.Application.Interfaces;

namespace MyMovieWeb.Application.Services
{
    public class MessageServices : IMessageServices
    {
        private readonly IHubContext<MessageHub> _hubContext;

        public MessageServices(IHubContext<MessageHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task<Result<bool>> SendMessage(string message)
        {
            await _hubContext.Clients.All.SendAsync("ServerMessage", message);
            return Result<bool>.Success(true, "Message sent successfully");
        }
    }
}
