using System;

namespace InfertilityTreatment.Entity.DTOs.Payments
{
    public class CreatePaymentDto
    {
        public int CustomerId { get; set; }
        public int TreatmentPackageId { get; set; }
        public string ReturnUrl { get; set; }
        public string CancelUrl { get; set; }
    }
}
