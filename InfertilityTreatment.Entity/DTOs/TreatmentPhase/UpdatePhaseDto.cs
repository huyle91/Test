﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InfertilityTreatment.Entity.Enums;

namespace InfertilityTreatment.Entity.DTOs.TreatmentPhase
{
    public class UpdatePhaseDto
    {
        public string PhaseName { get; set; }
        public int PhaseOrder { get; set; } 
        public PhaseStatus Status { get; set; }
        public decimal Cost { get; set; } 
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Instructions { get; set; }
        public string? Notes { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
