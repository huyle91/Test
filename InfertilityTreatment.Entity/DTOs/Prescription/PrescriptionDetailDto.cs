using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfertilityTreatment.Entity.DTOs.Prescription
{
    public class PrescriptionDetailDto
    {
        public int Id { get; set; } 
        public int MedicationId { get; set; } 
        public string MedicationName { get; set; }
        public int PhaseId { get; set; } 
        public string Dosage { get; set; }
        public string Frequency { get; set; }
        public string Instructions { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
