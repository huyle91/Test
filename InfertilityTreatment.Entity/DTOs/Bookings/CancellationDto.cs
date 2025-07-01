using System;

namespace InfertilityTreatment.Entity.DTOs.Bookings
{
    public class CancellationDto
    {
        public string Reason { get; set; }
        public bool CustomerInitiated { get; set; } = true;
        public DateTime CancellationDate { get; set; } = DateTime.Now;
        public bool RequestRefund { get; set; } = false;
        public string AdditionalNotes { get; set; }
    }
}
