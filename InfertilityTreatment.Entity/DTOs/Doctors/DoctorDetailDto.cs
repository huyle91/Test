namespace InfertilityTreatment.Entity.DTOs.Doctors
{
    public class DoctorDetailDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string LicenseNumber { get; set; } = string.Empty;
        public string? Specialization { get; set; }
        public int YearsOfExperience { get; set; }
        public string? Education { get; set; }
        public string? Biography { get; set; }
        public decimal? ConsultationFee { get; set; }
        public bool IsAvailable { get; set; }
        public decimal? SuccessRate { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
    }
}
