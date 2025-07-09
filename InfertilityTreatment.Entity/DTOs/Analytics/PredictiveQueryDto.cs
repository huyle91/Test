using System;

namespace InfertilityTreatment.Entity.DTOs.Analytics
{
    public class PredictiveQueryDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string PredictionType { get; set; } // e.g. SuccessRate, Revenue, etc.
        public int? DoctorId { get; set; }
        public string TreatmentType { get; set; }
    }
} 