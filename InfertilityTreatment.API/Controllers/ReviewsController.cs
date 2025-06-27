using InfertilityTreatment.Business.Interfaces;
using InfertilityTreatment.Entity.DTOs.Common;
using InfertilityTreatment.Entity.DTOs.Review;
using InfertilityTreatment.Entity.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace InfertilityTreatment.API.Controllers
{
    [Route("api/reviews")]
    [Authorize]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewsController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpPost]
        [Authorize(Roles = nameof(UserRole.Customer))]
        public async Task<IActionResult> CreateReview([FromBody] CreateReviewDto createReviewDto)
        {
            try
            {
                var result = await _reviewService.CreateReviewAsync(createReviewDto);
                if (result == null)
                    return BadRequest(ApiResponseDto<ReviewDto>.CreateError("Failed to create review. Please verify your input."));

                return Ok(ApiResponseDto<ReviewDto>.CreateSuccess(result, "Review created successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponseDto<ReviewDto>.CreateError("An unexpected error occurred while creating the review."));
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetFilteredReviews([FromQuery] ReviewFilterDto filter)
        {
            try
            {
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
                var result = await _reviewService.GetReviewsAsync(filter, userRole);
                return Ok(ApiResponseDto<PaginatedResultDto<ReviewDto>>.CreateSuccess(result, "Fetched filtered reviews successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponseDto<PaginatedResultDto<ReviewDto>>.CreateError("Failed to fetch filtered reviews."));
            }
        }

        [HttpGet("~/api/doctors/{doctorId}/reviews")]
        public async Task<IActionResult> GetReviewsByDoctor(int doctorId)
        {
            try
            {
                var result = await _reviewService.GetReviewsByDoctorAsync(doctorId);
                return Ok(ApiResponseDto<List<ReviewDto>>.CreateSuccess(result, "Doctor's reviews fetched successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponseDto<List<ReviewDto>>.CreateError("Failed to load doctor's reviews."));
            }
        }

        [HttpGet("~/api/doctors/{doctorId}/statistics")]
        public async Task<IActionResult> GetDoctorReviewStatistics(int doctorId)
        {
            try
            {
                var result = await _reviewService.GetReviewStatisticsAsync(doctorId);
                if (result == null)
                    return NotFound(ApiResponseDto<ReviewStatisticsDto>.CreateError("No statistics available for this doctor."));

                return Ok(ApiResponseDto<ReviewStatisticsDto>.CreateSuccess(result, "Doctor review statistics fetched successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponseDto<ReviewStatisticsDto>.CreateError("An error occurred while retrieving review statistics." ));
            }
        }

        [HttpPut("{id}/approve")]
        [Authorize(Roles = nameof(UserRole.Admin))]
        public async Task<IActionResult> ApproveReview(int id)
        {
            try
            {
                var success = await _reviewService.ApproveReviewAsync(id);
                if (!success)
                    return NotFound(ApiResponseDto<string>.CreateError("Review not found or already approved."));

                return Ok(ApiResponseDto<string>.CreateSuccess(null, "Review approved successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponseDto<string>.CreateError("An error occurred while approving the review."));
            }
        }
    }
}
