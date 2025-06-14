namespace InfertilityTreatment.Entity.DTOs.Doctors
{
    public class DoctorSearchDto
    {
        public string? Query { get; set; }
        public string? Specialization { get; set; }
        public bool? IsAvailable { get; set; }
    }
}
