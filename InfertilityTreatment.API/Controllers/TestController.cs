using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using InfertilityTreatment.Entity.DTOs.Common;

namespace InfertilityTreatment.API.Controllers
{
    [ApiController]
    [Route("api/test")]
    public class TestController(ILogger<TestController> logger) : ControllerBase
    {
        private readonly ILogger<TestController> _logger = logger;

        /// <summary>
        /// Public endpoint to test API response format
        /// </summary>
        [HttpGet("public")]
        public ActionResult<ApiResponseDto<object>> GetPublic()
        {
            var data = new
            {
                message = "This is a public endpoint",
                timestamp = DateTime.UtcNow,
                environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
            };

            return Ok(ApiResponseDto<object>.CreateSuccess(data, "Public endpoint accessed successfully"));
        }

        /// <summary>
        /// Protected endpoint for authenticated users
        /// </summary>
        [HttpGet("protected")]
        [Authorize]
        public ActionResult<ApiResponseDto<object>> GetProtected()
        {
            var data = new
            {
                message = "This is a protected endpoint",
                user = User.Identity?.Name,
                timestamp = DateTime.UtcNow,
                claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList()
            };

            return Ok(ApiResponseDto<object>.CreateSuccess(data, "Protected endpoint accessed successfully"));
        }

        /// <summary>
        /// Customer-only endpoint
        /// </summary>
        [HttpGet("customer")]
        [Authorize(Policy = "CustomerOnly")]
        public ActionResult<ApiResponseDto<object>> GetCustomerOnly()
        {
            var data = new
            {
                message = "This endpoint is for customers only",
                user = User.Identity?.Name,
                role = User.FindFirst("http://schemas.microsoft.com/ws/2008/06/identity/claims/role")?.Value
            };

            return Ok(ApiResponseDto<object>.CreateSuccess(data, "Customer-only endpoint accessed successfully"));
        }

        /// <summary>
        /// Doctor-only endpoint
        /// </summary>
        [HttpGet("doctor")]
        [Authorize(Policy = "DoctorOnly")]
        public ActionResult<ApiResponseDto<object>> GetDoctorOnly()
        {
            var data = new
            {
                message = "This endpoint is for doctors only",
                user = User.Identity?.Name,
                role = User.FindFirst("http://schemas.microsoft.com/ws/2008/06/identity/claims/role")?.Value
            };

            return Ok(ApiResponseDto<object>.CreateSuccess(data, "Doctor-only endpoint accessed successfully"));
        }

        /// <summary>
        /// Admin-only endpoint
        /// </summary>
        [HttpGet("admin")]
        [Authorize(Policy = "AdminOnly")]
        public ActionResult<ApiResponseDto<object>> GetAdminOnly()
        {
            var data = new
            {
                message = "This endpoint is for admins/managers only",
                user = User.Identity?.Name,
                role = User.FindFirst("http://schemas.microsoft.com/ws/2008/06/identity/claims/role")?.Value
            };

            return Ok(ApiResponseDto<object>.CreateSuccess(data, "Admin-only endpoint accessed successfully"));
        }

        /// <summary>
        /// Test endpoint that throws an exception to verify error handling
        /// </summary>
        [HttpGet("error")]
        public ActionResult<ApiResponseDto<object>> GetError()
        {
            _logger.LogInformation("Testing error handling middleware");
            throw new InvalidOperationException("This is a test exception to verify middleware error handling");
        }

        /// <summary>
        /// Test endpoint that simulates slow response
        /// </summary>
        [HttpGet("slow")]
        public async Task<ActionResult<ApiResponseDto<object>>> GetSlow()
        {
            _logger.LogInformation("Testing slow request logging");
            
            // Simulate slow operation
            await Task.Delay(1500);
            
            return Ok(ApiResponseDto<object>.CreateSuccess(
                new { message = "This was a slow request" }, 
                "Slow endpoint completed"));
        }
    }
}
