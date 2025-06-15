using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using InfertilityTreatment.Entity.Common;

namespace InfertilityTreatment.Entity.Entities
{
    public class TreatmentService : BaseEntity
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty; // IUI, IVF, ...

        [Column(TypeName = "nvarchar(max)")]
        public string? Description { get; set; }

        [Column(TypeName = "decimal(12,2)")]
        public decimal? BasePrice { get; set; }

        public int? EstimatedDuration { get; set; } 

        [Column(TypeName = "nvarchar(max)")]
        public string? Requirements { get; set; } 

        // Navigation Properties
        public virtual ICollection<TreatmentPackage> TreatmentPackages { get; set; } = new List<TreatmentPackage>();
    }
}