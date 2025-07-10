namespace InfertilityTreatment.Entity.DTOs.Appointments
{
    public class AutoScheduleResultDto
    {
        public int CycleId { get; set; }
        public int DoctorId { get; set; }
        public int TotalPlanned { get; set; }
        public int SuccessfullyScheduled { get; set; }
        public int Failed { get; set; }
        public List<AppointmentResponseDto> ScheduledAppointments { get; set; } = new();
        public List<string> Errors { get; set; } = new();
        public List<string> Warnings { get; set; } = new();
        public DateTime NextAvailableDate { get; set; }
    }
}
