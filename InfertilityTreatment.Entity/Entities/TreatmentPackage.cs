using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using InfertilityTreatment.Entity.Common;

namespace InfertilityTreatment.Entity.Entities
{
    public class TreatmentPackage : BaseEntity
    {
        [Required]
        public int ServiceId { get; set; }

        [Required]
        [MaxLength(200)]
        public string PackageName { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(max)")]
        public string? Description { get; set; }

        [Required]
        [Column(TypeName = "decimal(12,2)")]
        public decimal Price { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string? IncludedServices { get; set; } // JSON format

        public int? DurationWeeks { get; set; }

        // Navigation Properties
        [ForeignKey(nameof(ServiceId))]
        public virtual TreatmentService TreatmentService { get; set; } = null!;

        public virtual ICollection<TreatmentCycle> TreatmentCycles { get; set; } = new List<TreatmentCycle>();
    }
}