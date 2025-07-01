using System;

namespace InfertilityTreatment.Entity.DTOs.Bookings
{
    public class BookTreatmentDto
    {
        public int CustomerId { get; set; }
        public int TreatmentPackageId { get; set; }
        public int DoctorId { get; set; }
        public DateTime PreferredStartDate { get; set; }
        public string Notes { get; set; }
        public string ContactPhone { get; set; }
        public string EmergencyContact { get; set; }
    }
}
