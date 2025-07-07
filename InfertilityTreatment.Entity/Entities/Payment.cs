using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using InfertilityTreatment.Entity.Common;
using InfertilityTreatment.Entity.Enums;

namespace InfertilityTreatment.Entity.Entities
{
    public class Payment : BaseEntity
    {
        [Required]
        [MaxLength(50)]
        public string PaymentId { get; set; } = string.Empty;

        [Required]
        public int CustomerId { get; set; }

        public int? TreatmentCycleId { get; set; }

        public int? AppointmentId { get; set; }

        [Required]
        [Column(TypeName = "decimal(12,2)")]
        public decimal Amount { get; set; }

        [Required]
        [MaxLength(50)]
        public string PaymentMethod { get; set; } = string.Empty; // VNPay, Momo, Cash, etc.

        [Required]
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

        [MaxLength(100)]
        public string? TransactionId { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        public DateTime? ProcessedAt { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string? Notes { get; set; }

        // Navigation Properties
        [ForeignKey(nameof(CustomerId))]
        public virtual Customer Customer { get; set; } = null!;

        [ForeignKey(nameof(TreatmentCycleId))]
        public virtual TreatmentCycle? TreatmentCycle { get; set; }

        [ForeignKey(nameof(AppointmentId))]
        public virtual Appointment? Appointment { get; set; }

        public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
    }
}
