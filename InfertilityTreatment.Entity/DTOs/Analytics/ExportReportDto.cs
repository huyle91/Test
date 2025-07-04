using System;

namespace InfertilityTreatment.Entity.DTOs.Analytics
{
    public class ExportReportDto
    {
        public string ReportType { get; set; } = string.Empty; // "dashboard", "revenue", "doctor-performance", "patient-demographics"
        public string ExportFormat { get; set; } = string.Empty; // "pdf", "excel"
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? DoctorId { get; set; }
        public int? ServiceId { get; set; }
        public string? AdditionalFilters { get; set; }
    }

    public class ExportReportResponseDto
    {
        public string FileName { get; set; } = string.Empty;
        public string FileUrl { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public long FileSizeBytes { get; set; }
        public DateTime GeneratedAt { get; set; }
    }
}
