using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using InfertilityTreatment.Entity.Common;

namespace InfertilityTreatment.Entity.Entities
{
    public class Doctor : BaseEntity
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        [MaxLength(100)]
        public string LicenseNumber { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Specialization { get; set; }

        public int YearsOfExperience { get; set; }

        [MaxLength(500)]
        public string? Education { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string? Biography { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? ConsultationFee { get; set; }

        public bool IsAvailable { get; set; } = true;

        [Column(TypeName = "decimal(5,2)")]
        public decimal? SuccessRate { get; set; }

        // Navigation Properties
        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; } = null!;

        public virtual ICollection<TreatmentCycle> TreatmentCycles { get; set; } = new List<TreatmentCycle>();
        public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        //public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}