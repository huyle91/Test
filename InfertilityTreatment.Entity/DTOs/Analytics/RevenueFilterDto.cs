using System;

namespace InfertilityTreatment.Entity.DTOs.Analytics
{
    public class RevenueFilterDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int? DoctorId { get; set; }
        public int? ServiceId { get; set; }
        public string GroupBy { get; set; } // "month", "quarter", "year"
    }
}
