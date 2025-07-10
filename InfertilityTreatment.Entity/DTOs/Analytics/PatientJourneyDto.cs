using System;
using System.ComponentModel.DataAnnotations;

namespace InfertilityTreatment.Entity.DTOs.Analytics
{
    public class PatientJourneyDto
    {
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        public int? PatientId { get; set; }
        public int? DoctorId { get; set; }
    }
} 