using InfertilityTreatment.Entity.DTOs.TreatmentPakages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfertilityTreatment.Entity.DTOs.TreatmentServices
{
    public class TreatmentServiceDetailDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal BasePrice { get; set; }
        public int EstimatedDuration { get; set; }
        public List<TreatmentPackageDto> Packages { get; set; }
    }
}
