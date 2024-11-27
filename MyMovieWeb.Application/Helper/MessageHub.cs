using Microsoft.AspNetCore.SignalR;

namespace MyMovieWeb.Application.Helper
{
    public class MessageHub : Hub
    {
        public async Task SendMessage(string username, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", username, message);
        }
    }
}
