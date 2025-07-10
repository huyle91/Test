using InfertilityTreatment.Entity.DTOs.Common;
using System;
using System.ComponentModel.DataAnnotations;

namespace InfertilityTreatment.Entity.DTOs.Analytics
{
    public class OutcomeAnalysisDto : PaginationQueryDTO
    {
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        [StringLength(100)]
        public string TreatmentType { get; set; }
        public int? DoctorId { get; set; }
        [StringLength(100)]
        public string GroupBy { get; set; } // Age, TreatmentType, Doctor
    }
} 