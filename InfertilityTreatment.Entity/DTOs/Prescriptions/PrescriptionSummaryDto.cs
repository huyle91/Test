namespace InfertilityTreatment.Entity.DTOs.Prescriptions
{
    public class PrescriptionSummaryDto
    {
        public int Id { get; set; }
        public int PhaseId { get; set; }
        public string PhaseName { get; set; } = string.Empty;
        public int MedicationId { get; set; }
        public string MedicationName { get; set; } = string.Empty;
        public string Dosage { get; set; } = string.Empty;
        public string Frequency { get; set; } = string.Empty;
        public int Duration { get; set; }
        public string Instructions { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
