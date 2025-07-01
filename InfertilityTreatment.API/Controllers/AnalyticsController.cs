using InfertilityTreatment.Business.Interfaces;
using InfertilityTreatment.Entity.DTOs.Analytics;
using InfertilityTreatment.Entity.DTOs.Common;
using InfertilityTreatment.Entity.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace InfertilityTreatment.API.Controllers
{
    [Route("api/analytics")]
    [ApiController]
    [Authorize]
    public class AnalyticsController : ControllerBase
    {
        private readonly IAnalyticsService _analyticsService;

        public AnalyticsController(IAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
        }

        /// <summary>
        /// Get dashboard statistics for the current user
        /// </summary>
        /// <param name="startDate">Start date for filtering (optional, defaults to 30 days ago)</param>
        /// <param name="endDate">End date for filtering (optional, defaults to today)</param>
        /// <returns>Dashboard statistics</returns>
        [HttpGet("dashboard")]
        [Authorize(Roles = "Admin,Manager,Doctor")]
        public async Task<IActionResult> GetDashboardStats(
            [FromQuery] DateTime? startDate = null, 
            [FromQuery] DateTime? endDate = null)
        {
            try
            {
                // Get current user info from JWT token
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;

                if (string.IsNullOrEmpty(userIdClaim) || string.IsNullOrEmpty(roleClaim))
                {
                    return Unauthorized("Invalid token claims");
                }

                if (!int.TryParse(userIdClaim, out int userId))
                {
                    return BadRequest("Invalid user ID");
                }

                if (!Enum.TryParse<UserRole>(roleClaim, out UserRole userRole))
                {
                    return BadRequest("Invalid user role");
                }

                // Set default date range if not provided
                var dateRange = new DateRangeDto
                {
                    StartDate = startDate ?? DateTime.Now.AddDays(-30),
                    EndDate = endDate ?? DateTime.Now
                };

                var stats = await _analyticsService.GetDashboardStatsAsync(userRole, userId, dateRange);

                return Ok(new ApiResponseDto<DashboardStatsDto>
                {
                    Success = true,
                    Message = "Dashboard statistics retrieved successfully",
                    Data = stats
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponseDto<object>
                {
                    Success = false,
                    Message = $"An error occurred: {ex.Message}",
                    Data = null
                });
            }
        }

        /// <summary>
        /// Get treatment success rates across all services
        /// </summary>
        /// <returns>List of treatment success rates by service type</returns>
        [HttpGet("treatment-success-rates")]
        [Authorize(Roles = "Admin,Manager,Doctor")]
        public async Task<IActionResult> GetTreatmentSuccessRates()
        {
            try
            {
                var successRates = await _analyticsService.GetTreatmentSuccessRatesAsync();

                return Ok(new ApiResponseDto<object>
                {
                    Success = true,
                    Message = "Treatment success rates retrieved successfully",
                    Data = successRates
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponseDto<object>
                {
                    Success = false,
                    Message = $"An error occurred: {ex.Message}",
                    Data = null
                });
            }
        }

        /// <summary>
        /// Get revenue report with breakdown by time and service
        /// </summary>
        /// <param name="filter">Revenue filter parameters</param>
        /// <returns>Revenue report with monthly and service breakdowns</returns>
        [HttpPost("revenue-report")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetRevenueReport([FromBody] RevenueFilterDto filter)
        {
            try
            {
                if (filter == null)
                {
                    return BadRequest("Filter parameters are required");
                }

                // Validate date range
                if (filter.StartDate >= filter.EndDate)
                {
                    return BadRequest("Start date must be before end date");
                }

                // Limit date range to prevent performance issues (max 2 years)
                if ((filter.EndDate - filter.StartDate).TotalDays > 730)
                {
                    return BadRequest("Date range cannot exceed 2 years");
                }

                var report = await _analyticsService.GetRevenueReportAsync(filter);

                return Ok(new ApiResponseDto<RevenueReportDto>
                {
                    Success = true,
                    Message = "Revenue report generated successfully",
                    Data = report
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponseDto<object>
                {
                    Success = false,
                    Message = $"An error occurred: {ex.Message}",
                    Data = null
                });
            }
        }

        /// <summary>
        /// Get performance metrics for a specific doctor
        /// </summary>
        /// <param name="doctorId">Doctor ID</param>
        /// <returns>Doctor performance metrics</returns>
        [HttpGet("doctor-performance/{doctorId}")]
        [Authorize(Roles = "Admin,Manager,Doctor")]
        public async Task<IActionResult> GetDoctorPerformance(int doctorId)
        {
            try
            {
                // Check if current user is the doctor or has admin/manager role
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;

                if (string.IsNullOrEmpty(userIdClaim) || string.IsNullOrEmpty(roleClaim))
                {
                    return Unauthorized("Invalid token claims");
                }

                if (!int.TryParse(userIdClaim, out int currentUserId))
                {
                    return BadRequest("Invalid user ID");
                }

                if (!Enum.TryParse<UserRole>(roleClaim, out UserRole userRole))
                {
                    return BadRequest("Invalid user role");
                }

                // Check authorization: doctor can only view their own performance
                if (userRole == UserRole.Doctor)
                {
                    // For doctors, we need to check if the doctorId matches their doctor record
                    // This would require additional service call to get doctor by userId
                    // For now, we'll allow access but this should be implemented properly
                }

                var performance = await _analyticsService.GetDoctorPerformanceAsync(doctorId);

                return Ok(new ApiResponseDto<DoctorPerformanceDto>
                {
                    Success = true,
                    Message = "Doctor performance retrieved successfully",
                    Data = performance
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponseDto<object>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponseDto<object>
                {
                    Success = false,
                    Message = $"An error occurred: {ex.Message}",
                    Data = null
                });
            }
        }

        /// <summary>
        /// Get simplified dashboard stats for quick overview
        /// </summary>
        /// <returns>Quick stats overview</returns>
        [HttpGet("quick-stats")]
        public async Task<IActionResult> GetQuickStats()
        {
            try
            {
                // Get current user info
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;

                if (string.IsNullOrEmpty(userIdClaim) || string.IsNullOrEmpty(roleClaim))
                {
                    return Unauthorized("Invalid token claims");
                }

                if (!int.TryParse(userIdClaim, out int userId))
                {
                    return BadRequest("Invalid user ID");
                }

                if (!Enum.TryParse<UserRole>(roleClaim, out UserRole userRole))
                {
                    return BadRequest("Invalid user role");
                }

                // Get last 7 days stats
                var dateRange = new DateRangeDto
                {
                    StartDate = DateTime.Now.AddDays(-7),
                    EndDate = DateTime.Now
                };

                var stats = await _analyticsService.GetDashboardStatsAsync(userRole, userId, dateRange);

                // Return simplified response
                var quickStats = new
                {
                    TotalPatients = stats.TotalPatients,
                    TotalAppointments = stats.TotalAppointments,
                    ActiveTreatments = stats.ActiveTreatments,
                    SuccessRate = Math.Round(stats.SuccessRate, 2),
                    Period = "Last 7 days"
                };

                return Ok(new ApiResponseDto<object>
                {
                    Success = true,
                    Message = "Quick statistics retrieved successfully",
                    Data = quickStats
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponseDto<object>
                {
                    Success = false,
                    Message = $"An error occurred: {ex.Message}",
                    Data = null
                });
            }
        }
    }
}
