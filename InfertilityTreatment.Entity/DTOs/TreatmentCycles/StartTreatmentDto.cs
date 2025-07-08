using System.ComponentModel.DataAnnotations;

namespace InfertilityTreatment.Entity.DTOs.TreatmentCycles
{
    public class StartTreatmentDto
    {
        public DateTime? ActualStartDate { get; set; }
        
        [StringLength(1000)]
        public string? DoctorNotes { get; set; }
        
        public List<int> RequiredTestIds { get; set; } = new List<int>();
    }
}
