using System;
using System.Collections.Generic;

namespace InfertilityTreatment.Entity.DTOs.Analytics
{
    public class DoctorPerformanceDto
    {
        public int DoctorId { get; set; }
        public string DoctorName { get; set; }
        public string Specialization { get; set; }
        public int TotalPatients { get; set; }
        public int TotalAppointments { get; set; }
        public int CompletedTreatments { get; set; }
        public double SuccessRate { get; set; }
        public double AverageRating { get; set; }
        public int TotalReviews { get; set; }
        public decimal TotalRevenue { get; set; }
        public List<MonthlyPerformanceDto> MonthlyPerformance { get; set; }
        public DateTime LastUpdated { get; set; }
    }

    public class MonthlyPerformanceDto
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public int Appointments { get; set; }
        public decimal Revenue { get; set; }
        public double Rating { get; set; }
    }
}
