using System;

namespace InfertilityTreatment.Entity.DTOs.Payments
{
    public class CreatePaymentDto
    {
        public int CustomerId { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public int? AppointmentId { get; set; }
        public int? TreatmentCycleId { get; set; }
        public string ReturnUrl { get; set; }
        public string CancelUrl { get; set; }
    }
}
