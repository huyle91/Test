using System;

namespace InfertilityTreatment.Entity.DTOs.Payments
{
    public class PaymentDto
    {
        public int Id { get; set; }
        public string PaymentId { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; }
        public string PaymentMethod { get; set; }
        public string Description { get; set; }
        public string TransactionId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public int? AppointmentId { get; set; }
        public int? TreatmentCycleId { get; set; }
    }
}
