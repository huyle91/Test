using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using InfertilityTreatment.Entity.Common;

namespace InfertilityTreatment.Entity.Entities
{
    public class PaymentLog : BaseEntity
    {
        [Required]
        public int PaymentId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Action { get; set; } = string.Empty; // Created, Updated, Callback, Refunded, etc.

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = string.Empty;

        [Column(TypeName = "nvarchar(max)")]
        public string? RequestData { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string? ResponseData { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }

        // Navigation Properties
        [ForeignKey(nameof(PaymentId))]
        public virtual Payment Payment { get; set; } = null!;
    }
}
