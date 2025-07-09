using System.ComponentModel.DataAnnotations;
using InfertilityTreatment.Entity.Enums;

namespace InfertilityTreatment.Entity.DTOs.Notifications
{
    public class BroadcastNotificationDto
    {
        [Required]
        [StringLength(200, MinimumLength = 5)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(1000, MinimumLength = 10)]
        public string Message { get; set; } = string.Empty;

        [Required]
        public NotificationType Type { get; set; } = NotificationType.General;

        [Required]
        public NotificationPriority Priority { get; set; } = NotificationPriority.Medium;

        public DateTime? ScheduledAt { get; set; }

        public List<string>? TargetRoles { get; set; } // If null, send to all users

        public DateTime? ExpiresAt { get; set; }
    }
}
