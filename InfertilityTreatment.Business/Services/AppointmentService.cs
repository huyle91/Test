using AutoMapper;
using InfertilityTreatment.Business.Interfaces;
using InfertilityTreatment.Data.Repositories.Implementations;
using InfertilityTreatment.Data.Repositories.Interfaces;
using InfertilityTreatment.Entity.Constants;
using InfertilityTreatment.Entity.DTOs.Appointments;
using InfertilityTreatment.Entity.DTOs.Common;
using InfertilityTreatment.Entity.DTOs.DoctorSchedules;
using InfertilityTreatment.Entity.DTOs.TreatmentPakages;
using InfertilityTreatment.Entity.Entities;
using InfertilityTreatment.Entity.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfertilityTreatment.Business.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AppointmentService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<AppointmentResponseDto> CreateAppointmentAsync(CreateAppointmentDto dto)
        {
            // Check for conflict: same doctor, same date, same slot
            var conflict = await _unitOfWork.Appointments.GetByDoctorAndScheduleAsync(dto.DoctorId, dto.ScheduledDateTime, dto.DoctorScheduleId);
            if (conflict != null)
            {
                throw new InvalidOperationException("Doctor already has an appointment at this time slot.");
            }

            var appointment = new Appointment
            {
                CycleId = dto.CycleId,
                DoctorId = dto.DoctorId,
                DoctorScheduleId = dto.DoctorScheduleId,
                AppointmentType = dto.AppointmentType,
                ScheduledDateTime = dto.ScheduledDateTime,
                Notes = dto.Notes,
                Status = AppointmentStatus.Scheduled
            };

            var created = await _unitOfWork.Appointments.CreateAsync(appointment);
            await _unitOfWork.SaveChangesAsync();

            return new AppointmentResponseDto
            {
                Id = created.Id,
                DoctorId = created.DoctorId,
                DoctorScheduleId = created.DoctorScheduleId,
                AppointmentType = created.AppointmentType,
                ScheduledDateTime = created.ScheduledDateTime,
                Status = created.Status,
                Notes = created.Notes,
                Results = created.Results
            };
        }

        public async Task<PaginatedResultDto<AppointmentResponseDto>> GetAppointmentsByCustomerAsync(int customerId, PaginationQueryDTO pagination)
        {
            var list = await _unitOfWork.Appointments.GetByCustomerAsync(customerId, pagination);
            var dtoList = list.Items.Select(a => new AppointmentResponseDto
            {
                Id = a.Id,
                DoctorId = a.DoctorId,
                DoctorScheduleId = a.DoctorScheduleId,
                AppointmentType = a.AppointmentType,
                ScheduledDateTime = a.ScheduledDateTime,
                Status = a.Status,
                Notes = a.Notes,
                Results = a.Results
            }).ToList();

            return new PaginatedResultDto<AppointmentResponseDto>(
                dtoList,
                list.TotalCount,
                pagination.PageNumber,
                pagination.PageSize
            );
        }

        public async Task<PaginatedResultDto<AppointmentResponseDto>> GetAppointmentsByDoctorAsync(int doctorId, DateTime date, PaginationQueryDTO pagination)
        {
            var list = await _unitOfWork.Appointments.GetByDoctorAndDateAsync(doctorId, date, pagination);
            var dtoList = list.Items.Select(a => new AppointmentResponseDto
            {
                Id = a.Id,
                DoctorId = a.DoctorId,
                DoctorScheduleId = a.DoctorScheduleId,
                AppointmentType = a.AppointmentType,
                ScheduledDateTime = a.ScheduledDateTime,
                Status = a.Status,
                Notes = a.Notes,
                Results = a.Results
            }).ToList();

            return new PaginatedResultDto<AppointmentResponseDto>(
                dtoList,
                list.TotalCount,
                pagination.PageNumber,
                pagination.PageSize
            );
        }

        public async Task<AppointmentResponseDto> RescheduleAppointmentAsync(int id, int doctorScheduleId, DateTime scheduledDateTime)
        {
            var appointment = await _unitOfWork.Appointments.GetByIdAsync(id);
            if (appointment == null) return null;

            // Check for conflict: same doctor, same date, same slot (excluding current appointment)
            var conflict = await _unitOfWork.Appointments.GetByDoctorAndScheduleAsync(appointment.DoctorId, scheduledDateTime, doctorScheduleId);
            if (conflict != null && conflict.Id != id)
            {
                throw new InvalidOperationException("Doctor already has an appointment at this time slot.");
            }

            appointment.DoctorScheduleId = doctorScheduleId;
            appointment.ScheduledDateTime = scheduledDateTime;
            appointment.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.Appointments.UpdateAsync(appointment);
            await _unitOfWork.SaveChangesAsync();

            return new AppointmentResponseDto
            {
                Id = appointment.Id,
                DoctorId = appointment.DoctorId,
                DoctorScheduleId = appointment.DoctorScheduleId,
                AppointmentType = appointment.AppointmentType,
                ScheduledDateTime = appointment.ScheduledDateTime,
                Status = appointment.Status,
                Notes = appointment.Notes,
                Results = appointment.Results
            };
        }

        public async Task<AppointmentResponseDto> CancelAppointmentAsync(int id)
        {
            var appointment = await _unitOfWork.Appointments.GetByIdAsync(id);
            if (appointment == null) throw new Exception("Appointment not found");

            appointment.Status = AppointmentStatus.Cancelled;
            await _unitOfWork.Appointments.UpdateAsync(appointment);
            await _unitOfWork.SaveChangesAsync();

            return new AppointmentResponseDto
            {
                Id = appointment.Id,
                DoctorId = appointment.DoctorId,
                DoctorScheduleId = appointment.DoctorScheduleId,
                AppointmentType = appointment.AppointmentType,
                ScheduledDateTime = appointment.ScheduledDateTime,
                Status = appointment.Status,
                Notes = appointment.Notes,
                Results = appointment.Results
            };
        }

        public async Task<PaginatedResultDto<DoctorScheduleDto>> GetDoctorAvailabilityAsync(int doctorId, DateTime date, PaginationQueryDTO pagination)
        {
            var entityResult = await _unitOfWork.Appointments.GetDoctorAvailabilityAsync(doctorId, date, pagination);
            return new PaginatedResultDto<DoctorScheduleDto>(
                _mapper.Map<List<DoctorScheduleDto>>(entityResult.Items),
                entityResult.TotalCount,
                entityResult.PageNumber,
                entityResult.PageSize
            );
        }

        public async Task<AppointmentResponseDto?> GetByIdAsync(int id)
        {
            var appointment = await _unitOfWork.Appointments.GetByIdAsync(id);
            if (appointment == null) return null;
            return new AppointmentResponseDto
            {
                Id = appointment.Id,
                DoctorId = appointment.DoctorId,
                DoctorScheduleId = appointment.DoctorScheduleId,
                AppointmentType = appointment.AppointmentType,
                ScheduledDateTime = appointment.ScheduledDateTime,
                Status = appointment.Status,
                Notes = appointment.Notes,
                Results = appointment.Results
            };
        }
    }

}
