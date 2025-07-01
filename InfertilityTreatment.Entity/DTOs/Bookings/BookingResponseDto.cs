using System;

namespace InfertilityTreatment.Entity.DTOs.Bookings
{
    public class BookingResponseDto
    {
        public int BookingId { get; set; }
        public string BookingReference { get; set; }
        public string Status { get; set; }
        public DateTime BookingDate { get; set; }
        public DateTime? ScheduledDate { get; set; }
        public string Message { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentStatus { get; set; }
        public bool RequiresPayment { get; set; }
        public string PaymentUrl { get; set; }
    }
}
