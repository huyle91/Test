using Microsoft.Extensions.Logging;
using InfertilityTreatment.Business.Interfaces;
using InfertilityTreatment.Entity.Enums;
using System.Text.Json;

namespace InfertilityTreatment.Business.Services
{
    public class AuditLogService : IAuditLogService
    {
        private readonly ILogger<AuditLogService> _logger;

        public AuditLogService(ILogger<AuditLogService> logger)
        {
            _logger = logger;
        }

        public async Task LogPaymentTransactionAsync(int customerId, string paymentId, PaymentStatus status, decimal amount, string paymentMethod, string? transactionId = null)
        {
            var logData = new
            {
                LogType = "PaymentTransaction",
                CustomerId = customerId,
                PaymentId = paymentId,
                Status = status.ToString(),
                Amount = amount,
                PaymentMethod = paymentMethod,
                TransactionId = transactionId,
                Timestamp = DateTime.UtcNow
            };

            _logger.LogInformation("Payment Transaction: {LogData}", JsonSerializer.Serialize(logData));
        }

        public async Task LogBookingProcessAsync(int customerId, int? appointmentId, int? treatmentCycleId, string action, bool success, string? errorMessage = null)
        {
            var logData = new
            {
                LogType = "BookingProcess",
                CustomerId = customerId,
                AppointmentId = appointmentId,
                TreatmentCycleId = treatmentCycleId,
                Action = action,
                Success = success,
                ErrorMessage = errorMessage,
                Timestamp = DateTime.UtcNow
            };

            if (success)
            {
                _logger.LogInformation("Booking Process Success: {LogData}", JsonSerializer.Serialize(logData));
            }
            else
            {
                _logger.LogWarning("Booking Process Failed: {LogData}", JsonSerializer.Serialize(logData));
            }
        }

        public async Task LogErrorAsync(string source, string message, Exception? exception = null, int? userId = null)
        {
            var logData = new
            {
                LogType = "Error",
                Source = source,
                Message = message,
                UserId = userId,
                ExceptionType = exception?.GetType().Name,
                ExceptionMessage = exception?.Message,
                StackTrace = exception?.StackTrace,
                Timestamp = DateTime.UtcNow
            };

            _logger.LogError(exception, "Application Error: {LogData}", JsonSerializer.Serialize(logData));
        }

        public async Task LogUserActionAsync(int userId, string action, string? details = null, string? ipAddress = null)
        {
            var logData = new
            {
                LogType = "UserAction",
                UserId = userId,
                Action = action,
                Details = details,
                IpAddress = ipAddress,
                Timestamp = DateTime.UtcNow
            };

            _logger.LogInformation("User Action: {LogData}", JsonSerializer.Serialize(logData));
        }

        public async Task LogPerformanceAsync(string operation, long durationMs, bool success, string? additionalInfo = null)
        {
            var logData = new
            {
                LogType = "Performance",
                Operation = operation,
                DurationMs = durationMs,
                Success = success,
                AdditionalInfo = additionalInfo,
                Timestamp = DateTime.UtcNow
            };

            if (durationMs > 5000) // Log slow operations (> 5 seconds)
            {
                _logger.LogWarning("Slow Operation: {LogData}", JsonSerializer.Serialize(logData));
            }
            else
            {
                _logger.LogInformation("Performance Log: {LogData}", JsonSerializer.Serialize(logData));
            }
        }

        public async Task LogDataAccessAsync(int? userId, string table, string operation, int? recordId = null)
        {
            var logData = new
            {
                LogType = "DataAccess",
                UserId = userId,
                Table = table,
                Operation = operation, // CREATE, READ, UPDATE, DELETE
                RecordId = recordId,
                Timestamp = DateTime.UtcNow
            };

            _logger.LogDebug("Data Access: {LogData}", JsonSerializer.Serialize(logData));
        }

        public async Task LogSecurityEventAsync(string eventType, int? userId = null, string? ipAddress = null, string? details = null)
        {
            var logData = new
            {
                LogType = "Security",
                EventType = eventType, // LOGIN_SUCCESS, LOGIN_FAILED, UNAUTHORIZED_ACCESS, etc.
                UserId = userId,
                IpAddress = ipAddress,
                Details = details,
                Timestamp = DateTime.UtcNow
            };

            _logger.LogWarning("Security Event: {LogData}", JsonSerializer.Serialize(logData));
        }

        public async Task LogSystemEventAsync(string eventType, string message, object? data = null)
        {
            var logData = new
            {
                LogType = "System",
                EventType = eventType,
                Message = message,
                Data = data,
                Timestamp = DateTime.UtcNow
            };

            _logger.LogInformation("System Event: {LogData}", JsonSerializer.Serialize(logData));
        }
    }
}
