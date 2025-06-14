namespace InfertilityTreatment.Entity.DTOs.Doctors
{
    public class CreateDoctorDto
    {
        public int UserId { get; set; }
        public string LicenseNumber { get; set; } = string.Empty;
        public string? Specialization { get; set; }
        public int YearsOfExperience { get; set; }
        public string? Education { get; set; }
        public string? Biography { get; set; }
        public decimal? ConsultationFee { get; set; }
        public bool IsAvailable { get; set; } = true;
        public decimal? SuccessRate { get; set; }
    }
}
