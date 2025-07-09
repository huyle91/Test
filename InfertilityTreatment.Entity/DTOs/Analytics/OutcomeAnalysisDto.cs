using System;

namespace InfertilityTreatment.Entity.DTOs.Analytics
{
    public class OutcomeAnalysisDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string TreatmentType { get; set; }
        public int? DoctorId { get; set; }
        public string GroupBy { get; set; } // Age, TreatmentType, Doctor
    }
} 