using InfertilityTreatment.Business.Interfaces;
using InfertilityTreatment.Entity.Constants;
using InfertilityTreatment.Entity.DTOs.Common;
using InfertilityTreatment.Entity.DTOs.TreatmentPakages;
using InfertilityTreatment.Entity.DTOs.TreatmentServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InfertilityTreatment.API.Controllers
{
    [Route("api/treatment-packages")]
    [ApiController]
    //[Authorize]
    public class TreatmentPackageController : ControllerBase
    {
        private readonly ITreatmentPackageService _service;
        private readonly ITreatmentPackageService _treatmentService;

        public TreatmentPackageController(ITreatmentPackageService service, ITreatmentPackageService treatmentService)
        {
            _service = service;
            _treatmentService = treatmentService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponseDto<IEnumerable<TreatmentPackageDto>>>> GetAll()
        {
            try
            {
                var packages = await _service.GetAllAsync();
                return Ok(ApiResponseDto<IEnumerable<TreatmentPackageDto>>.CreateSuccess(packages, TreatmentPackageMessages.GetAllSuccess));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponseDto<string>.CreateError(TreatmentPackageMessages.UnknowError ));

            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponseDto<TreatmentPackageDto>>> GetById(int id)
        {
            try
            {
                var package = await _service.GetByIdAsync(id);
                if (package == null)
                    return NotFound(ApiResponseDto<string>.CreateError(TreatmentPackageMessages.NotFound));

                return Ok(ApiResponseDto<TreatmentPackageDto>.CreateSuccess(package, TreatmentPackageMessages.GetSuccess));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponseDto<string>.CreateError(TreatmentPackageMessages.UnknowError));
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<ApiResponseDto<object>>> Create([FromBody] CreateTreatmentPackageDto dto)
        {
            if (!ModelState.IsValid)
                return UnprocessableEntity(ApiResponseDto<string>.CreateError(TreatmentPackageMessages.InvalidInput));

            try
            {

                var id = await _service.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id },
                    ApiResponseDto<object>.CreateSuccess(new { id }, TreatmentPackageMessages.CreateSuccess));
            }
            catch (ArgumentException ex)
            {
                return UnprocessableEntity(ApiResponseDto<string>.CreateError(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponseDto<string>.CreateError(TreatmentPackageMessages.UnknowError));
            }
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponseDto<string>>> Update(int id, [FromBody] UpdateTreatmentPackageDto dto)
        {
            if (!ModelState.IsValid)
                return UnprocessableEntity(ApiResponseDto<string>.CreateError(TreatmentPackageMessages.InvalidInput));

            try
            {
                var updated = await _service.UpdateAsync(id, dto);
                if (!updated)
                    return NotFound(ApiResponseDto<string>.CreateError(TreatmentPackageMessages.NotFound));

                return Ok(ApiResponseDto<string>.CreateSuccess(TreatmentPackageMessages.UpdateSuccess));
            }
            catch (ArgumentException ex)
            {
                return UnprocessableEntity(ApiResponseDto<string>.CreateError(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponseDto<string>.CreateError(TreatmentPackageMessages.UnknowError));
            }
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponseDto<string>>> Delete(int id)
        {
            try
            {

                var deleted = await _service.DeleteAsync(id);
                if (!deleted)
                    return NotFound(ApiResponseDto<string>.CreateError(TreatmentPackageMessages.NotFound));

                return Ok(ApiResponseDto<string>.CreateSuccess(TreatmentPackageMessages.DeletedSuccess));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponseDto<string>.CreateError(TreatmentPackageMessages.UnknowError ));
            }
        }

        [Authorize]
        [HttpGet("by-service/{serviceId}")]
        public async Task<ActionResult<ApiResponseDto<IEnumerable<TreatmentPackageDto>>>> GetPackagesByService(int serviceId)
        {
            try
            {
                var packages = await _service.GetByServiceIdAsync(serviceId);
                if (packages == null || !packages.Any())
                    return NotFound(ApiResponseDto<string>.CreateError("No treatment packages found for the given service ID."));

                return Ok(ApiResponseDto<IEnumerable<TreatmentPackageDto>>.CreateSuccess(packages, "Fetched treatment packages by service ID successfully."));
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponseDto<string>.CreateError("An error occurred while fetching treatment packages by service ID."));
            }
        }
    }
}
