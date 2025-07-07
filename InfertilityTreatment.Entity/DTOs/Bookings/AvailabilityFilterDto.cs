using System;

namespace InfertilityTreatment.Entity.DTOs.Bookings
{
    public class AvailabilityFilterDto
    {
        public int? DoctorId { get; set; }
        public int? ServiceId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public TimeSpan? PreferredTime { get; set; }
        public int? Duration { get; set; } // in minutes
    }
}
