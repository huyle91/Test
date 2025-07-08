using System.ComponentModel.DataAnnotations;

namespace InfertilityTreatment.Entity.DTOs.Medications
{
    public class UpdateMedicationDto
    {
        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(200, MinimumLength = 1)]
        public string ActiveIngredient { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string Manufacturer { get; set; } = string.Empty;

        [Required]
        [StringLength(1000, MinimumLength = 1)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [StringLength(500, MinimumLength = 1)]
        public string StorageInstructions { get; set; } = string.Empty;

        [Required]
        [StringLength(1000, MinimumLength = 1)]
        public string SideEffects { get; set; } = string.Empty;
    }
}
