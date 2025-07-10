namespace InfertilityTreatment.Entity.DTOs.Appointments
{
    public class BulkCreateResultDto
    {
        public int TotalRequested { get; set; }
        public int SuccessfullyCreated { get; set; }
        public int Failed { get; set; }
        public List<AppointmentResponseDto> CreatedAppointments { get; set; } = new();
        public List<string> Errors { get; set; } = new();
        public List<ConflictDto> Conflicts { get; set; } = new();
    }

    public class ConflictDto
    {
        public int AppointmentIndex { get; set; }
        public string ConflictType { get; set; } = string.Empty;
        public string ConflictReason { get; set; } = string.Empty;
        public DateTime ConflictTime { get; set; }
        public AppointmentResponseDto? ExistingAppointment { get; set; }
    }
}
