using InfertilityTreatment.Business.Interfaces;
using InfertilityTreatment.Entity.Constants;
using InfertilityTreatment.Entity.DTOs.Appointments;
using InfertilityTreatment.Entity.DTOs.Common;
using InfertilityTreatment.Entity.DTOs.DoctorSchedules;
using InfertilityTreatment.Entity.DTOs.TreatmentPakages;
using InfertilityTreatment.Entity.DTOs.TreatmentServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        [HttpPost]
        public async Task<ActionResult<ApiResponseDto<AppointmentResponseDto>>> CreateAppointment([FromBody] CreateAppointmentDto dto)
        {
            try
            {
                var result = await _appointmentService.CreateAppointmentAsync(dto);
                return Ok(ApiResponseDto<AppointmentResponseDto>.CreateSuccess(result, "Appointment created successfully."));
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

        [HttpGet]
        public async Task<ActionResult<ApiResponseDto<PaginatedResultDto<AppointmentResponseDto>>>> GetAppointments(
            [FromQuery] int userId,
            [FromQuery] string role,
            [FromQuery] PaginationQueryDTO pagination,
            [FromQuery] DateTime? date = null)
        {
            try
            {
                if (pagination.PageSize > 100) pagination.PageSize = 100;
                if (pagination.PageNumber < 1) pagination.PageNumber = 1;

                PaginatedResultDto<AppointmentResponseDto> result;

                if (role == "Customer")
                {
                    result = await _appointmentService.GetAppointmentsByCustomerAsync(userId, pagination);
                }
                else if (role == "Doctor" && date.HasValue)
                {
                    result = await _appointmentService.GetAppointmentsByDoctorAsync(userId, date.Value, pagination);
                }
                else
                {
                    return BadRequest(ApiResponseDto<string>.CreateError("Invalid role or missing parameters."));
                }

                return Ok(ApiResponseDto<PaginatedResultDto<AppointmentResponseDto>>.CreateSuccess(result, "Appointments retrieved successfully."));
            }
            catch (Exception)
            {
                return StatusCode(500, ApiResponseDto<string>.CreateError("An error occurred while retrieving appointments."));
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

        [HttpPut("{id}/reschedule")]
        public async Task<ActionResult<ApiResponseDto<AppointmentResponseDto>>> RescheduleAppointment(int id, [FromBody] RescheduleAppointmentDto dto)
        {
            try
            {
                var result = await _appointmentService.RescheduleAppointmentAsync(id, dto.DoctorScheduleId, dto.ScheduledDateTime);
                return Ok(ApiResponseDto<AppointmentResponseDto>.CreateSuccess(result, "Appointment rescheduled successfully."));
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ApiResponseDto<string>.CreateError(ex.Message));
            }
            catch (Exception)
            {
                return StatusCode(500, ApiResponseDto<string>.CreateError("An error occurred while rescheduling appointment."));
            }
        }

        [HttpPut("{id}/cancel")]
        public async Task<ActionResult<ApiResponseDto<AppointmentResponseDto>>> CancelAppointment(int id)
        {
            try
            {
                var result = await _appointmentService.CancelAppointmentAsync(id);
                return Ok(ApiResponseDto<AppointmentResponseDto>.CreateSuccess(result, "Appointment cancelled successfully."));
            }
            catch (Exception)
            {
                return StatusCode(500, ApiResponseDto<string>.CreateError("An error occurred while cancelling appointment."));
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
                if (pagination.PageSize > 100) pagination.PageSize = 100;
                if (pagination.PageNumber < 1) pagination.PageNumber = 1;

                var result = await _appointmentService.GetDoctorAvailabilityAsync(id, date, pagination);
                return Ok(ApiResponseDto<PaginatedResultDto<DoctorScheduleDto>>.CreateSuccess(result, "Doctor availability retrieved successfully."));
            }
            catch (Exception)
            {
                return StatusCode(500, ApiResponseDto<string>.CreateError("An error occurred while retrieving doctor availability."));
            }
        }
    }


}
