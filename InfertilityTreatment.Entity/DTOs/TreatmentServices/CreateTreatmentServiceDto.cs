using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfertilityTreatment.Entity.DTOs.TreatmentServices
{
    public class CreateTreatmentServiceDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal? BasePrice { get; set; }
        public int? EstimatedDuration { get; set; }
        public string? Requirements { get; set; }
    }
}
