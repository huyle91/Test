using InfertilityTreatment.Business.Interfaces;
using InfertilityTreatment.Data.Repositories.Interfaces;
using InfertilityTreatment.Entity.DTOs.Appointments;
using InfertilityTreatment.Entity.DTOs.Bookings;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InfertilityTreatment.Business.Services
{
    public class BookingService : IBookingService
    {
        private readonly IUnitOfWork _unitOfWork;

        public BookingService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // Stub implementations for Week 6
        public async Task<BookingResponseDto> BookTreatmentCycleAsync(BookTreatmentDto dto)
        {
            // TODO: Implement in BE023
            throw new NotImplementedException("Will be implemented in BE023");
        }

        public async Task<List<TimeSlotDto>> GetAvailableSlotsAsync(AvailabilityFilterDto filter)
        {
            // TODO: Implement in BE023
            throw new NotImplementedException("Will be implemented in BE023");
        }

        public async Task<AppointmentDto> BookDoctorAppointmentAsync(BookAppointmentDto dto)
        {
            // TODO: Implement in BE023
            throw new NotImplementedException("Will be implemented in BE023");
        }

        public async Task<bool> RescheduleBookingAsync(int bookingId, RescheduleDto dto)
        {
            // TODO: Implement in BE023
            throw new NotImplementedException("Will be implemented in BE023");
        }

        public async Task<bool> CancelBookingAsync(int bookingId, CancellationDto dto)
        {
            // TODO: Implement in BE023
            throw new NotImplementedException("Will be implemented in BE023");
        }
    }
}
