namespace InfertilityTreatment.Entity.DTOs.TreatmentCycles
{
    public class CycleTimelineDto
    {
        public int CycleId { get; set; }
        
        public string CycleName { get; set; } = string.Empty;
        
        public string OverallStatus { get; set; } = string.Empty;
        
        public DateTime CreatedDate { get; set; }
        
        public DateTime? StartDate { get; set; }
        
        public DateTime? EstimatedCompletionDate { get; set; }
        
        public List<TimelineEventDto> TimelineEvents { get; set; } = new List<TimelineEventDto>();
        
        public int CompletionPercentage { get; set; }
    }
}
