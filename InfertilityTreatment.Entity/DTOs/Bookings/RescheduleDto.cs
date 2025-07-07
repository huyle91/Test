using System;

namespace InfertilityTreatment.Entity.DTOs.Bookings
{
    public class RescheduleDto
    {
        public DateTime NewDate { get; set; }
        public TimeSpan NewTime { get; set; }
        public string Reason { get; set; }
        public bool CustomerInitiated { get; set; } = true;
    }
}
