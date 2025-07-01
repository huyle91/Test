using System;

namespace InfertilityTreatment.Entity.DTOs.Bookings
{
    public class TimeSlotDto
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool IsAvailable { get; set; }
        public int? DoctorId { get; set; }
        public string DoctorName { get; set; }
        public string SlotType { get; set; } // "consultation", "treatment", "followup"
        public decimal? Price { get; set; }
        public int MaxBookings { get; set; } = 1;
        public int CurrentBookings { get; set; } = 0;
    }
}
