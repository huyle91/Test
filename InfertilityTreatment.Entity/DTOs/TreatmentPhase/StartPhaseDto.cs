using System;
using System.ComponentModel.DataAnnotations;

namespace InfertilityTreatment.Entity.DTOs.TreatmentPhase
{
    public class StartPhaseDto
    {
        [Required]
        public DateTime StartDate { get; set; }
        
        public string? Instructions { get; set; }
        
        public string? Notes { get; set; }
    }
}
