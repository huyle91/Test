using System;
using System.ComponentModel.DataAnnotations;

namespace InfertilityTreatment.Entity.DTOs.TreatmentPhase
{
    public class CompletePhaseDto
    {
        [Required]
        public DateTime EndDate { get; set; }
        
        public string? Results { get; set; }
        
        public string? NextPhaseInstructions { get; set; }
        
        public string? Notes { get; set; }
    }
}
