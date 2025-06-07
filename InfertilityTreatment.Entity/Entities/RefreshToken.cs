using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using InfertilityTreatment.Entity.Common;

namespace InfertilityTreatment.Entity.Entities
{
    public class RefreshToken : BaseEntity
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        [MaxLength(500)]
        public string Token { get; set; } = string.Empty;

        [Required]
        public DateTime ExpiresAt { get; set; }

        // Navigation Properties
        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; } = null!;
    }
}