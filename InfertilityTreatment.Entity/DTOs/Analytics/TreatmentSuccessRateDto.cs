using System;

namespace InfertilityTreatment.Entity.DTOs.Analytics
{
    public class TreatmentSuccessRateDto
    {
        public string TreatmentType { get; set; }
        public int TotalCycles { get; set; }
        public int SuccessfulCycles { get; set; }
        public double SuccessRate { get; set; }
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
    }
}
