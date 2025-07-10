namespace InfertilityTreatment.Entity.DTOs.Notifications
{
    public class RealTimeNotificationStatusDto
    {
        public int UserId { get; set; }
        public bool IsConnected { get; set; }
        public DateTime? LastConnectedAt { get; set; }
        public string ConnectionId { get; set; } = string.Empty;
        public List<string> ActiveGroups { get; set; } = new();
        public int UnreadCount { get; set; }
        public int TotalNotifications { get; set; }
        public List<NotificationResponseDto> RecentNotifications { get; set; } = new();
        public Dictionary<string, object> ConnectionInfo { get; set; } = new();
        public int ConnectionCount { get; set; }
        public DateTime? LastSeen { get; set; }
    }
}
