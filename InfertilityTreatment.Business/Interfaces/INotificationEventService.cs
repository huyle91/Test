using System;
using System.Threading.Tasks;

namespace InfertilityTreatment.Business.Interfaces
{
    public interface INotificationEvent
    {
        string Type { get; }
        object Data { get; }
        int? UserId { get; }
        string? Role { get; }
        string? GroupName { get; }
        DateTime Timestamp { get; }
    }

    public interface INotificationEventService
    {
        Task PublishNotificationEventAsync(INotificationEvent notificationEvent);
    }

    public class NotificationEvent : INotificationEvent
    {
        public string Type { get; set; } = string.Empty;
        public object Data { get; set; } = new();
        public int? UserId { get; set; }
        public string? Role { get; set; }
        public string? GroupName { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public static NotificationEvent CreateUserNotification(int userId, string type, object data)
        {
            return new NotificationEvent
            {
                Type = type,
                Data = data,
                UserId = userId,
                Timestamp = DateTime.UtcNow
            };
        }

        public static NotificationEvent CreateRoleNotification(string role, string type, object data)
        {
            return new NotificationEvent
            {
                Type = type,
                Data = data,
                Role = role,
                Timestamp = DateTime.UtcNow
            };
        }

        public static NotificationEvent CreateBroadcastNotification(string type, object data)
        {
            return new NotificationEvent
            {
                Type = type,
                Data = data,
                Timestamp = DateTime.UtcNow
            };
        }

        public static NotificationEvent CreateGroupNotification(string groupName, string type, object data)
        {
            return new NotificationEvent
            {
                Type = type,
                Data = data,
                GroupName = groupName,
                Timestamp = DateTime.UtcNow
            };
        }
    }
}
