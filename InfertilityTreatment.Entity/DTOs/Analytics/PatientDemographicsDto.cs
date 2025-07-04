using System.Collections.Generic;

namespace InfertilityTreatment.Entity.DTOs.Analytics
{
    public class PatientDemographicsDto
    {
        public int TotalPatients { get; set; }
        public List<TreatmentTypeDto> TreatmentTypes { get; set; } = new List<TreatmentTypeDto>();
        public GenderDistributionDto GenderDistribution { get; set; } = new GenderDistributionDto();
    }



    public class TreatmentTypeDto
    {
        public string TreatmentType { get; set; } = string.Empty;
        public int Count { get; set; }
        public double Percentage { get; set; }
    }

    public class GenderDistributionDto
    {
        public int Male { get; set; }
        public int Female { get; set; }
        public int Other { get; set; }
    }
}
