using InfertilityTreatment.API.Hubs;
using InfertilityTreatment.Business.Interfaces;
using InfertilityTreatment.Entity.DTOs.Notifications;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace InfertilityTreatment.API.Services
{
    public class SignalRNotificationService : IRealTimeNotificationService
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly ILogger<SignalRNotificationService> _logger;
        private readonly ConcurrentDictionary<int, List<string>> _userConnections = new();

        public SignalRNotificationService(
            IHubContext<NotificationHub> hubContext,
            ILogger<SignalRNotificationService> logger)
        {
            _hubContext = hubContext;
            _logger = logger;
        }

        public async Task HandleNotificationEventAsync(INotificationEvent notificationEvent)
        {
            try
            {
                switch (notificationEvent.Type)
                {
                    case "notification_created":
                    case "notification_updated":
                        await HandleUserNotification(notificationEvent);
                        break;
                    case "broadcast":
                        await HandleBroadcastNotification(notificationEvent);
                        break;
                    case "role_notification":
                        await HandleRoleNotification(notificationEvent);
                        break;
                    case "group_notification":
                        await HandleGroupNotification(notificationEvent);
                        break;
                    default:
                        _logger.LogWarning("Unknown notification event type: {Type}", notificationEvent.Type);
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to handle notification event of type {Type}", notificationEvent.Type);
            }
        }

        private async Task HandleUserNotification(INotificationEvent notificationEvent)
        {
            if (notificationEvent.UserId.HasValue)
            {
                var userGroup = $"user_{notificationEvent.UserId.Value}";
                await _hubContext.Clients.Group(userGroup).SendAsync("ReceiveNotification", notificationEvent.Data);
                
                _logger.LogInformation("Sent notification to user {UserId}", notificationEvent.UserId.Value);
            }
        }

        private async Task HandleBroadcastNotification(INotificationEvent notificationEvent)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveNotification", notificationEvent.Data);
            _logger.LogInformation("Sent broadcast notification");
        }

        private async Task HandleRoleNotification(INotificationEvent notificationEvent)
        {
            if (!string.IsNullOrEmpty(notificationEvent.Role))
            {
                var roleGroup = $"role_{notificationEvent.Role.ToLower()}";
                await _hubContext.Clients.Group(roleGroup).SendAsync("ReceiveNotification", notificationEvent.Data);
                
                _logger.LogInformation("Sent notification to role {Role}", notificationEvent.Role);
            }
        }

        private async Task HandleGroupNotification(INotificationEvent notificationEvent)
        {
            if (!string.IsNullOrEmpty(notificationEvent.GroupName))
            {
                await _hubContext.Clients.Group(notificationEvent.GroupName).SendAsync("ReceiveNotification", notificationEvent.Data);
                
                _logger.LogInformation("Sent notification to group {GroupName}", notificationEvent.GroupName);
            }
        }

        public async Task<bool> SendNotificationToUserAsync(int userId, object notification)
        {
            try
            {
                var userGroup = $"user_{userId}";
                await _hubContext.Clients.Group(userGroup).SendAsync("ReceiveNotification", notification);
                _logger.LogInformation("Sent real-time notification to user {UserId}", userId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send notification to user {UserId}", userId);
                return false;
            }
        }

        public async Task<bool> SendBroadcastNotificationAsync(object notification)
        {
            try
            {
                await _hubContext.Clients.All.SendAsync("ReceiveNotification", notification);
                _logger.LogInformation("Sent broadcast notification to all users");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send broadcast notification");
                return false;
            }
        }

        public async Task<bool> SendNotificationToRoleAsync(string role, object notification)
        {
            try
            {
                var roleGroup = $"role_{role.ToLower()}";
                await _hubContext.Clients.Group(roleGroup).SendAsync("ReceiveNotification", notification);
                _logger.LogInformation("Sent notification to role {Role}", role);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send notification to role {Role}", role);
                return false;
            }
        }

        public async Task SendNotificationToGroupAsync(string groupName, object notification)
        {
            await _hubContext.Clients.Group(groupName).SendAsync("ReceiveNotification", notification);
        }

        public async Task<RealTimeNotificationStatusDto> GetRealTimeNotificationStatusAsync(int userId)
        {
            var userGroup = $"user_{userId}";
            var isConnected = _userConnections.ContainsKey(userId) && _userConnections[userId].Any();
            
            return new RealTimeNotificationStatusDto
            {
                UserId = userId,
                IsConnected = isConnected,
                ConnectionCount = isConnected ? _userConnections[userId].Count : 0,
                LastSeen = DateTime.UtcNow // This would be tracked in a real implementation
            };
        }

        public async Task<NotificationHubStatusDto> GetNotificationHubStatusAsync()
        {
            // Use NotificationHub static data instead of local _userConnections
            var totalConnections = NotificationHub.ConnectedUsers.Count;
            var authenticatedUsers = NotificationHub.ConnectedUsers.Values.Count(u => u.IsAuthenticated);
            var anonymousConnections = totalConnections - authenticatedUsers;
            
            // Group by roles
            var roleGroups = NotificationHub.ConnectedUsers.Values
                .Where(u => u.IsAuthenticated)
                .GroupBy(u => u.UserRole)
                .ToDictionary(g => g.Key, g => g.Count());

            var userConnections = NotificationHub.UserConnections
                .Select(kvp => new
                {
                    UserId = kvp.Key,
                    ConnectionCount = kvp.Value.Count,
                    UserRole = NotificationHub.ConnectedUsers.Values
                        .FirstOrDefault(u => u.UserId == kvp.Key)?.UserRole ?? "Unknown"
                }).ToList<object>();

            return new NotificationHubStatusDto
            {
                TotalConnections = totalConnections,
                AuthenticatedUsers = authenticatedUsers,
                AnonymousConnections = anonymousConnections,
                RoleDistribution = roleGroups,
                UserConnections = userConnections,
                ActiveGroups = NotificationHub.ConnectedUsers.Values
                    .SelectMany(u => u.Groups)
                    .Distinct()
                    .ToList(),
                ServerTime = DateTime.UtcNow,
                Details = new Dictionary<string, object>
                {
                    { "ConnectionDetails", NotificationHub.ConnectedUsers.Values.Select(u => new 
                        {
                            u.ConnectionId,
                            u.UserId,
                            u.UserRole,
                            u.IsAuthenticated,
                            u.ConnectedAt,
                            GroupCount = u.Groups.Count
                        }).ToList() }
                }
            };
        }

        public void AddUserConnection(int userId, string connectionId)
        {
            _userConnections.AddOrUpdate(userId, 
                new List<string> { connectionId }, 
                (key, existing) => 
                {
                    existing.Add(connectionId);
                    return existing;
                });
        }

        public void RemoveUserConnection(int userId, string connectionId)
        {
            if (_userConnections.TryGetValue(userId, out var connections))
            {
                connections.Remove(connectionId);
                if (!connections.Any())
                {
                    _userConnections.TryRemove(userId, out _);
                }
            }
        }

        public bool IsUserConnected(int userId)
        {
            return NotificationHub.UserConnections.ContainsKey(userId) && 
                   NotificationHub.UserConnections[userId].Any();
        }

        public InfertilityTreatment.API.Hubs.UserConnectionInfo? GetUserConnectionInfo(int userId)
        {
            if (NotificationHub.UserConnections.TryGetValue(userId, out var connectionIds))
            {
                var firstConnectionId = connectionIds.FirstOrDefault();
                if (firstConnectionId != null && 
                    NotificationHub.ConnectedUsers.TryGetValue(firstConnectionId, out var connectionInfo))
                {
                    return new InfertilityTreatment.API.Hubs.UserConnectionInfo
                    {
                        ConnectionId = firstConnectionId,
                        UserId = userId,
                        UserRole = connectionInfo.UserRole,
                        ConnectedAt = connectionInfo.ConnectedAt,
                        IsAuthenticated = connectionInfo.IsAuthenticated,
                        Groups = connectionInfo.Groups,
                        ConnectionCount = connectionIds.Count
                    };
                }
            }
            return null;
        }

        public int GetConnectionCount()
        {
            return NotificationHub.ConnectedUsers.Count;
        }
    }
}
