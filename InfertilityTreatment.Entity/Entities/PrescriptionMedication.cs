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
    public class PrescriptionMedication : BaseEntity
    {
        [Required]
        public int PrescriptionId { get; set; }
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

        // Navigation Properties
        [ForeignKey(nameof(PrescriptionId))]
        public Prescription Prescription { get; set; }
        [ForeignKey(nameof(MedicationId))]
        public Medication Medication { get; set; }
    }
}
