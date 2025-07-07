using InfertilityTreatment.Entity.DTOs.Appointments;
using InfertilityTreatment.Entity.DTOs.Bookings;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InfertilityTreatment.Business.Interfaces
{
    public interface IBookingService
    {
        // Prepare for BE023: Enhanced Booking System
        Task<BookingResponseDto> BookTreatmentCycleAsync(BookTreatmentDto dto);
        Task<List<TimeSlotDto>> GetAvailableSlotsAsync(AvailabilityFilterDto filter);
        Task<AppointmentDto> BookDoctorAppointmentAsync(BookAppointmentDto dto);
        Task<bool> RescheduleBookingAsync(int bookingId, RescheduleDto dto);
        Task<bool> CancelBookingAsync(int bookingId, CancellationDto dto);
    }
}
