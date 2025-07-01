using System;

namespace InfertilityTreatment.Entity.DTOs.Payments
{
    public class PaymentResponseDto
    {
        public string PaymentId { get; set; }
        public string PaymentUrl { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedAt { get; set; }
        public string TransactionId { get; set; }
    }
}
