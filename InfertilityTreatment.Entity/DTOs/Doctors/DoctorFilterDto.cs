namespace InfertilityTreatment.Entity.DTOs.Doctors
{
    public class DoctorFilterDto
    {
        public string? Specialization { get; set; }
        public bool? IsAvailable { get; set; }
        public double? MinRating { get; set; }
        public int? MinYearsOfExperience { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
