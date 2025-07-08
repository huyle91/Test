using System.ComponentModel.DataAnnotations;

namespace InfertilityTreatment.Entity.DTOs.Prescriptions
{
    public class UpdatePrescriptionDto
    {
        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string Dosage { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string Frequency { get; set; } = string.Empty;

        [Required]
        [Range(1, 365, ErrorMessage = "Duration must be between 1 and 365 days")]
        public int Duration { get; set; }

        [Required]
        [StringLength(500, MinimumLength = 1)]
        public string Instructions { get; set; } = string.Empty;

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }
    }
}
