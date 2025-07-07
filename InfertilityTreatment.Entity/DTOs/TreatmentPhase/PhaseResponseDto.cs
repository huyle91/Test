using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InfertilityTreatment.Entity.Enums;

namespace InfertilityTreatment.Entity.DTOs.TreatmentPhase
{
    public class PhaseResponseDto
    {
        public int Id { get; set; }
        public int CycleId { get; set; }
        public string PhaseName { get; set; } = string.Empty;
        public int PhaseOrder { get; set; }

        public PhaseStatus Status { get; set; } = PhaseStatus.Pending;

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public string? Instructions { get; set; }
        public string? Notes { get; set; }
    }
}
