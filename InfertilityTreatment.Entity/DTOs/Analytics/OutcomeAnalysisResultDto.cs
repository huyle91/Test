using System;
using System.Collections.Generic;

namespace InfertilityTreatment.Entity.DTOs.Analytics
{
    public class OutcomeAnalysisResultDto
    {
        public double SuccessRate { get; set; }
        public Dictionary<string, double> SuccessRateByGroup { get; set; } 
        public List<TimeSpan> TimeToSuccessList { get; set; }
        public List<DoctorComparisonDto> DoctorComparisons { get; set; }
        public List<TrendPointDto> TrendData { get; set; }
    }

    public class DoctorComparisonDto
    {
        public int DoctorId { get; set; }
        public string DoctorName { get; set; }
        public double SuccessRate { get; set; }
    }

    public class TrendPointDto
    {
        public DateTime Date { get; set; }
        public double Value { get; set; }
    }
} 