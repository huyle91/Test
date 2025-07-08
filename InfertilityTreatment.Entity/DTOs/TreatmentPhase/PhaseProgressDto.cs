using InfertilityTreatment.Entity.Enums;
using System;
using System.Collections.Generic;

namespace InfertilityTreatment.Entity.DTOs.TreatmentPhase
{
    public class PhaseProgressDto
    {
        public int PhaseId { get; set; }
        public string PhaseName { get; set; } = string.Empty;
        public PhaseStatus Status { get; set; }
        public int PhaseOrder { get; set; }
        
        /// <summary>
        /// Progress percentage (0-100)
        /// </summary>
        public double ProgressPercentage { get; set; }
        
        /// <summary>
        /// Current milestone or step within the phase
        /// </summary>
        public string CurrentMilestone { get; set; } = string.Empty;
        
        /// <summary>
        /// List of completed milestones/checkpoints
        /// </summary>
        public List<string> CompletedMilestones { get; set; } = new List<string>();
        
        /// <summary>
        /// List of pending milestones/checkpoints
        /// </summary>
        public List<string> PendingMilestones { get; set; } = new List<string>();
        
        public DateTime? StartDate { get; set; }
        public DateTime? EstimatedEndDate { get; set; }
        public DateTime? ActualEndDate { get; set; }
        
        /// <summary>
        /// Number of appointments scheduled for this phase
        /// </summary>
        public int TotalAppointments { get; set; }
        
        /// <summary>
        /// Number of completed appointments
        /// </summary>
        public int CompletedAppointments { get; set; }
        
        /// <summary>
        /// Number of test results associated with this phase
        /// </summary>
        public int TotalTestResults { get; set; }
        
        /// <summary>
        /// Number of completed test results
        /// </summary>
        public int CompletedTestResults { get; set; }
        
        public string? Instructions { get; set; }
        public string? Notes { get; set; }
        
        public DateTime LastUpdated { get; set; }
    }
}
