using System;
using System.Collections.Generic;

namespace InfertilityTreatment.Entity.DTOs.Analytics
{
    public class DashboardStatsDto
    {
        public int TotalPatients { get; set; }
        public int TotalAppointments { get; set; }
        public int TotalTreatmentCycles { get; set; }
        public decimal TotalRevenue { get; set; }
        public int ActiveTreatments { get; set; }
        public int CompletedTreatments { get; set; }
        public double SuccessRate { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
