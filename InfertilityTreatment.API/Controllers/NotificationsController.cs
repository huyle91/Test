using Microsoft.AspNetCore.Mvc;
using InfertilityTreatment.Entity.DTOs.Notifications;
using InfertilityTreatment.Business.Interfaces;
using System.Security.Claims;
using InfertilityTreatment.Entity.Enums;
using Microsoft.AspNetCore.Authorization;
using InfertilityTreatment.Data.Repositories.Interfaces;
using InfertilityTreatment.Entity.Entities;
using InfertilityTreatment.Entity.DTOs.Common;
using Microsoft.Extensions.Logging;
using FluentValidation;
using InfertilityTreatment.API.Services;


namespace InfertilityTreatment.API.Controllers
{
    [ApiController]
    [Route("api/notifications/")]
    [Authorize]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        private readonly SignalRNotificationService _signalRService;
        private readonly IBaseRepository<User> _userRepository;
        private readonly ILogger<NotificationsController> _logger;
        private readonly IValidator<BroadcastNotificationDto> _broadcastValidator;
        private readonly IValidator<ScheduleNotificationDto> _scheduleValidator;

        public NotificationsController(
            INotificationService notificationService,
            SignalRNotificationService signalRService,
            IBaseRepository<User> userRepository,
            ILogger<NotificationsController> logger,
            IValidator<BroadcastNotificationDto> broadcastValidator,
            IValidator<ScheduleNotificationDto> scheduleValidator)
        {
            _notificationService = notificationService;
            _signalRService = signalRService;
            _userRepository = userRepository;
            _logger = logger;
            _broadcastValidator = broadcastValidator;
            _scheduleValidator = scheduleValidator;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<NotificationResponseDto>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetUserNotifications()
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized(new { success = false, 
                    message = "User not authenticated or ID not found in claims." });
            }

            var notifications = await _notificationService.GetUserNotificationsAsync(userId.Value);

