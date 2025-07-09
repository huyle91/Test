using System;
using System.Collections.Generic;

namespace InfertilityTreatment.Entity.DTOs.Analytics
{
    public class PredictiveAnalyticsResultDto
    {
        public string PredictionType { get; set; }
        public Dictionary<string, double> Predictions { get; set; }
        public string Notes { get; set; }
    }
} 