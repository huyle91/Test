using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfertilityTreatment.Entity.DTOs.Medications
{
    public class MedicationFilterDto
    {
        public string? Name { get; set; }
        public string? ActiveIngredient { get; set; }
        public string? Manufacturer { get; set; }
        public bool? IsActive { get; set; }
        public string? SortBy { get; set; } = "Name";
        public string? SortDirection { get; set; } = "asc";
    }
}
