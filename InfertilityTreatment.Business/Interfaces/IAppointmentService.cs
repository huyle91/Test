using InfertilityTreatment.Entity.DTOs.Appointments;
using InfertilityTreatment.Entity.DTOs.Common;
using InfertilityTreatment.Entity.DTOs.DoctorSchedules;
using InfertilityTreatment.Entity.DTOs.TreatmentPakages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfertilityTreatment.Business.Interfaces
{
    public interface IAppointmentService
    {
        Task<AppointmentResponseDto> CreateAppointmentAsync(CreateAppointmentDto dto);
        Task<PaginatedResultDto<AppointmentResponseDto>> GetAppointmentsByCustomerAsync(int customerId, PaginationQueryDTO pagination);
        Task<PaginatedResultDto<AppointmentResponseDto>> GetAppointmentsByDoctorAsync(int doctorId, DateTime date, PaginationQueryDTO pagination);
        Task<AppointmentResponseDto> RescheduleAppointmentAsync(int appointmentId, int doctorScheduleId, DateTime scheduledDateTime);
        Task<AppointmentResponseDto> CancelAppointmentAsync(int appointmentId);
        Task<PaginatedResultDto<DoctorScheduleDto>> GetDoctorAvailabilityAsync(int doctorId, DateTime date, PaginationQueryDTO pagination);
        Task<AppointmentResponseDto?> GetByIdAsync(int id);
        Task<AvailabilityResponseDto> CheckAvailabilityAsync(AvailabilityQueryDto query);
        Task<BulkCreateResultDto> CreateBulkAppointmentsAsync(BulkCreateAppointmentsDto dto);
        Task<AutoScheduleResultDto> AutoScheduleAppointmentsAsync(AutoScheduleDto dto);
        Task<List<ConflictDto>> GetScheduleConflictsAsync(ConflictCheckDto query);
        Task<bool> SendAppointmentReminderAsync(int appointmentId);
    }

}