            if (notifications == null || !notifications.Any())
            {
                return Ok(new { success = true, 
                    message = "No notifications found for this user.", data = new List<NotificationResponseDto>() });
            }
            return Ok(new { success = true, message = "Notifications retrieved successfully.", data = notifications });
        }

        [HttpPut("{id}/mark-read")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> MarkNotificationAsRead(int id)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized(new { success = false, 
                    message = "User not authenticated or ID not found in claims." });
            }

            var notification = await _notificationService.GetNotificationIfOwnedAsync(id, userId.Value);
            if (notification == null || notification.UserId != userId.Value)
                return NotFound(new { success = false,
                    message = "You do not have permission to mark this notification as read or notification not found." });

            var success = await _notificationService.MarkAsReadAsync(id);
            if (!success)
            {
                return NotFound(new { success = false, 
                    message = $"Notification with ID {id} not found or could not be marked as read." });
            }
            return Ok(new { success = true, message = $"Notification {id} marked as read successfully." });
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteNotification(int id)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized(new { success = false,
                    message = "User not authenticated or ID not found in claims." });
            }

            var success = await _notificationService.DeleteNotificationAsync(id);
            if (!success)
            {
                return NotFound(new { success = false,
                    message = $"Notification with ID {id} not found or could not be deleted." });
            }
            return Ok(new { success = true, message = $"Notification {id} deleted successfully." });
        }
        private int? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out var userId))
            {
                return userId;
            }
            return null;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(Roles = nameof(UserRole.Doctor))]
        public async Task<IActionResult> CreateNotification([FromBody] CreateNotificationDto createDto)
        {
            var currentUserId = GetCurrentUserId();
            var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Validation failed.", 
                    errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList() });
            }
            if (currentUserId == null || currentUserRole == null)
            {
                return Unauthorized(new { success = false, message = "User not authenticated or role not found." });
            }
            if (currentUserRole != UserRole.Doctor.ToString())
            {
                return StatusCode(StatusCodes.Status403Forbidden,
                    new { success = false, message = "Only doctors are allowed to create notifications." });
            }

            var targetUser = await _userRepository.GetByIdAsync(createDto.UserId);
            if (targetUser == null)
            {
                return BadRequest(new { success = false, message = "Target user for notification not found." });
            }
            if (targetUser.Role != UserRole.Customer)
            {
                return StatusCode(StatusCodes.Status403Forbidden, 
                    new { success = false, message = "Doctors can only create notifications for customers." });
            }

            var success = await _notificationService.CreateNotificationAsync(createDto);
            if (!success)
            {
                return BadRequest(new { success = false, 
                    message = "Failed to create notification due to an internal service error." });
            }
            return StatusCode(StatusCodes.Status201Created, 
                new { success = true, message = "Notification created successfully." });
        }

        #region Enhanced Real-time Notification APIs

        /// <summary>
        /// Broadcast notification to all users or specific roles
        /// </summary>
        [HttpPost("broadcast")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> BroadcastNotification([FromBody] BroadcastNotificationDto dto)
        {
            try
            {
                var validationResult = await _broadcastValidator.ValidateAsync(dto);
                if (!validationResult.IsValid)
                {
                    return BadRequest(ApiResponseDto<string>.CreateError("Validation failed", 
                        validationResult.Errors.Select(e => e.ErrorMessage).ToList()));
                }

                var notification = new
                {
                    Id = Guid.NewGuid().ToString(),
                    Title = dto.Title,
                    Message = dto.Message,
                    Type = dto.Type,
                    Priority = dto.Priority,
                    Timestamp = DateTime.UtcNow,
                    
                    ExpiresAt = dto.ExpiresAt,
                    IsBroadcast = true
                };

                if (dto.TargetRoles != null && dto.TargetRoles.Any())
                {
                    // Send to specific roles
                    foreach (var role in dto.TargetRoles)
                    {
                        await _signalRService.SendNotificationToRoleAsync(role, notification);
                    }
                    
                    _logger.LogInformation("Broadcast notification sent to roles: {Roles}", string.Join(", ", dto.TargetRoles));
                }
                else
                {
                    // Send to all users
                    await _signalRService.SendBroadcastNotificationAsync(notification);
                    _logger.LogInformation("Broadcast notification sent to all users");
                }

               

                return Ok(ApiResponseDto<object>.CreateSuccess(notification, "Broadcast notification sent successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send broadcast notification");
                return StatusCode(500, ApiResponseDto<string>.CreateError("Failed to send broadcast notification"));
            }
        }

        /// <summary>
        /// Schedule a notification for later delivery
        /// </summary>
        [HttpPost("schedule")]
        [Authorize(Roles = "Doctor,Admin,Manager")]
        public async Task<IActionResult> ScheduleNotification([FromBody] ScheduleNotificationDto dto)
        {
            try
            {
                var validationResult = await _scheduleValidator.ValidateAsync(dto);
                if (!validationResult.IsValid)
                {
                    return BadRequest(ApiResponseDto<string>.CreateError("Validation failed", 
                        validationResult.Errors.Select(e => e.ErrorMessage).ToList()));
                }

                if (dto.ScheduledAt <= DateTime.UtcNow)
                {
                    return BadRequest(ApiResponseDto<string>.CreateError("Scheduled time must be in the future"));
                }

                // Validate target user exists
                var targetUser = await _userRepository.GetByIdAsync(dto.UserId);
                if (targetUser == null)
                {
                    return BadRequest(ApiResponseDto<string>.CreateError("Target user not found"));
                }

                // Create scheduled notification in database
                var createDto = new CreateNotificationDto
                {
                    UserId = dto.UserId,
                    Title = dto.Title,
                    Message = dto.Message,
                    Type = dto.Type.ToString(),
                    RelatedEntityId = dto.RelatedEntityId,
                    RelatedEntityType = dto.RelatedEntityType,
                    ScheduledAt = dto.ScheduledAt
                };

                var success = await _notificationService.CreateNotificationAsync(createDto);
                if (!success)
                {
                    return StatusCode(500, ApiResponseDto<string>.CreateError("Failed to schedule notification"));
                }

               
                
                var response = new
                {
                    UserId = dto.UserId,
                    Title = dto.Title,
                    Type = dto.Type,
                    Priority = dto.Priority,
                    ScheduledAt = dto.ScheduledAt,
                    SendEmail = dto.SendEmail,
                    Status = "Scheduled"
                };

                _logger.LogInformation("Notification scheduled for user {UserId} at {ScheduledAt}", 
                    dto.UserId, dto.ScheduledAt);

                return Ok(ApiResponseDto<object>.CreateSuccess(response, "Notification scheduled successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to schedule notification for user {UserId}", dto.UserId);
                return StatusCode(500, ApiResponseDto<string>.CreateError("Failed to schedule notification"));
            }
        }

        /// <summary>
        /// Get real-time notification status for a user
        /// </summary>
        [HttpGet("user/{userId}/real-time-status")]
        [AllowAnonymous] // Allow for testing
        public async Task<IActionResult> GetRealTimeNotificationStatus(int userId)
        {
            try
            {
                // Check if user is connected using NotificationHub data
                var isConnected = _signalRService.IsUserConnected(userId);
                var connectionInfo = _signalRService.GetUserConnectionInfo(userId);
                
                // Validate target user exists
                var targetUser = await _userRepository.GetByIdAsync(userId);
                if (targetUser == null)
                {
                    return NotFound(ApiResponseDto<string>.CreateError("User not found"));
                }

                // Get user notifications
                var notifications = await _notificationService.GetUserNotificationsAsync(userId);
                var unreadCount = notifications.Count(n => !n.IsRead);

                var status = new RealTimeNotificationStatusDto
                {
                    UserId = userId,
                    IsConnected = isConnected,
                    LastConnectedAt = connectionInfo?.ConnectedAt ?? DateTime.UtcNow,
                    ConnectionId = connectionInfo?.ConnectionId ?? "none",
                    ActiveGroups = connectionInfo?.Groups ?? new List<string>(),
                    UnreadCount = unreadCount,
                    TotalNotifications = notifications.Count,
                    RecentNotifications = notifications.OrderByDescending(n => n.CreatedAt).Take(5).ToList(),
                    ConnectionInfo = new Dictionary<string, object>
                    {
                        { "UserAgent", Request.Headers["User-Agent"].ToString() },
                        { "IPAddress", HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown" },
                        { "CheckedAt", DateTime.UtcNow },
                        { "IsRealTimeConnected", isConnected },
                        { "ConnectionCount", connectionInfo?.ConnectionCount ?? 0 }
                    }
                };

                return Ok(ApiResponseDto<RealTimeNotificationStatusDto>.CreateSuccess(status, 
                    "Real-time notification status retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get real-time notification status for user {UserId}", userId);
                return StatusCode(500, ApiResponseDto<string>.CreateError("Failed to get notification status"));
            }
        }

        /// <summary>
        /// Get SignalR hub status (All authenticated users can access)
        /// </summary>
        [HttpGet("hub-status")]
        [AllowAnonymous] // Allow anyone to check hub status for testing
        public async Task<IActionResult> GetHubStatus()
        {
            try
            {
                var hubStatus = await _signalRService.GetNotificationHubStatusAsync();

                return Ok(ApiResponseDto<NotificationHubStatusDto>.CreateSuccess(hubStatus, 
                    "SignalR hub status retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get SignalR hub status");
                return StatusCode(500, ApiResponseDto<string>.CreateError("Failed to get hub status"));
            }
        }

        #endregion

        // ...existing private methods...
    }
}

