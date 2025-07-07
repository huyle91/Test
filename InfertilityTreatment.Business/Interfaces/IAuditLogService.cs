using InfertilityTreatment.Entity.Enums;

namespace InfertilityTreatment.Business.Interfaces
{
    public interface IAuditLogService
    {
        Task LogPaymentTransactionAsync(int customerId, string paymentId, PaymentStatus status, decimal amount, string paymentMethod, string? transactionId = null);
        Task LogBookingProcessAsync(int customerId, int? appointmentId, int? treatmentCycleId, string action, bool success, string? errorMessage = null);
        Task LogErrorAsync(string source, string message, Exception? exception = null, int? userId = null);
        Task LogUserActionAsync(int userId, string action, string? details = null, string? ipAddress = null);
        Task LogPerformanceAsync(string operation, long durationMs, bool success, string? additionalInfo = null);
        Task LogDataAccessAsync(int? userId, string table, string operation, int? recordId = null);
        Task LogSecurityEventAsync(string eventType, int? userId = null, string? ipAddress = null, string? details = null);
        Task LogSystemEventAsync(string eventType, string message, object? data = null);
    }
}
