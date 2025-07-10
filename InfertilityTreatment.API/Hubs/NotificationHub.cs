using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using System.Security.Claims;

namespace InfertilityTreatment.API.Hubs
{
    [Authorize] // Require authentication for SignalR hub
    public class NotificationHub : Hub
    {
        // Static collection to track connected users with detailed info
        public static readonly ConcurrentDictionary<string, UserConnectionInfo> ConnectedUsers = new();
        public static readonly ConcurrentDictionary<int, List<string>> UserConnections = new();
        
        private readonly ILogger<NotificationHub> _logger;

        public NotificationHub(ILogger<NotificationHub> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Join user to their specific group for receiving notifications
        /// </summary>
        /// <param name="userId">User ID to join group for</param>
        public async Task JoinUserGroup(string userId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");
            await Clients.Caller.SendAsync("JoinedGroup", $"user_{userId}");
        }

        /// <summary>
        /// Leave user group
        /// </summary>
        /// <param name="userId">User ID to leave group for</param>
        public async Task LeaveUserGroup(string userId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user_{userId}");
            await Clients.Caller.SendAsync("LeftGroup", $"user_{userId}");
        }

        /// <summary>
        /// Automatically join user to their group when connected (based on JWT claims)
        /// </summary>
        public override async Task OnConnectedAsync()
        {
            var connectionId = Context.ConnectionId;
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = Context.User?.FindFirst(ClaimTypes.Role)?.Value;
            var isAuthenticated = Context.User?.Identity?.IsAuthenticated ?? false;

            // Debug logging - let's see all claims
            _logger.LogInformation($"=== SignalR Connection Debug ===");
            _logger.LogInformation($"Connection ID: {connectionId}");
            _logger.LogInformation($"Is Authenticated: {isAuthenticated}");
            _logger.LogInformation($"User Identity Name: {Context.User?.Identity?.Name}");
            _logger.LogInformation($"Authentication Type: {Context.User?.Identity?.AuthenticationType}");
            
            if (Context.User?.Claims != null)
            {
                _logger.LogInformation("All Claims:");
                foreach (var claim in Context.User.Claims)
                {
                    _logger.LogInformation($"  {claim.Type} = {claim.Value}");
                }
            }
            else
            {
                _logger.LogWarning("No claims found in Context.User");
            }
            
            _logger.LogInformation($"Extracted UserId: {userId}");
            _logger.LogInformation($"Extracted UserRole: {userRole}");
            _logger.LogInformation($"=== End Debug ===");
            _logger.LogInformation($"UserId: {userId} ({userRole}) connected ");
            var connectionInfo = new UserConnectionInfo
            {
                ConnectionId = connectionId,
                UserId = isAuthenticated && int.TryParse(userId, out var id) ? id : 0,
                UserRole = userRole ?? "Anonymous",
                ConnectedAt = DateTime.UtcNow,
                IsAuthenticated = isAuthenticated,
                Groups = new List<string>()
            };

            ConnectedUsers.TryAdd(connectionId, connectionInfo);

            if (isAuthenticated && connectionInfo.UserId > 0)
            {
                // Track user connections
                UserConnections.AddOrUpdate(
                    connectionInfo.UserId,
                    new List<string> { connectionId },
                    (key, existing) => { existing.Add(connectionId); return existing; }
                );

                // Add to user-specific group
                await Groups.AddToGroupAsync(connectionId, $"user_{connectionInfo.UserId}");
                connectionInfo.Groups.Add($"user_{connectionInfo.UserId}");

                // Add to role-specific group
                if (!string.IsNullOrEmpty(userRole))
                {
                    await Groups.AddToGroupAsync(connectionId, $"role_{userRole.ToLower()}");
                    connectionInfo.Groups.Add($"role_{userRole.ToLower()}");
                }

                _logger.LogInformation($"Authenticated user {connectionInfo.UserId} ({userRole}) connected with connection {connectionId}");
                
                await Clients.Caller.SendAsync("Connected", new
                {
                    UserId = connectionInfo.UserId,
                    Role = userRole,
                    ConnectionId = connectionId,
                    ConnectedAt = DateTime.UtcNow,
                    IsAuthenticated = true
                });
            }
            else
            {
                _logger.LogInformation($"Anonymous user connected with connection {connectionId}");
                
                await Clients.Caller.SendAsync("Connected", new
                {
                    UserId = 0,
                    Role = "Anonymous",
                    ConnectionId = connectionId,
                    ConnectedAt = DateTime.UtcNow,
                    IsAuthenticated = false
                });
            }

            await base.OnConnectedAsync();
        }

        /// <summary>
        /// Handle disconnection
        /// </summary>
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var connectionId = Context.ConnectionId;
            
            if (ConnectedUsers.TryRemove(connectionId, out var userInfo))
            {
                if (userInfo.IsAuthenticated && userInfo.UserId > 0)
                {
                    // Remove from user connections
                    if (UserConnections.TryGetValue(userInfo.UserId, out var connections))
                    {
                        connections.Remove(connectionId);
                        if (!connections.Any())
                        {
                            UserConnections.TryRemove(userInfo.UserId, out _);
                        }
                    }

                    _logger.LogInformation($"Authenticated user {userInfo.UserId} ({userInfo.UserRole}) disconnected");
                }
                else
                {
                    _logger.LogInformation("Anonymous user disconnected");
                }
            }

            await base.OnDisconnectedAsync(exception);
        }

