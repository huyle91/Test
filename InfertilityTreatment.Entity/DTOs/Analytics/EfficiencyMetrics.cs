namespace InfertilityTreatment.Entity.DTOs.Analytics
{
    public class EfficiencyMetrics
    {
        public double AverageAppointmentDuration { get; set; }
        public double DoctorUtilizationRate { get; set; }
        public double PatientSatisfactionScore { get; set; }
        public int TotalCyclesCompleted { get; set; }
        public decimal AverageRevenuePerCycle { get; set; }
    }
} 