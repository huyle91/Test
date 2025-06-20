using InfertilityTreatment.Entity.Enums;
using System;

namespace InfertilityTreatment.Entity.DTOs.Results
{
    public class TestResultDetailDto : TestResultDto
    {
        public string? ResultInterpretation { get; set; } // Normal/Abnormal/RequiresAttention
    }
}
