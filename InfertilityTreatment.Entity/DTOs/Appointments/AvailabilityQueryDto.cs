using System.ComponentModel.DataAnnotations;

namespace InfertilityTreatment.Entity.DTOs.Appointments
{
    public class AvailabilityQueryDto
    {
        [Required]
        public int DoctorId { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        [Range(15, 240, ErrorMessage = "Duration must be between 15 and 240 minutes")]
        public int Duration { get; set; } // minutes

        [Required]
        [StringLength(50)]
        public string AppointmentType { get; set; } = string.Empty;

        public bool IncludeBufferTime { get; set; } = true;
    }
}
