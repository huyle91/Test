using InfertilityTreatment.Business.Interfaces;
using InfertilityTreatment.Entity.DTOs.Common;
using InfertilityTreatment.Entity.DTOs.Prescription;
using InfertilityTreatment.Entity.DTOs.Prescriptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace InfertilityTreatment.API.Controllers
{
    [Route("api/prescriptions")]
    [ApiController]
    [Authorize]
    public class PrescriptionsController : ControllerBase
    {
        private readonly IPrescriptionService _prescriptionService;

        public PrescriptionsController(IPrescriptionService prescriptionService)
        {
            _prescriptionService = prescriptionService;
        }

        /// <summary>
        /// Create a new prescription for a treatment phase
        /// </summary>
        [HttpPost("phase/{phaseId}")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> CreatePrescription(int phaseId, [FromBody] CreatePrescriptionDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(ApiResponseDto<string>.CreateError("Validation failed.", errors));
            }

            try
            {
                var prescription = await _prescriptionService.CreatePrescriptionAsync(phaseId, dto);
                return CreatedAtAction(nameof(GetPrescriptionById), new { id = prescription.Id },
                    ApiResponseDto<PrescriptionDetailDto>.CreateSuccess(prescription, "Prescription created successfully."));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ApiResponseDto<string>.CreateError(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponseDto<string>.CreateError("An error occurred while creating the prescription."));
            }
        }

        /// <summary>
        /// Get prescription by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPrescriptionById(int id)
        {
            try
            {
                var prescription = await _prescriptionService.GetPrescriptionByIdAsync(id);
                if (prescription == null)
                    return NotFound(ApiResponseDto<string>.CreateError("Prescription not found."));

                return Ok(ApiResponseDto<PrescriptionDetailDto>.CreateSuccess(prescription, "Prescription retrieved successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponseDto<string>.CreateError("An error occurred while retrieving the prescription."));
            }
        }

        /// <summary>
        /// Update an existing prescription
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> UpdatePrescription(int id, [FromBody] UpdatePrescriptionDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(ApiResponseDto<string>.CreateError("Validation failed.", errors));
            }

            try
            {
                var prescription = await _prescriptionService.UpdatePrescriptionAsync(id, dto);
                return Ok(ApiResponseDto<PrescriptionDetailDto>.CreateSuccess(prescription, "Prescription updated successfully."));
            }
            catch (ArgumentException ex)
            {
                return NotFound(ApiResponseDto<string>.CreateError(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponseDto<string>.CreateError("An error occurred while updating the prescription."));
            }
        }

        /// <summary>
        /// Delete a prescription (soft delete)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> DeletePrescription(int id)
        {
            try
            {
                var result = await _prescriptionService.DeletePrescriptionAsync(id);
                if (!result)
                    return NotFound(ApiResponseDto<string>.CreateError("Prescription not found."));

                return Ok(ApiResponseDto<string>.CreateSuccess(null, "Prescription deleted successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponseDto<string>.CreateError("An error occurred while deleting the prescription."));
            }
        }

        /// <summary>
        /// Get all prescriptions for a specific customer with pagination and search
        /// </summary>
        [HttpGet("customer/{customerId}")]
        public async Task<IActionResult> GetPrescriptionsByCustomer(int customerId, [FromQuery] PaginationQueryDTO pagination)
        {
            try
            {
                // Authorization check - only allow customers to view their own prescriptions or doctors/admins
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                
                if (userRole == "Customer" && userId != customerId.ToString())
                {
                    return Forbid();
                }

                var prescriptions = await _prescriptionService.GetPrescriptionsByCustomerAsync(customerId, pagination);
                return Ok(ApiResponseDto<PaginatedResultDto<PrescriptionSummaryDto>>.CreateSuccess(prescriptions, 
                    $"Retrieved {prescriptions.Items.Count} prescriptions for customer {customerId}."));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponseDto<string>.CreateError("An error occurred while retrieving prescriptions."));
            }
        }

        /// <summary>
        /// Get active prescriptions for a specific customer with pagination and search
        /// </summary>
        [HttpGet("customer/{customerId}/active")]
        public async Task<IActionResult> GetActivePrescriptions(int customerId, [FromQuery] PaginationQueryDTO pagination)
        {
            try
            {
                // Authorization check - only allow customers to view their own prescriptions or doctors/admins
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                
                if (userRole == "Customer" && userId != customerId.ToString())
                {
                    return Forbid();
                }

                var prescriptions = await _prescriptionService.GetActivePrescriptionsAsync(customerId, pagination);
                return Ok(ApiResponseDto<PaginatedResultDto<PrescriptionSummaryDto>>.CreateSuccess(prescriptions, 
                    $"Retrieved {prescriptions.Items.Count} active prescriptions for customer {customerId}."));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponseDto<string>.CreateError("An error occurred while retrieving active prescriptions."));
            }
        }

        /// <summary>
        /// Get all prescriptions with pagination and search
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Doctor,Admin")]
        public async Task<IActionResult> GetAllPrescriptions([FromQuery] PaginationQueryDTO pagination)
        {
            try
            {
                var prescriptions = await _prescriptionService.GetAllPrescriptionsAsync(pagination);
                return Ok(ApiResponseDto<PaginatedResultDto<PrescriptionSummaryDto>>.CreateSuccess(prescriptions, 
                    $"Retrieved {prescriptions.Items.Count} prescriptions."));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponseDto<string>.CreateError("An error occurred while retrieving prescriptions."));
            }
        }

        /// <summary>
        /// Get prescriptions for a specific treatment phase with pagination and search
        /// </summary>
        [HttpGet("phase/{phaseId}")]
        [Authorize(Roles = "Doctor,Admin")]
        public async Task<IActionResult> GetPrescriptionsByPhase(int phaseId, [FromQuery] PaginationQueryDTO pagination)
        {
            try
            {
                var prescriptions = await _prescriptionService.GetPrescriptionsByPhaseAsync(phaseId, pagination);
                return Ok(ApiResponseDto<PaginatedResultDto<PrescriptionDetailDto>>.CreateSuccess(prescriptions, 
                    $"Retrieved {prescriptions.Items.Count} prescriptions for phase {phaseId}."));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponseDto<string>.CreateError("An error occurred while retrieving prescriptions for the phase."));
            }
        }
    }
}
