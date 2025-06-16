using Microsoft.AspNetCore.Mvc;
using InfertilityTreatment.Entity.DTOs.Notifications;
using InfertilityTreatment.Business.Interfaces;
using System.Security.Claims;
using InfertilityTreatment.Entity.Enums;

namespace InfertilityTreatment.API.Controllers
{
    [ApiController]
    [Route("api/notifications/")]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<NotificationResponseDto>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetUserNotifications()
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized(new { success = false, message = "User not authenticated or ID not found in claims." });
            }

            var notifications = await _notificationService.GetUserNotificationsAsync(userId.Value);
            return Ok(new { success = true, message = "Notifications retrieved successfully.", data = notifications });
        }

        [HttpPut("{id}/mark-read")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> MarkNotificationAsRead(int id)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized(new { success = false, message = "User not authenticated or ID not found in claims." });
            }

            var success = await _notificationService.MarkAsReadAsync(id);
            if (!success)
            {
                return NotFound(new { success = false, message = $"Notification with ID {id} not found or could not be marked as read." });
            }
            return Ok(new { success = true, message = $"Notification {id} marked as read successfully." });
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> DeleteNotification(int id)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized(new { success = false, message = "User not authenticated or ID not found in claims." });
            }

            var success = await _notificationService.DeleteNotificationAsync(id);
            if (!success)
            {
                return NotFound(new { success = false, message = $"Notification with ID {id} not found or could not be deleted." });
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
        //[HttpPost]
        //[ProducesResponseType(StatusCodes.Status201Created)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //public async Task<IActionResult> CreateNotification([FromBody] CreateNotificationDto createDto)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    var success = await _notificationService.CreateNotificationAsync(createDto);
        //    if (!success)
        //    {
        //        return BadRequest("Failed to create notification.");
        //    }
        //    return StatusCode(StatusCodes.Status201Created, new { message = "Notification created successfully." });
        //}
    }
}

