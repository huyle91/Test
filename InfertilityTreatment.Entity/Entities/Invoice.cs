using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using InfertilityTreatment.Entity.Common;
using InfertilityTreatment.Entity.Enums;

namespace InfertilityTreatment.Entity.Entities
{
    public class Invoice : BaseEntity
    {
        [Required]
        [MaxLength(50)]
        public string InvoiceNumber { get; set; } = string.Empty;

        [Required]
        public int CustomerId { get; set; }

        public int? AppointmentId { get; set; }

        public int? TreatmentCycleId { get; set; }

        [MaxLength(50)]
        public string? PaymentId { get; set; }

        [Required]
        [Column(TypeName = "decimal(12,2)")]
        public decimal SubTotal { get; set; }

        [Column(TypeName = "decimal(12,2)")]
        public decimal TaxAmount { get; set; } = 0;

        [Column(TypeName = "decimal(12,2)")]
        public decimal DiscountAmount { get; set; } = 0;

        [Required]
        [Column(TypeName = "decimal(12,2)")]
        public decimal TotalAmount { get; set; }

        [Required]
        public InvoiceStatus Status { get; set; } = InvoiceStatus.Draft;

        public DateTime IssueDate { get; set; } = DateTime.UtcNow;

        public DateTime? DueDate { get; set; }

        public DateTime? PaidDate { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string? Notes { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string? Terms { get; set; }

        // Navigation Properties
        [ForeignKey(nameof(CustomerId))]
        public virtual Customer Customer { get; set; } = null!;

        [ForeignKey(nameof(AppointmentId))]
        public virtual Appointment? Appointment { get; set; }

        [ForeignKey(nameof(TreatmentCycleId))]
        public virtual TreatmentCycle? TreatmentCycle { get; set; }

        [ForeignKey(nameof(PaymentId))]
        public virtual Payment? Payment { get; set; }

        public virtual ICollection<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();
    }
}
