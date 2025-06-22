using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfertilityTreatment.Entity.DTOs.Prescription
{
    public class PrescriptionDetailDto
    {
        public Guid Id { get; set; }
        public Guid MedicationId { get; set; }
        public string MedicationName { get; set; }
        public Guid AppointmentId { get; set; }
        public string AppointmentNote { get; set; }
        public int Quantity { get; set; }
        public string Instructions { get; set; }
    }
}
