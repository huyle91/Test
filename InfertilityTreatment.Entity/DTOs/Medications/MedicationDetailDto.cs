using InfertilityTreatment.Entity.DTOs.Prescription;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfertilityTreatment.Entity.DTOs.Medications
{
    public class MedicationDetailDto
    {
        public int Id { get; set; } 
        public string Name { get; set; }
        public string ActiveIngredient { get; set; } 
        public string Manufacturer { get; set; }
        public string Description { get; set; } 
        public List<PrescriptionDetailDto> Prescriptions { get; set; } = new List<PrescriptionDetailDto>();
    }

}
