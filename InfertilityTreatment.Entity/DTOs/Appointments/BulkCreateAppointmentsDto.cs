using System.ComponentModel.DataAnnotations;

namespace InfertilityTreatment.Entity.DTOs.Appointments
{
    public class BulkCreateAppointmentsDto
    {
        [Required]
        [MinLength(1, ErrorMessage = "At least one appointment is required")]
        [MaxLength(50, ErrorMessage = "Maximum 50 appointments per bulk operation")]
        public List<CreateAppointmentDto> Appointments { get; set; } = new();

        public bool CheckConflicts { get; set; } = true;

        public bool SendNotifications { get; set; } = true;

        public bool ContinueOnError { get; set; } = false;
    }
}
