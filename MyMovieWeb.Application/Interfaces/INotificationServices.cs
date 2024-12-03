using MyMovieWeb.Application.DTOs.Responses;
using MyMovieWeb.Domain.Entities;
using System.Linq.Expressions;

namespace MyMovieWeb.Application.Interfaces
{
    public interface INotificationServices
    {
        Task<Result<NotificationDTO>> AddNotification(string userId, string message, string? url = null);
        Task AddNotifications(List<string> userIds, string message, string? url = null);
        Task<Result<NotificationDTO>> MarkAsRead(int id);
        Task<Result<bool>> DeleteNotification(int id);
        Task<Result<List<NotificationDTO>>> GetNotifications(string userId);
    }
}
