using System;
using System.Collections.Generic;

namespace InfertilityTreatment.Entity.DTOs.Analytics
{
    public class CustomReportDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<string> Metrics { get; set; }
        public List<int> DoctorIds { get; set; }
        public List<string> TreatmentTypes { get; set; }
        public string ExportFormat { get; set; } // CSV, Excel, PDF
    }
} 