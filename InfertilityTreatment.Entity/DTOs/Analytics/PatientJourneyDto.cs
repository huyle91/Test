using System;

namespace InfertilityTreatment.Entity.DTOs.Analytics
{
    public class PatientJourneyDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int? PatientId { get; set; }
        public int? DoctorId { get; set; }
    }
} 