        /// <summary>
        /// Join doctor-specific group for medical notifications
        /// </summary>
        public async Task JoinDoctorGroup()
        {
            var userRole = Context.User?.FindFirst(ClaimTypes.Role)?.Value;
            if (userRole == "Doctor")
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, "doctors");
                await Clients.Caller.SendAsync("JoinedDoctorGroup", "doctors");
            }
        }

        /// <summary>
        /// Join admin group for administrative notifications
        /// </summary>
        public async Task JoinAdminGroup()
        {
            var userRole = Context.User?.FindFirst(ClaimTypes.Role)?.Value;
            if (userRole == "Admin" || userRole == "Manager")
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, "admins");
                await Clients.Caller.SendAsync("JoinedAdminGroup", "admins");
            }
        }

        /// <summary>
        /// Join customer group for patient notifications
        /// </summary>
        public async Task JoinCustomerGroup()
        {
            var userRole = Context.User?.FindFirst(ClaimTypes.Role)?.Value;
            if (userRole == "Customer")
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, "customers");
                await Clients.Caller.SendAsync("JoinedCustomerGroup", "customers");
            }
        }

        /// <summary>
        /// Mark notification as read
        /// </summary>
        /// <param name="notificationId">ID of notification to mark as read</param>
        public async Task MarkNotificationAsRead(int notificationId)
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (!string.IsNullOrEmpty(userId))
            {
                // This would typically call a service to update the database
                await Clients.Caller.SendAsync("NotificationMarkedAsRead", new
                {
                    NotificationId = notificationId,
                    UserId = userId,
                    MarkedAt = DateTime.UtcNow
                });
            }
        }

        /// <summary>
        /// Join a specific group by name
        /// </summary>
        /// <param name="groupName">Name of the group to join</param>
        public async Task JoinGroup(string groupName)
        {
            try
            {
                var connectionId = Context.ConnectionId;
                
                if (ConnectedUsers.TryGetValue(connectionId, out var userInfo))
                {
                    await Groups.AddToGroupAsync(connectionId, groupName);
                    userInfo.Groups.Add(groupName);
                    
                    var userName = userInfo.IsAuthenticated ? $"User {userInfo.UserId} ({userInfo.UserRole})" : "Anonymous";
                    
                    _logger.LogInformation($"{userName} joined group: {groupName}");

                    
                    await Clients.Caller.SendAsync("JoinedGroup", groupName);
                }
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("Error", $"Failed to join group {groupName}: {ex.Message}");
            }
        }

        /// <summary>
        /// Leave a specific group by name
        /// </summary>
        /// <param name="groupName">Name of the group to leave</param>
        public async Task LeaveGroup(string groupName)
        {
            try
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
                await Clients.Caller.SendAsync("LeftGroup", groupName);
                
                // Log the group leave
                var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Anonymous";
                Console.WriteLine($"User {userId} left group: {groupName}");
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("Error", $"Failed to leave group {groupName}: {ex.Message}");
            }
        }

        /// <summary>
        /// Ping method for testing connection
        /// </summary>
        public async Task Ping()
        {
            await Clients.Caller.SendAsync("Pong", new
            {
                Message = "Connection is alive!",
                ServerTime = DateTime.UtcNow,
                ConnectionId = Context.ConnectionId
            });
        }

        /// <summary>
        /// Get connection info for debugging
        /// </summary>
        public async Task GetConnectionInfo()
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = Context.User?.FindFirst(ClaimTypes.Role)?.Value;
            var userName = Context.User?.FindFirst(ClaimTypes.Name)?.Value;

            await Clients.Caller.SendAsync("ConnectionInfo", new
            {
                ConnectionId = Context.ConnectionId,
                UserId = userId,
                UserRole = userRole,
                UserName = userName,
                ConnectedAt = DateTime.UtcNow,
                IsAuthenticated = Context.User?.Identity?.IsAuthenticated ?? false
            });
        }
    }

    public class UserConnectionInfo
    {
        public string ConnectionId { get; set; } = string.Empty;
        public int UserId { get; set; }
        public string UserRole { get; set; } = string.Empty;
        public DateTime ConnectedAt { get; set; }
        public bool IsAuthenticated { get; set; }
        public List<string> Groups { get; set; } = new();
        public int ConnectionCount { get; set; } = 1; // For compatibility
    }
}
