using InfertilityTreatment.Entity.DTOs.Common;
using InfertilityTreatment.Entity.Enums;

namespace InfertilityTreatment.Entity.DTOs.Payments
{
    public class PaymentHistoryDto
    {
        public string PaymentId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public PaymentStatus Status { get; set; }
        public string? TransactionId { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ProcessedAt { get; set; }
    }

    public class PaymentStatusDto
    {
        public string PaymentId { get; set; } = string.Empty;
        public PaymentStatus Status { get; set; }
        public string? TransactionId { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ProcessedAt { get; set; }
        public string? Notes { get; set; }
    }

    public class RefundRequestDto
    {
        public string PaymentId { get; set; } = string.Empty;
        public decimal RefundAmount { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string AdminUserId { get; set; } = string.Empty;
    }

    public class RefundResponseDto
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? RefundTransactionId { get; set; }
        public decimal RefundAmount { get; set; }
        public DateTime ProcessedAt { get; set; }
    }
}
