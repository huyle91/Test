using InfertilityTreatment.Business.Interfaces;
using InfertilityTreatment.Entity.Constants;
using InfertilityTreatment.Entity.DTOs.Common;
using InfertilityTreatment.Entity.DTOs.TreatmentServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InfertilityTreatment.API.Controllers
{
    [Route("api/treatment-services")]
    [ApiController]
    //[Authorize] 
    public class TreatmentServiceController : ControllerBase
    {
        private readonly ITreatmentServiceService _service;

        public TreatmentServiceController(ITreatmentServiceService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponseDto<IEnumerable<TreatmentServiceDto>>>> GetAll()
        {
            try
            {
                var result = await _service.GetAllAsync();
                return Ok(ApiResponseDto<IEnumerable<TreatmentServiceDto>>.CreateSuccess(result, TreatmentServiceMessages.GetAllSuccess));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponseDto<string>.CreateError(TreatmentServiceMessages.InternalError ));
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponseDto<TreatmentServiceDto>>> GetById(int id)
        {
            try
            {
                var result = await _service.GetByIdAsync(id);
                if (result == null)
                    return NotFound(ApiResponseDto<string>.CreateError(TreatmentServiceMessages.NotFound));

                return Ok(ApiResponseDto<TreatmentServiceDto>.CreateSuccess(result, TreatmentServiceMessages.GetSuccess));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponseDto<string>.CreateError(TreatmentServiceMessages.InternalError));
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<ApiResponseDto<object>>> Create([FromBody] CreateTreatmentServiceDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return UnprocessableEntity(ApiResponseDto<string>.CreateError(TreatmentServiceMessages.InvalidInput));

                var id = await _service.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id },
                    ApiResponseDto<object>.CreateSuccess(new { id }, TreatmentServiceMessages.CreatedSuccessfully));
            }
            catch (ArgumentException ex)
            {
                return UnprocessableEntity(ApiResponseDto<string>.CreateError(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponseDto<string>.CreateError(TreatmentServiceMessages.InternalError));
            }
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponseDto<string>>> Update(int id, [FromBody] UpdateTreatmentServiceDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return UnprocessableEntity(ApiResponseDto<string>.CreateError(TreatmentServiceMessages.InvalidInput));

                var updated = await _service.UpdateAsync(id, dto);
                if (!updated)
                    return NotFound(ApiResponseDto<string>.CreateError(TreatmentServiceMessages.NotFound));

                return Ok(ApiResponseDto<string>.CreateSuccess(TreatmentServiceMessages.CreatedSuccessfully));
            }
            catch (ArgumentException ex)
            {
                return UnprocessableEntity(ApiResponseDto<string>.CreateError(ex.Message)); 
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponseDto<string>.CreateError(TreatmentServiceMessages.InternalError));
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
                    return NotFound(ApiResponseDto<string>.CreateError(TreatmentServiceMessages.NotFound));

                return Ok(ApiResponseDto<string>.CreateSuccess(TreatmentServiceMessages.DeletedSuccessfully));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponseDto<string>.CreateError(TreatmentServiceMessages.InternalError));
            }
        }
    }
}
