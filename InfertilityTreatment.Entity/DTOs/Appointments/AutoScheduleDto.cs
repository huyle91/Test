using System.ComponentModel.DataAnnotations;

namespace InfertilityTreatment.Entity.DTOs.Appointments
{
    public class AutoScheduleDto
    {
        [Required]
        public int CycleId { get; set; }

        [Required]
        public int DoctorId { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "At least one appointment type is required")]
        public List<string> AppointmentTypes { get; set; } = new();

        [Required]
        public DateTime PreferredStartDate { get; set; }

        [Required]
        public TimeSpan PreferredTimeStart { get; set; }

        [Required]
        public TimeSpan PreferredTimeEnd { get; set; }

        public int DefaultDuration { get; set; } = 30; // minutes

        public int DaysBetweenAppointments { get; set; } = 7;

        public bool SendNotifications { get; set; } = true;
    }
}
