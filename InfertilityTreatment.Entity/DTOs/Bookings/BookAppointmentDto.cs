using System;

namespace InfertilityTreatment.Entity.DTOs.Bookings
{
    public class BookAppointmentDto
    {
        public int CustomerId { get; set; }
        public int DoctorId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public TimeSpan AppointmentTime { get; set; }
        public string AppointmentType { get; set; } // "consultation", "followup", "treatment"
        public string Notes { get; set; }
        public string Symptoms { get; set; }
        public bool IsUrgent { get; set; } = false;
    }
}
