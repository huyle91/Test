using System;

namespace InfertilityTreatment.Entity.DTOs.Analytics
{
    public class PerformanceFilterDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? MetricType { get; set; } // "appointments", "success_rate", "revenue", "all"
        public bool IncludeMonthlyBreakdown { get; set; } = true;
        public bool IncludeCompareToPrevious { get; set; } = true;
    }
}
