using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfertilityTreatment.Entity.DTOs.TreatmentPhase
{
    public class UpdatePhaseDto
    {
        public string PhaseName { get; set; }
        public string PhaseOrder { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal Cost { get; set; }
        public DateTime StartDate { get; set; }
        public string? Instructions { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; } = true;
        public string? Note
        {
            get; set;
        }
    }
}
