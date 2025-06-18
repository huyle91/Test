using InfertilityTreatment.Entity.DTOs.Notifications;
using InfertilityTreatment.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfertilityTreatment.Business.Interfaces
{
    public interface INotificationService
    {
        Task<bool> CreateNotificationAsync(CreateNotificationDto createDto);
        Task<List<NotificationResponseDto>> GetUserNotificationsAsync(int userId);
        Task<bool> MarkAsReadAsync(int notificationId);
        Task<bool> DeleteNotificationAsync(int notificationId);
        Task<Notification?> GetNotificationIfOwnedAsync(int notificationId, int userId);
    }
}
