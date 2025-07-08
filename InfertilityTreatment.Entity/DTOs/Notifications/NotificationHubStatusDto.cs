namespace InfertilityTreatment.Entity.DTOs.Notifications
{
    public class NotificationHubStatusDto
    {
        public int TotalConnections { get; set; }
        public int AuthenticatedUsers { get; set; }
        public int AnonymousConnections { get; set; }
        public Dictionary<string, int> RoleDistribution { get; set; } = new();
        public List<object> UserConnections { get; set; } = new();
        public List<string> ActiveGroups { get; set; } = new();
        public DateTime ServerTime { get; set; }
        public Dictionary<string, object> Details { get; set; } = new();
        
        // Legacy properties for backward compatibility
        public int TotalConnectedUsers => AuthenticatedUsers;
        public int ConnectedDoctors => RoleDistribution.GetValueOrDefault("Doctor", 0);
        public int ConnectedCustomers => RoleDistribution.GetValueOrDefault("Customer", 0);
        public int ConnectedAdmins => RoleDistribution.GetValueOrDefault("Admin", 0) + RoleDistribution.GetValueOrDefault("Manager", 0);
        public DateTime LastUpdated => ServerTime;
        public List<UserConnectionInfo> ActiveConnections { get; set; } = new();
        public Dictionary<string, int> GroupMemberships { get; set; } = new();
        public int ConnectedUsers => AuthenticatedUsers;
    }

    public class UserConnectionInfo
    {
        public int UserId { get; set; }
        public string UserRole { get; set; } = string.Empty;
        public string ConnectionId { get; set; } = string.Empty;
        public DateTime ConnectedAt { get; set; }
        public List<string> Groups { get; set; } = new();
        public int ConnectionCount { get; set; } = 1;
    }
}
