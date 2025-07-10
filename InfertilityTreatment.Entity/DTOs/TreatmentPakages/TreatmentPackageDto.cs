using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfertilityTreatment.Entity.DTOs.TreatmentPakages
{
    public class TreatmentPackageDto
    {
        public int Id { get; set; }
        public int ServiceId { get; set; }
        public string PackageName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? IncludedServices { get; set; }
        public int? DurationWeeks { get; set; }

        public bool IsActive { get; set; }
    }
}
