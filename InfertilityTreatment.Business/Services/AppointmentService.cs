using AutoMapper;
using InfertilityTreatment.Business.Interfaces;
using InfertilityTreatment.Data.Repositories.Implementations;
using InfertilityTreatment.Data.Repositories.Interfaces;
using InfertilityTreatment.Entity.Constants;
using InfertilityTreatment.Entity.DTOs.Appointments;
using InfertilityTreatment.Entity.DTOs.Common;
using InfertilityTreatment.Entity.DTOs.DoctorSchedules;
using InfertilityTreatment.Entity.DTOs.Notifications;
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
        private readonly INotificationService _notificationService;
        private readonly IRealTimeNotificationService _realTimeNotificationService;

        public AppointmentService(IUnitOfWork unitOfWork, IMapper mapper, INotificationService notificationService, IRealTimeNotificationService realTimeNotificationService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _notificationService = notificationService;
            _realTimeNotificationService = realTimeNotificationService;
        }

        public async Task<AppointmentResponseDto> CreateAppointmentAsync(CreateAppointmentDto dto)
        {

                // Check existence of related entities
                var cycle = await _unitOfWork.TreatmentCycles.GetCycleByIdAsync(dto.CycleId);
                if (cycle == null)
                    throw new ArgumentException("Cycle not found");
                var doctor = await _unitOfWork.Doctors.GetDoctorByIdAsync(dto.DoctorId);
                if (doctor == null)
                    throw new ArgumentException("Doctor not found");
                var doctorSchedule = await _unitOfWork.DoctorSchedules.GetByIdAsync(dto.DoctorScheduleId);
                if (doctorSchedule == null)
                    throw new ArgumentException("DoctorSchedule not found");

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
                await _unitOfWork.CommitTransactionAsync();

                var result = new AppointmentResponseDto
                {
                    Id = created.Id,
                    CycleId = created.CycleId,
                    DoctorId = created.DoctorId,
                    DoctorScheduleId = created.DoctorScheduleId,
                    AppointmentType = created.AppointmentType,
                    ScheduledDateTime = created.ScheduledDateTime,
                    Status = created.Status,
                    Notes = created.Notes,
                    Results = created.Results
                };

                // Send notification for appointment creation
                await SendAppointmentCreatedNotificationAsync(result);

                return result;
            
            
        }

        public async Task<PaginatedResultDto<AppointmentResponseDto>> GetAppointmentsByCustomerAsync(int customerId, PaginationQueryDTO pagination)
        {
            var cus = await _unitOfWork.Users.GetByIdWithProfilesAsync(customerId);
            if (cus?.Customer == null)
            {
                return new PaginatedResultDto<AppointmentResponseDto>(
                [],
                0,
                pagination.PageNumber,
                pagination.PageSize
            );
            }
            var list = await _unitOfWork.Appointments.GetByCustomerAsync(cus.Customer.Id, pagination);
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
            var doc = await _unitOfWork.Users.GetByIdWithProfilesAsync(doctorId);
            if (doc?.Doctor == null)
            {
                return new PaginatedResultDto<AppointmentResponseDto>(
                [],
                0,
                pagination.PageNumber,
                pagination.PageSize
            );
            }
            var list = await _unitOfWork.Appointments.GetByDoctorAndDateAsync(doc.Doctor.Id, date, pagination);
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
            if (id <= 0)
                throw new ArgumentException("AppointmentId is required and must be greater than 0");
            if (doctorScheduleId <= 0)
                throw new ArgumentException("DoctorScheduleId is required and must be greater than 0");
            if (scheduledDateTime < DateTime.UtcNow)
                throw new ArgumentException("ScheduledDateTime must be in the future");

            var appointment = await _unitOfWork.Appointments.GetByIdAsync(id);
            if (appointment == null)
                throw new InvalidOperationException("Appointment not found");

            var doctorSchedule = await _unitOfWork.DoctorSchedules.GetByIdAsync(doctorScheduleId);
            if (doctorSchedule == null)
                throw new ArgumentException("DoctorSchedule not found");

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

            var result = new AppointmentResponseDto
            {
                Id = appointment.Id,
                CycleId = appointment.CycleId,
                DoctorId = appointment.DoctorId,
                DoctorScheduleId = appointment.DoctorScheduleId,
                AppointmentType = appointment.AppointmentType,
                ScheduledDateTime = appointment.ScheduledDateTime,
                Status = appointment.Status,
                Notes = appointment.Notes,
                Results = appointment.Results
            };

            // Send notification for appointment reschedule
            await SendAppointmentRescheduledNotificationAsync(result);

            return result;
        }

        public async Task<AppointmentResponseDto> CancelAppointmentAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("AppointmentId is required and must be greater than 0");
            
                var appointment = await _unitOfWork.Appointments.GetByIdAsync(id);
                if (appointment == null) throw new InvalidOperationException("Appointment not found");

                appointment.Status = AppointmentStatus.Cancelled;
                await _unitOfWork.Appointments.UpdateAsync(appointment);
                await _unitOfWork.SaveChangesAsync();

                var result = new AppointmentResponseDto
                {
                    Id = appointment.Id,
                    CycleId = appointment.CycleId,
                    DoctorId = appointment.DoctorId,
                    DoctorScheduleId = appointment.DoctorScheduleId,
                    AppointmentType = appointment.AppointmentType,
                    ScheduledDateTime = appointment.ScheduledDateTime,
                    Status = appointment.Status,
                    Notes = appointment.Notes,
                    Results = appointment.Results
                };

                // Send notification for appointment cancellation
                await SendAppointmentCancelledNotificationAsync(result);

                return result;
            
            
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
                CycleId = appointment.CycleId,
                DoctorId = appointment.DoctorId,
                DoctorScheduleId = appointment.DoctorScheduleId,
                AppointmentType = appointment.AppointmentType,
                ScheduledDateTime = appointment.ScheduledDateTime,
                Status = appointment.Status,
                Notes = appointment.Notes,
                Results = appointment.Results
            };
        }

        // Enhanced Appointment Features
        public async Task<AvailabilityResponseDto> CheckAvailabilityAsync(AvailabilityQueryDto query)
        {
            var doctor = await _unitOfWork.Doctors.GetDoctorByIdAsync(query.DoctorId);
            if (doctor == null)
                throw new ArgumentException("Doctor not found");

            var response = new AvailabilityResponseDto
            {
                DoctorId = query.DoctorId,
                DoctorName = doctor.User?.FullName ?? "Unknown",
                QueryDate = query.StartDate.Date
            };

            var availableSlots = new List<AvailabilitySlotDto>();
            var currentTime = query.StartDate;

            while (currentTime <= query.EndDate)
            {
                // Check doctor schedule
                var doctorSchedules = await _unitOfWork.DoctorSchedules
                    .GetDoctorSchedulesByDateAsync(query.DoctorId, currentTime.Date);

                if (doctorSchedules.Any())
                {
                    foreach (var schedule in doctorSchedules)
                    {
                        var slotStart = currentTime.Date.Add(schedule.StartTime);
                        var slotEnd = currentTime.Date.Add(schedule.EndTime);

                        // Generate time slots based on duration
                        while (slotStart.AddMinutes(query.Duration) <= slotEnd)
                        {
                            var slot = new AvailabilitySlotDto
                            {
                                StartTime = slotStart,
                                EndTime = slotStart.AddMinutes(query.Duration),
                                Duration = query.Duration,
                                IsAvailable = true
                            };

                            // Check for existing appointments
                            var existingAppointment = await _unitOfWork.Appointments
                                .GetByDoctorAndTimeRangeAsync(query.DoctorId, slotStart, slotStart.AddMinutes(query.Duration));

                            if (existingAppointment != null)
                            {
                                slot.IsAvailable = false;
                                slot.Reason = "Already booked";
                            }

                            availableSlots.Add(slot);
                            slotStart = slotStart.AddMinutes(query.Duration + (query.IncludeBufferTime ? 15 : 0));
                        }
                    }
                }

                currentTime = currentTime.AddDays(1);
            }

            response.AvailableSlots = availableSlots;
            response.TotalSlots = availableSlots.Count;
            response.AvailableCount = availableSlots.Count(s => s.IsAvailable);

            return response;
        }

        public async Task<BulkCreateResultDto> CreateBulkAppointmentsAsync(BulkCreateAppointmentsDto dto)
        {
            var result = new BulkCreateResultDto
            {
                TotalRequested = dto.Appointments.Count
            };

            for (int i = 0; i < dto.Appointments.Count; i++)
            {
                try
                {
                    var appointment = dto.Appointments[i];

                    // Check conflicts if enabled
                    if (dto.CheckConflicts)
                    {
                        var existingAppointment = await _unitOfWork.Appointments
                            .GetByDoctorAndScheduleAsync(appointment.DoctorId, appointment.ScheduledDateTime, appointment.DoctorScheduleId);

                        if (existingAppointment != null)
                        {
                            result.Conflicts.Add(new ConflictDto
                            {
                                AppointmentIndex = i,
                                ConflictType = "Time Conflict",
                                ConflictReason = "Doctor already has an appointment at this time",
                                ConflictTime = appointment.ScheduledDateTime,
                                ExistingAppointment = _mapper.Map<AppointmentResponseDto>(existingAppointment)
                            });

                            if (!dto.ContinueOnError)
                            {
                                result.Failed++;
                                continue;
                            }
                        }
                    }

                    var createdAppointment = await CreateAppointmentAsync(appointment);
                    result.CreatedAppointments.Add(createdAppointment);
                    result.SuccessfullyCreated++;

                    // Send notifications if enabled
                    if (dto.SendNotifications)
                    {
                        await SendAppointmentCreatedNotificationAsync(createdAppointment);
                    }
                }
                catch (Exception ex)
                {
                    result.Errors.Add($"Appointment {i}: {ex.Message}");
                    result.Failed++;

                    if (!dto.ContinueOnError)
                        break;
                }
            }

            return result;
        }

        public async Task<AutoScheduleResultDto> AutoScheduleAppointmentsAsync(AutoScheduleDto dto)
        {
            var result = new AutoScheduleResultDto
            {
                CycleId = dto.CycleId,
                DoctorId = dto.DoctorId,
                TotalPlanned = dto.AppointmentTypes.Count
            };

            var cycle = await _unitOfWork.TreatmentCycles.GetCycleByIdAsync(dto.CycleId);
            if (cycle == null)
            {
                result.Errors.Add("Treatment cycle not found");
                return result;
            }

            var doctor = await _unitOfWork.Doctors.GetDoctorByIdAsync(dto.DoctorId);
            if (doctor == null)
            {
                result.Errors.Add("Doctor not found");
                return result;
            }

            var currentDate = dto.PreferredStartDate;
            result.NextAvailableDate = currentDate;

            foreach (var appointmentType in dto.AppointmentTypes)
            {
                try
                {
                    // Parse appointment type string to enum
                    if (!Enum.TryParse<AppointmentType>(appointmentType, true, out var parsedAppointmentType))
                    {
                        result.Errors.Add($"Invalid appointment type: {appointmentType}");
                        continue;
                    }

                    // Find next available slot
                    var availabilityQuery = new AvailabilityQueryDto
                    {
                        DoctorId = dto.DoctorId,
                        StartDate = currentDate,
                        EndDate = currentDate.AddDays(30), // Search within 30 days
                        Duration = dto.DefaultDuration,
                        AppointmentType = appointmentType
                    };

                    var availability = await CheckAvailabilityAsync(availabilityQuery);
                    var nextSlot = availability.AvailableSlots
                        .Where(s => s.IsAvailable && 
                               s.StartTime.TimeOfDay >= dto.PreferredTimeStart && 
                               s.StartTime.TimeOfDay <= dto.PreferredTimeEnd)
                        .OrderBy(s => s.StartTime)
                        .FirstOrDefault();

                    if (nextSlot != null)
                    {
                        // Find appropriate doctor schedule
                        var schedules = await _unitOfWork.DoctorSchedules
                            .GetDoctorSchedulesByDateAsync(dto.DoctorId, nextSlot.StartTime.Date);
                        
                        var schedule = schedules.FirstOrDefault(s => 
                            nextSlot.StartTime.TimeOfDay >= s.StartTime && 
                            nextSlot.EndTime.TimeOfDay <= s.EndTime);

                        if (schedule != null)
                        {
                            var appointmentDto = new CreateAppointmentDto
                            {
                                CycleId = dto.CycleId,
                                DoctorId = dto.DoctorId,
                                DoctorScheduleId = schedule.Id,
                                ScheduledDateTime = nextSlot.StartTime,
                                AppointmentType = parsedAppointmentType,
                                Notes = $"Auto-scheduled {appointmentType} appointment"
                            };

                            var createdAppointment = await CreateAppointmentAsync(appointmentDto);
                            result.ScheduledAppointments.Add(createdAppointment);
                            result.SuccessfullyScheduled++;

                            // Send notifications if enabled
                            if (dto.SendNotifications)
                            {
                                await SendAppointmentCreatedNotificationAsync(createdAppointment);
                            }

                            currentDate = nextSlot.StartTime.AddDays(dto.DaysBetweenAppointments);
                            result.NextAvailableDate = currentDate;
                        }
                        else
                        {
                            result.Errors.Add($"No doctor schedule found for {appointmentType} at {nextSlot.StartTime}");
                            result.Failed++;
                        }
                    }
                    else
                    {
                        result.Errors.Add($"No available slot found for {appointmentType}");
                        result.Failed++;
                    }
                }
                catch (Exception ex)
                {
                    result.Errors.Add($"Error scheduling {appointmentType}: {ex.Message}");
                    result.Failed++;
                }
            }

            return result;
        }

        public async Task<List<ConflictDto>> GetScheduleConflictsAsync(ConflictCheckDto query)
        {
            var conflicts = new List<ConflictDto>();

            var appointments = await _unitOfWork.Appointments
                .GetByDoctorAndDateRangeAsync(query.DoctorId, query.StartDate, query.EndDate);

            for (int i = 0; i < appointments.Count; i++)
            {
                var current = appointments[i];

                // Check for overlapping appointments
                if (query.IncludeOverlapping)
                {
                    var overlapping = appointments.Where(a => 
                        a.Id != current.Id &&
                        a.ScheduledDateTime < current.ScheduledDateTime.AddHours(1) // Assuming 1 hour duration
                        && a.ScheduledDateTime.AddHours(1) > current.ScheduledDateTime)
                        .ToList();

                    foreach (var overlap in overlapping)
                    {
                        conflicts.Add(new ConflictDto
                        {
                            ConflictType = "Overlapping",
                            ConflictReason = "Appointments overlap in time",
                            ConflictTime = current.ScheduledDateTime,
                            ExistingAppointment = _mapper.Map<AppointmentResponseDto>(overlap)
                        });
                    }
                }

                // Check for back-to-back appointments
                if (query.IncludeBackToBack)
                {
                    var backToBack = appointments.Where(a =>
                        a.Id != current.Id &&
                        Math.Abs((a.ScheduledDateTime - current.ScheduledDateTime.AddHours(1)).TotalMinutes) < 15)
                        .ToList();

                    foreach (var btb in backToBack)
                    {
                        conflicts.Add(new ConflictDto
                        {
                            ConflictType = "Back-to-Back",
                            ConflictReason = "Insufficient time between appointments",
                            ConflictTime = current.ScheduledDateTime,
                            ExistingAppointment = _mapper.Map<AppointmentResponseDto>(btb)
                        });
                    }
                }
            }

            return conflicts.Distinct().ToList();
        }

        public async Task<bool> SendAppointmentReminderAsync(int appointmentId)
        {
            try
            {
                var appointment = await _unitOfWork.Appointments.GetByIdAsync(appointmentId);
                if (appointment == null)
                    return false;

                // Get cycle with customer information
                var cycle = await _unitOfWork.TreatmentCycles.GetCycleByIdAsync(appointment.CycleId);
                var cus = await _unitOfWork.Customers.GetByIdAsync(cycle.CustomerId);

                var reminderDto = new CreateNotificationDto
                {
                    UserId = cus.UserId,
                    Title = "Appointment Reminder",
                    Message = $"You have an appointment scheduled for {appointment.ScheduledDateTime:MMM dd, yyyy} at {appointment.ScheduledDateTime:HH:mm}",
                    Type = NotificationType.Appointment.ToString(),
                    ScheduledAt = DateTime.UtcNow,
                    RelatedEntityId = appointmentId,
                    RelatedEntityType = "Appointment"
                };

                // Save notification to database
                await _notificationService.CreateNotificationAsync(reminderDto);
                
                
                
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // Helper method for sending appointment created notifications
        private async Task SendAppointmentCreatedNotificationAsync(AppointmentResponseDto appointment)
        {
            try
            {
                // Get cycle with customer information
                var cycle = await _unitOfWork.TreatmentCycles.GetCycleByIdAsync(appointment.CycleId);
                var cus = await _unitOfWork.Customers.GetByIdAsync(cycle.CustomerId);

                var notificationDto = new CreateNotificationDto
                {
                    UserId = cus.UserId,
                    Title = "New Appointment Scheduled",
                    Message = $"Your {appointment.AppointmentType} appointment has been scheduled for {appointment.ScheduledDateTime:MMM dd, yyyy}",
                    Type = NotificationType.Appointment.ToString(),
                    ScheduledAt = DateTime.UtcNow,
                    RelatedEntityId = appointment.Id,
                    RelatedEntityType = "Appointment"
                };

                // Save notification to database
                await _notificationService.CreateNotificationAsync(notificationDto);
                
                
            }
            catch (Exception)
            {
                // Log error but don't fail the appointment creation
            }
        }

        // Helper method for sending appointment rescheduled notifications
        private async Task SendAppointmentRescheduledNotificationAsync(AppointmentResponseDto appointment)
        {
            try
            {
                // Get cycle with customer information
                var cycle = await _unitOfWork.TreatmentCycles.GetCycleByIdAsync(appointment.CycleId);
                var cus = await _unitOfWork.Customers.GetByIdAsync(cycle.CustomerId);

                var notificationDto = new CreateNotificationDto
                {
                    UserId = cus.UserId,
                    Title = "Appointment Rescheduled",
                    Message = $"Your {appointment.AppointmentType} appointment has been rescheduled to {appointment.ScheduledDateTime:MMM dd, yyyy} at {appointment.ScheduledDateTime:HH:mm}",
                    Type = NotificationType.Appointment.ToString(),
                    ScheduledAt = DateTime.UtcNow,
                    RelatedEntityId = appointment.Id,
                    RelatedEntityType = "Appointment"
                };

                // Save notification to database
                await _notificationService.CreateNotificationAsync(notificationDto);
                
                
            }
            catch (Exception)
            {
                // Log error but don't fail the appointment operation
            }
        }

        // Helper method for sending appointment cancelled notifications
        private async Task SendAppointmentCancelledNotificationAsync(AppointmentResponseDto appointment)
        {
            try
            {
                // Get cycle with customer information
                var cycle = await _unitOfWork.TreatmentCycles.GetCycleByIdAsync(appointment.CycleId);
                var cus = await _unitOfWork.Customers.GetByIdAsync(cycle.CustomerId);

                var notificationDto = new CreateNotificationDto
                {
                    UserId = cus.UserId,
                    Title = "Appointment Cancelled",
                    Message = $"Your {appointment.AppointmentType} appointment scheduled for {appointment.ScheduledDateTime:MMM dd, yyyy} at {appointment.ScheduledDateTime:HH:mm} has been cancelled",
                    Type = NotificationType.Appointment.ToString(),
                    ScheduledAt = DateTime.UtcNow,
                    RelatedEntityId = appointment.Id,
                    RelatedEntityType = "Appointment"
                };

                // Save notification to database
                await _notificationService.CreateNotificationAsync(notificationDto);
                
                
            }
            catch (Exception)
            {
                // Log error but don't fail the appointment operation
            }
        }
    }

}
