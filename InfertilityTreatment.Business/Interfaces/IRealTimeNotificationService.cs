using InfertilityTreatment.Entity.DTOs.Notifications;

namespace InfertilityTreatment.Business.Interfaces
{
    /// <summary>
    /// Service for sending real-time notifications to users via SignalR
    /// </summary>
    public interface IRealTimeNotificationService
    {
        /// <summary>
        /// Send notification to a specific user
        /// </summary>
        /// <param name="userId">Target user ID</param>
        /// <param name="notification">Notification data</param>
        /// <returns>True if sent successfully</returns>
        Task<bool> SendNotificationToUserAsync(int userId, object notification);

        /// <summary>
        /// Send notification to all users with a specific role
        /// </summary>
        /// <param name="role">Target role</param>
        /// <param name="notification">Notification data</param>
        /// <returns>True if sent successfully</returns>
        Task<bool> SendNotificationToRoleAsync(string role, object notification);

        /// <summary>
        /// Send notification to all connected users
        /// </summary>
        /// <param name="notification">Notification data</param>
        /// <returns>True if sent successfully</returns>
        Task<bool> SendBroadcastNotificationAsync(object notification);
    }
}
