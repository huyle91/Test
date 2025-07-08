using InfertilityTreatment.Entity.Constants;

namespace InfertilityTreatment.Entity.DTOs.Prescriptions
{
    public class PrescriptionFilterDto
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = PaginationConstants.DefaultPageSize;
        public string? Search { get; set; }
        public int? CustomerId { get; set; }
        public int? PhaseId { get; set; }
        public int? MedicationId { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? SortBy { get; set; } = "StartDate";
        public string? SortDirection { get; set; } = "desc";
    }
}
