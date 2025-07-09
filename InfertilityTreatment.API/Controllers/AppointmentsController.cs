using InfertilityTreatment.Business.Interfaces;
using InfertilityTreatment.Entity.Constants;
using InfertilityTreatment.Entity.DTOs.Appointments;
using InfertilityTreatment.Entity.DTOs.Common;
using InfertilityTreatment.Entity.DTOs.DoctorSchedules;
using InfertilityTreatment.Entity.DTOs.TreatmentPakages;
using InfertilityTreatment.Entity.DTOs.TreatmentServices;
using InfertilityTreatment.Entity.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace InfertilityTreatment.API.Controllers
{
    [ApiController]
    [Route("api/appointments")]
    public class AppointmentsController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;

        public AppointmentsController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        [Authorize(Roles = "Doctor")]
        [HttpPost]
        public async Task<ActionResult<ApiResponseDto<AppointmentResponseDto>>> CreateAppointment([FromBody] CreateAppointmentDto dto)
        {
            try
            {
                var result = await _appointmentService.CreateAppointmentAsync(dto);
                return Ok(ApiResponseDto<AppointmentResponseDto>.CreateSuccess(result, "Appointment created successfully."));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponseDto<string>.CreateError(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ApiResponseDto<string>.CreateError(ex.Message));
            }
            catch (Exception)
            {
                return StatusCode(500, ApiResponseDto<string>.CreateError("An error occurred while creating appointment."));
            }
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<ApiResponseDto<PaginatedResultDto<AppointmentResponseDto>>>> GetAppointments(
            [FromQuery, Required] int userId,
            [FromQuery, Required] UserRole role,
            [FromQuery] PaginationQueryDTO pagination,
            [FromQuery] DateTime? date = null)
        {
            try
            {
                pagination.PageNumber = pagination.PageNumber <= 0 ? 1 : pagination.PageNumber;
                pagination.PageSize = pagination.PageSize <= 0 ? 100 : pagination.PageSize;
                if (pagination.PageSize > 100) pagination.PageSize = 100;
                if (pagination.PageNumber < 1) pagination.PageNumber = 1;

                PaginatedResultDto<AppointmentResponseDto> result;

                if (role == UserRole.Customer)
                {
                    result = await _appointmentService.GetAppointmentsByCustomerAsync(userId, pagination);
                }
                else if (role == UserRole.Doctor)
                {
                    if (!date.HasValue)
                    {
                        return BadRequest(ApiResponseDto<string>.CreateError("Missing date parameter for doctor."));
                    }
                    result = await _appointmentService.GetAppointmentsByDoctorAsync(userId, date.Value, pagination);
                }
                else
                {
                    return BadRequest(ApiResponseDto<string>.CreateError("Invalid role or missing parameters."));
                }

                return Ok(ApiResponseDto<PaginatedResultDto<AppointmentResponseDto>>.CreateSuccess(result, "Appointments retrieved successfully."));
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ApiResponseDto<string>.CreateError(ex.Message));
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponseDto<string>.CreateError(AppointmentMessages.UnknownError));
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponseDto<AppointmentResponseDto>>> GetAppointmentById(int id)
        {
            var result = await _appointmentService.GetByIdAsync(id);
            if (result == null)
                return NotFound(ApiResponseDto<string>.CreateError("Appointment not found."));
            return Ok(ApiResponseDto<AppointmentResponseDto>.CreateSuccess(result));
        }

        [Authorize]
        [HttpPut("{id}/reschedule")]
        public async Task<ActionResult<ApiResponseDto<AppointmentResponseDto>>> RescheduleAppointment(int id, [FromBody] RescheduleAppointmentDto dto)
        {
            try
            {
                var result = await _appointmentService.RescheduleAppointmentAsync(id, dto.DoctorScheduleId, dto.ScheduledDateTime);
                return Ok(ApiResponseDto<AppointmentResponseDto>.CreateSuccess(result, "Appointment rescheduled successfully."));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponseDto<string>.CreateError(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ApiResponseDto<string>.CreateError(ex.Message));
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                   ApiResponseDto<string>.CreateError(AppointmentMessages.UnknownError));
            }
        }

        [Authorize]
        [HttpPut("{id}/cancel")]
        public async Task<ActionResult<ApiResponseDto<AppointmentResponseDto>>> CancelAppointment(int id)
        {
            try
            {
                var result = await _appointmentService.CancelAppointmentAsync(id);
                return Ok(ApiResponseDto<AppointmentResponseDto>.CreateSuccess(result, "Appointment cancelled successfully."));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponseDto<string>.CreateError(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ApiResponseDto<string>.CreateError(ex.Message));
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponseDto<string>.CreateError(AppointmentMessages.UnknownError));
            }
        }

        [HttpGet("/api/doctors/{id}/availability")]
        public async Task<ActionResult<ApiResponseDto<PaginatedResultDto<DoctorScheduleDto>>>> GetDoctorAvailability(
            int id,
            [FromQuery] DateTime date,
            [FromQuery] PaginationQueryDTO pagination)
        {
            try
            {
                pagination.PageNumber = pagination.PageNumber <= 0 ? 1 : pagination.PageNumber;
                pagination.PageSize = pagination.PageSize <= 0 ? 100 : pagination.PageSize;
                if (pagination.PageSize > 100) pagination.PageSize = 100;
                if (pagination.PageNumber < 1) pagination.PageNumber = 1;

                var result = await _appointmentService.GetDoctorAvailabilityAsync(id, date, pagination);
                return Ok(ApiResponseDto<PaginatedResultDto<DoctorScheduleDto>>.CreateSuccess(result, "Doctor availability retrieved successfully."));
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                   ApiResponseDto<string>.CreateError(AppointmentMessages.UnknownError));
            }
        }

        // Enhanced Appointment APIs

        [HttpGet("availability")]
        [Authorize]
        public async Task<ActionResult<ApiResponseDto<AvailabilityResponseDto>>> CheckAvailability([FromQuery] AvailabilityQueryDto query)
        {
            try
            {
                var result = await _appointmentService.CheckAvailabilityAsync(query);
                return Ok(ApiResponseDto<AvailabilityResponseDto>.CreateSuccess(result, "Availability checked successfully."));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponseDto<string>.CreateError(ex.Message));
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponseDto<string>.CreateError("An error occurred while checking availability."));
            }
        }

        [HttpPost("bulk-create")]
        [Authorize(Roles = "Doctor,Admin")]
        public async Task<ActionResult<ApiResponseDto<BulkCreateResultDto>>> CreateBulkAppointments([FromBody] BulkCreateAppointmentsDto dto)
        {
            try
            {
                var result = await _appointmentService.CreateBulkAppointmentsAsync(dto);
                return Ok(ApiResponseDto<BulkCreateResultDto>.CreateSuccess(result, 
                    $"Bulk creation completed. {result.SuccessfullyCreated}/{result.TotalRequested} appointments created."));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponseDto<string>.CreateError(ex.Message));
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponseDto<string>.CreateError("An error occurred during bulk appointment creation."));
            }
        }

        [HttpPost("auto-schedule")]
        [Authorize(Roles = "Doctor")]
        public async Task<ActionResult<ApiResponseDto<AutoScheduleResultDto>>> AutoScheduleAppointments([FromBody] AutoScheduleDto dto)
        {
            try
            {
                var result = await _appointmentService.AutoScheduleAppointmentsAsync(dto);
                return Ok(ApiResponseDto<AutoScheduleResultDto>.CreateSuccess(result,
                    $"Auto-scheduling completed. {result.SuccessfullyScheduled}/{result.TotalPlanned} appointments scheduled."));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponseDto<string>.CreateError(ex.Message));
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponseDto<string>.CreateError("An error occurred during auto-scheduling."));
            }
        }

        [HttpGet("conflicts")]
        [Authorize(Roles = "Doctor,Admin")]
        public async Task<ActionResult<ApiResponseDto<List<ConflictDto>>>> GetScheduleConflicts([FromQuery] ConflictCheckDto query)
        {
            try
            {
                var result = await _appointmentService.GetScheduleConflictsAsync(query);
                return Ok(ApiResponseDto<List<ConflictDto>>.CreateSuccess(result, 
                    $"Found {result.Count} schedule conflicts."));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponseDto<string>.CreateError(ex.Message));
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponseDto<string>.CreateError("An error occurred while checking conflicts."));
            }
        }

        [HttpPost("{id}/send-reminder")]
        [Authorize(Roles = "Doctor,Admin")]
        public async Task<ActionResult<ApiResponseDto<bool>>> SendAppointmentReminder(int id)
        {
            try
            {
                var result = await _appointmentService.SendAppointmentReminderAsync(id);
                if (result)
                {
                    return Ok(ApiResponseDto<bool>.CreateSuccess(true, "Reminder sent successfully."));
                }
                else
                {
                    return NotFound(ApiResponseDto<bool>.CreateError("Appointment not found or reminder failed."));
                }
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponseDto<string>.CreateError("An error occurred while sending reminder."));
            }
        }
    }


}
