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
        public string Dosage { get; set; }
        [Required]
        public string Frequency { get; set; }
        [Required]
        public int Duration { get; set; }
        [Required]
        public string Instructions { get; set; }
        [Required]

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        // Navigation Property
        [ForeignKey(nameof(PhaseId))]
        public TreatmentPhase TreatmentPhase { get; set; }
        [ForeignKey(nameof(MedicationId))]
        public Medication Medication { get; set; }
    }
}
