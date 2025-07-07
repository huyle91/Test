using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using InfertilityTreatment.Entity.Common;

namespace InfertilityTreatment.Entity.Entities
{
    public class InvoiceItem : BaseEntity
    {
        [Required]
        public int InvoiceId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public int Quantity { get; set; } = 1;

        [Required]
        [Column(TypeName = "decimal(12,2)")]
        public decimal UnitPrice { get; set; }

        [Required]
        [Column(TypeName = "decimal(12,2)")]
        public decimal TotalPrice { get; set; }

        public int? TreatmentServiceId { get; set; }

        public int? MedicationId { get; set; }

        // Navigation Properties
        [ForeignKey(nameof(InvoiceId))]
        public virtual Invoice Invoice { get; set; } = null!;

        [ForeignKey(nameof(TreatmentServiceId))]
        public virtual TreatmentService? TreatmentService { get; set; }

        [ForeignKey(nameof(MedicationId))]
        public virtual Medication? Medication { get; set; }
    }
}
