using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MyMovieWeb.Application.DTOs.Responses;
using MyMovieWeb.Application.Interfaces;
using MyMovieWeb.Domain.Entities;
using MyMovieWeb.Domain.Interfaces;
using System.Linq.Expressions;

namespace MyMovieWeb.Application.Services
{
    public class NotificationServices : INotificationServices
    {
        private readonly IMapper _mapper;
        private readonly IRepository<Notification> _notificationRepo;

        public NotificationServices(IMapper mapper, IRepository<Notification> notificationRepository)
        {
            _mapper = mapper;
            _notificationRepo = notificationRepository;
        }

        public async Task<Result<NotificationDTO>> AddNotification(string userId, string message, string? url = null)
        {
            Notification newNotification = new Notification
            {
                UserId = userId,
                Message = message,
                Url = url
            };

            Notification createdNotification = await _notificationRepo.AddAsync(newNotification);
            NotificationDTO notificationDTO = _mapper.Map<NotificationDTO>(createdNotification);
            return Result<NotificationDTO>.Success(notificationDTO, "Notification created successfully");
        }

        public async Task AddNotifications(List<string> userIds, string message, string? url = null)
        {
            if (!userIds.Any()) return;

            foreach (var userId in userIds)
            {
                await _notificationRepo.AddAsync(new Notification
                {
                    UserId = userId,
                    Message = message,
                    Url = url
                });
            }
        }


        public async Task<Result<NotificationDTO>> MarkAsRead(int id)
        {
            Notification? notification = await _notificationRepo.GetByIdAsync(id);
            if (notification is null)
            {
                return Result<NotificationDTO>.Failure($"Notification id {id} not found");
            }

            notification.IsRead = true;
            Notification readedNotification = await _notificationRepo.UpdateAsync(notification);
            NotificationDTO notificationDTO = _mapper.Map<NotificationDTO>(readedNotification);

            return Result<NotificationDTO>.Success(notificationDTO, "Mark notification as read successfully");
        }

        public async Task<Result<bool>> DeleteNotification(int id)
        {
            Notification? notification = await _notificationRepo.GetByIdAsync(id);
            if (notification is null)
            {
                return Result<bool>.Failure($"Notification id {id} not found");
            }

            await _notificationRepo.RemoveAsync(notification);

            return Result<bool>.Success(true, "Notification deleted successfully");
        }

        public async Task<Result<List<NotificationDTO>>> GetNotifications(string userId)
        {
            IEnumerable<Notification> notification = await _notificationRepo
                .GetBaseQuery(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

            List<NotificationDTO> notificationDTOs = _mapper.Map<List<NotificationDTO>>(notification);

            return Result<List<NotificationDTO>>.Success(notificationDTOs, "Notifications retrieved successfully");
        }
    }
}
