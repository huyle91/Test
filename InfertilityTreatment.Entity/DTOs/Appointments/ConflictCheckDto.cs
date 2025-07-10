using System.ComponentModel.DataAnnotations;

namespace InfertilityTreatment.Entity.DTOs.Appointments
{
    public class ConflictCheckDto
    {
        [Required]
        public int DoctorId { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public bool IncludeOverlapping { get; set; } = true;

        public bool IncludeBackToBack { get; set; } = true;
    }
}
