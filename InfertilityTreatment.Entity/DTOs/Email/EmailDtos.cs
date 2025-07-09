using System.ComponentModel.DataAnnotations;

namespace InfertilityTreatment.Entity.DTOs.Email
{
    public class SendReminderDto
    {
        [Required]
        public int AppointmentId { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public string? CustomMessage { get; set; }
    }

    public class SendTestResultDto
    {
        [Required]
        public int TestResultId { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public string? CustomMessage { get; set; }
    }

    public class BulkEmailDto
    {
        [Required]
        [StringLength(200)]
        public string Subject { get; set; } = string.Empty;

        [Required]
        public string Message { get; set; } = string.Empty;

        [Required]
        [MinLength(1, ErrorMessage = "At least one recipient is required")]
        public List<string> Recipients { get; set; } = new();

        public bool IsHtml { get; set; } = true;

        public List<string>? UserRoles { get; set; }

        public List<int>? SpecificUserIds { get; set; }
    }

    public class EmailResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<string> Errors { get; set; } = new();
        public int SuccessCount { get; set; }
        public int FailureCount { get; set; }
    }

    public class SendWelcomeEmailDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string CustomerName { get; set; } = string.Empty;
    }

    public class SendAppointmentConfirmationDto
    {
        [Required]
        public int AppointmentId { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }

    public class SendTreatmentPhaseUpdateDto
    {
        [Required]
        public int TreatmentPhaseId { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public string? CustomMessage { get; set; }
    }

    public class SendPaymentConfirmationDto
    {
        [Required]
        public int PaymentId { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
    public class TestEmailDto
    {
        public string Email { get; set; } = string.Empty;
    }
}
