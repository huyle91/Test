using InfertilityTreatment.Entity.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfertilityTreatment.Entity.Entities
{
    public class Prescription : BaseEntity
    {
        [Required]
        public int PhaseId { get; set; }
        [Required]
        public int MedicationId { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string Dosage { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string Frequency { get; set; }
        [Required]
        [Range(1, 365, ErrorMessage = "Duration must be between 1 and 365 days")]
        public int Duration { get; set; }
        [Required]
        [StringLength(500, MinimumLength = 1)]
        public string Instructions { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        // Navigation Property
        [ForeignKey(nameof(PhaseId))]
        public TreatmentPhase TreatmentPhase { get; set; }
        [ForeignKey(nameof(MedicationId))]
        public Medication Medication { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (EndDate <= StartDate)
            {
                yield return new ValidationResult(
                    "End date must be after start date",
                    new[] { nameof(EndDate) });
            }
        }
    }
}
