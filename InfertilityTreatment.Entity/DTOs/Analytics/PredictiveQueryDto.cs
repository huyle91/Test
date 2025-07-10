using System;
using System.ComponentModel.DataAnnotations;

namespace InfertilityTreatment.Entity.DTOs.Analytics
{
    public class PredictiveQueryDto
    {
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        [Required]
        [StringLength(100)]
        public string PredictionType { get; set; } // e.g. SuccessRate, Revenue, etc.
        public int? DoctorId { get; set; }
        [StringLength(100)]
        public string TreatmentType { get; set; }
    }
} 