namespace InfertilityTreatment.Entity.DTOs.Doctors
{
    public class DoctorResponseDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? Specialization { get; set; }
        public bool IsAvailable { get; set; }
        public decimal? SuccessRate { get; set; }
        public int YearsOfExperience { get; set; }
        public string LicenseNumber { get; set; } = string.Empty;
    }
}
