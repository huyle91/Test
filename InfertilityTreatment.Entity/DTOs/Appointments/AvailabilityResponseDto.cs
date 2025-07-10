namespace InfertilityTreatment.Entity.DTOs.Appointments
{
    public class AvailabilitySlotDto
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int Duration { get; set; } // minutes
        public bool IsAvailable { get; set; }
        public string? Reason { get; set; } // Reason if not available
    }

    public class AvailabilityResponseDto
    {
        public int DoctorId { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public DateTime QueryDate { get; set; }
        public List<AvailabilitySlotDto> AvailableSlots { get; set; } = new();
        public int TotalSlots { get; set; }
        public int AvailableCount { get; set; }
    }
}
