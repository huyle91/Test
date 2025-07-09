using System;

namespace InfertilityTreatment.Entity.DTOs.Analytics
{
    public class EfficiencyQueryDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int? DoctorId { get; set; }
        public string TreatmentType { get; set; }
    }
} 