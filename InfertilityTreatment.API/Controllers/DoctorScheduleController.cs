using InfertilityTreatment.Business.Interfaces;
using InfertilityTreatment.Entity.DTOs.Common;
using InfertilityTreatment.Entity.DTOs.DoctorSchedules;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace InfertilityTreatment.API.Controllers
{
    [Route("api/doctor-schedules")]
    [ApiController]
    public class DoctorScheduleController : ControllerBase
    {
        private readonly IDoctorScheduleService _service;
        public DoctorScheduleController(IDoctorScheduleService service)
        {
            _service = service;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null)
                return NotFound(ApiResponseDto<DoctorScheduleDto>.CreateError("DoctorSchedule not found"));
            return Ok(ApiResponseDto<DoctorScheduleDto>.CreateSuccess(result));
        }

        [HttpGet("by-doctor/{doctorId}")]
        public async Task<IActionResult> GetByDoctorId(int doctorId, [FromQuery] PaginationQueryDTO pagination)
        {
            var result = await _service.GetByDoctorIdAsync(doctorId, pagination);
            return Ok(ApiResponseDto<PaginatedResultDto<DoctorScheduleDto>>.CreateSuccess(result));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateDoctorScheduleDto dto)
        {
            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, ApiResponseDto<DoctorScheduleDto>.CreateSuccess(created, "DoctorSchedule created successfully"));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateDoctorScheduleDto dto)
        {
            var updated = await _service.UpdateAsync(id, dto);
            return Ok(ApiResponseDto<DoctorScheduleDto>.CreateSuccess(updated, "DoctorSchedule updated successfully"));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteAsync(id);
            if (!deleted)
                return NotFound(ApiResponseDto<string>.CreateError("DoctorSchedule not found"));
            return Ok(ApiResponseDto<string>.CreateSuccess(null, "DoctorSchedule deleted successfully"));
        }
    }
}
