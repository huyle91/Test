using InfertilityTreatment.Business.Interfaces;
using InfertilityTreatment.Business.Services;
using InfertilityTreatment.Entity.DTOs.Common;
using InfertilityTreatment.Entity.DTOs.Users;
using InfertilityTreatment.Entity.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace InfertilityTreatment.API.Controllers
{
    [Route("api/customers")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        public CustomersController(ICustomerService customerService)
        {
            _customerService = customerService;
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponseDto<CustomerDetailDto>>> GetProfile(int id)
        {
            try
            {
                var customer = await _customerService.GetCustomerProfileAsync(id);
                return Ok(ApiResponseDto<CustomerDetailDto>.CreateSuccess(customer, "Customer profile retrieved successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponseDto<string>.CreateError("An error occurred while retrieving the customer profile."));
          
            }
        }
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponseDto<string>>> UpdateProfile(int id, [FromBody] CustomerProfileDto updateProfileDto)
        {
            try
            {
                var result = await _customerService.UpdateCustomerProfileAsync(id, updateProfileDto);
                return Ok(ApiResponseDto<string>.CreateSuccess(result, "Customer profile update successfully."));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponseDto<string>.CreateError(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponseDto<string>.CreateError("An error occurred while retrieving the customer profile."));
            }
        }
        [HttpPut("{id}/medical-history")]
        public async Task<ActionResult<ApiResponseDto<string>>> UpdateMedicalHistory(int id, [FromBody] MedicalHistoryDTO dto)
        {
            try
            {
                var result = await _customerService.UpdateMedicalHistoryAsync(id, dto.MedicalHistory);
                return Ok(ApiResponseDto<string>.CreateSuccess(result, "Customer profile update successfully."));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponseDto<string>.CreateError(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ApiResponseDto<string>.CreateError("An error occurred while retrieving the customer profile."));
            }
        }
    }
}
