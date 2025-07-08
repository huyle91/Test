namespace InfertilityTreatment.Entity.DTOs.TreatmentCycles
{
    public class TimelineEventDto
    {
        public int Id { get; set; }
        
        public string EventType { get; set; } = string.Empty;
        
        public string Title { get; set; } = string.Empty;
        
        public string? Description { get; set; }
        
        public DateTime EventDate { get; set; }
        
        public bool IsCompleted { get; set; }
        
        public string Status { get; set; } = string.Empty;
        
        public int? PhaseId { get; set; }
    }
}
