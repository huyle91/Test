using System.ComponentModel.DataAnnotations;
using InfertilityTreatment.Entity.Enums;

namespace InfertilityTreatment.Entity.DTOs.Notifications
{
    public class ScheduleNotificationDto
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        [StringLength(200, MinimumLength = 5)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(1000, MinimumLength = 10)]
        public string Message { get; set; } = string.Empty;

        [Required]
        public NotificationType Type { get; set; } = NotificationType.Reminder;

        [Required]
        public NotificationPriority Priority { get; set; } = NotificationPriority.Medium;

        [Required]
        public DateTime ScheduledAt { get; set; }

        public int? RelatedEntityId { get; set; } // Appointment ID, Cycle ID, etc.

        public string? RelatedEntityType { get; set; } // Appointment, TreatmentCycle, Payment, etc.


        public bool SendEmail { get; set; } = false;

    }
}
