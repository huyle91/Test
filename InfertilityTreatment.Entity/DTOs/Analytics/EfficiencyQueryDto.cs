using System;
using System.ComponentModel.DataAnnotations;

namespace InfertilityTreatment.Entity.DTOs.Analytics
{
    public class EfficiencyQueryDto
    {
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        public int? DoctorId { get; set; }
        [StringLength(100)]
        public string TreatmentType { get; set; }
    }
} 