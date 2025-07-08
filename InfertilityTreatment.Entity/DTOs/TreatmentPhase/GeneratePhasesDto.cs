using InfertilityTreatment.Entity.Enums;
using System.ComponentModel.DataAnnotations;

namespace InfertilityTreatment.Entity.DTOs.TreatmentPhase
{
    public class GeneratePhasesDto
    {
        [Required]
        public TreatmentType TreatmentType { get; set; }
        
        public bool UseCustomSettings { get; set; } = false;
        
        /// <summary>
        /// Custom phase names and settings (optional)
        /// Format: [{"PhaseName": "Phase 1", "Order": 1, "EstimatedDuration": 7}]
        /// </summary>
        public string? CustomSettings { get; set; }
    }
}
