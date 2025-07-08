using InfertilityTreatment.Business.Interfaces;
using InfertilityTreatment.Data.Repositories.Interfaces;
using InfertilityTreatment.Entity.DTOs.Notifications;
using InfertilityTreatment.Entity.Entities;
using InfertilityTreatment.Entity.Enums;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfertilityTreatment.Business.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IEmailService _emailService;
        private readonly IBaseRepository<User> _userRepository;
        private readonly IRealTimeNotificationService _realTimeService;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(
            INotificationRepository notificationRepository,
            IEmailService emailService,
            IBaseRepository<User> userRepository,
            IRealTimeNotificationService realTimeService,
            ILogger<NotificationService> logger)
        {
            _notificationRepository = notificationRepository;
            _emailService = emailService;
            _userRepository = userRepository;
            _realTimeService = realTimeService;
            _logger = logger;
        }
        public async Task<bool> CreateNotificationAsync(CreateNotificationDto createDto)
        {
            var notification = new Notification
            {
                UserId = createDto.UserId,
                Title = createDto.Title,
                Message = createDto.Message,
                Type = createDto.Type,
                IsRead = false,
                CreatedAt = DateTime.UtcNow,
                RelatedEntityId = createDto.RelatedEntityId,
                RelatedEntityType = createDto.RelatedEntityType,
                ScheduledAt = createDto.ScheduledAt,
                SentAt = null
            };

            await _notificationRepository.AddAsync(notification);
            var saved = await _notificationRepository.SaveChangesAsync();

            if (saved > 0)
            {
                try
                {
                    // Create notification response DTO for real-time sending
                    var notificationDto = new NotificationResponseDto
                    {
                        Id = notification.Id,
                        UserId = notification.UserId,
                        Title = notification.Title,
                        Message = notification.Message,
                        Type = notification.Type,
                        IsRead = notification.IsRead,
                        CreatedAt = notification.CreatedAt,
                        RelatedEntityId = notification.RelatedEntityId,
                        RelatedEntityType = notification.RelatedEntityType,
                        ScheduledAt = notification.ScheduledAt,
                        SentAt = notification.SentAt
                    };

                    // Send real-time notification directly to user
                    var realTimeSent = await _realTimeService.SendNotificationToUserAsync(createDto.UserId, notificationDto);
                    
                    if (realTimeSent)
                    {
                        _logger.LogInformation("Sent real-time notification {NotificationId} to user {UserId}", 
                            notification.Id, createDto.UserId);
                    }
                    else
                    {
                        _logger.LogWarning("Failed to send real-time notification {NotificationId} to user {UserId}", 
                            notification.Id, createDto.UserId);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send real-time notification {NotificationId} to user {UserId}", 
                        notification.Id, createDto.UserId);
                    // Continue with email sending even if real-time fails
                }

                // Existing email logic...
                //var user = await _userRepository.GetByIdAsync(createDto.UserId);
                //if (user != null && !string.IsNullOrEmpty(user.Email))
                //{
                //    if (createDto.Type == NotificationType.Appointment.ToString() ||
                //        createDto.Type == NotificationType.General.ToString() ||
                //        createDto.Type == NotificationType.Result.ToString())
                //    {
                //        var emailSubject = $"[Thông báo] {createDto.Title}";
                //        var emailBody = $"Kính gửi {user.FullName},<br><br>{createDto.Message}<br><br>Trân trọng,<br>Trung tâm Hiếm muộn.";
                //        await _emailService.SendEmailAsync(user.Email, emailSubject, emailBody);
                //        notification.SentAt = DateTime.UtcNow;
                //        await _notificationRepository.UpdateAsync(notification);
                //        await _notificationRepository.SaveChangesAsync();
                //    }
                //    else if (createDto.Type == NotificationType.Reminder.ToString() && createDto.ScheduledAt.HasValue && createDto.ScheduledAt.Value <= DateTime.UtcNow.AddMinutes(5))
                //    {
                //        var emailSubject = $"[Nhắc nhở] {createDto.Title}";
                //        var emailBody = $"Kính gửi {user.FullName},<br><br>{createDto.Message}<br><br>Trân trọng,<br>Trung tâm Hiếm muộn.";
                //        await _emailService.SendEmailAsync(user.Email, emailSubject, emailBody);
                //        notification.SentAt = DateTime.UtcNow;
                //        await _notificationRepository.UpdateAsync(notification);
                //        await _notificationRepository.SaveChangesAsync();
                //    }
                //}
                return true;
            }
            return false;
        }
        public async Task<List<NotificationResponseDto>> GetUserNotificationsAsync(int userId)
        {
            var notifications = await _notificationRepository.GetNotificationsByUserIdAsync(userId);

            return notifications.Select(n => new NotificationResponseDto
            {
                Id = n.Id,
                UserId = n.UserId,
                Title = n.Title,
                Message = n.Message,
                Type = n.Type,
                IsRead = n.IsRead,
                CreatedAt = n.CreatedAt,
                RelatedEntityId = n.RelatedEntityId,
                RelatedEntityType = n.RelatedEntityType,
                ScheduledAt = n.ScheduledAt,
                SentAt = n.SentAt
            }).ToList();
        }
        public async Task<bool> MarkAsReadAsync(int notificationId)
        {
            var notification = await _notificationRepository.GetByIdAsync(notificationId);
            if (notification == null)
            {
                return false;
            }

            notification.IsRead = true;
            await _notificationRepository.UpdateAsync(notification);
            return await _notificationRepository.SaveChangesAsync() > 0;
        }
        public async Task<bool> DeleteNotificationAsync(int notificationId)
        {
            var exists = await _notificationRepository.ExistsAsync(notificationId);
            if (!exists)
            {
                return false;
            }

            await _notificationRepository.DeleteAsync(notificationId);
            return await _notificationRepository.SaveChangesAsync() > 0;
        }
        public async Task<Notification?> GetNotificationIfOwnedAsync(int notificationId, int userId)
        {
            return await _notificationRepository.FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId);
        }
    }
}
