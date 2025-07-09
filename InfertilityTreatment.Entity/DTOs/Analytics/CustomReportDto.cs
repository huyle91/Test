using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace InfertilityTreatment.Entity.DTOs.Analytics
{
    public class CustomReportDto
    {
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        [Required]
        public List<string> Metrics { get; set; }
        public List<int> DoctorIds { get; set; }
        public List<string> TreatmentTypes { get; set; }
        [Required]
        [StringLength(20)]
        public string ExportFormat { get; set; } // CSV, Excel, PDF
    }
} 