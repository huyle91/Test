using System;
using System.Collections.Generic;

namespace InfertilityTreatment.Entity.DTOs.Analytics
{
    public class PatientJourneyResultDto
    {
        public int PatientId { get; set; }
        public string PatientName { get; set; }
        public List<JourneyStepDto> Steps { get; set; }
    }

    public class JourneyStepDto
    {
        public string StepName { get; set; }
        public DateTime Timestamp { get; set; }
        public string Status { get; set; }
    }
} 