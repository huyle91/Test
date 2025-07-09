using InfertilityTreatment.Business.Exceptions;
using InfertilityTreatment.Business.Interfaces;
using InfertilityTreatment.Entity.DTOs.Analytics;
using InfertilityTreatment.Entity.DTOs.Appointments;
using InfertilityTreatment.Entity.DTOs.Common;
using InfertilityTreatment.Entity.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

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
        /// Get dashboard statistics by role
        /// </summary>
        [HttpGet("dashboard/{role}")]
        [Authorize(Roles = "Admin,Manager,Doctor,Customer")]
        public async Task<IActionResult> GetDashboardStatsByRole(
            string role,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;

                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return BadRequest(new ApiResponseDto<object>
                    {
                        Success = false,
                        Message = "Invalid user ID",
                        Data = null
                    });
                }

                if (string.IsNullOrEmpty(roleClaim))
                {
                    return BadRequest(new ApiResponseDto<object>
                    {
                        Success = false,
                        Message = "Invalid user role",
                        Data = null
                    });
                }

                // Logic gộp: Check role của user hiện tại
                string targetRole;
                if (roleClaim == "Doctor" || roleClaim == "Customer")
                {
                    // Doctor/Customer chỉ được xem dashboard của chính họ
                    targetRole = roleClaim;
                }
                else if (roleClaim == "Admin" || roleClaim == "Manager")
                {
                    // Admin/Manager có thể xem dashboard của role được truyền vào
                    targetRole = role;
                }
                else
                {
                    return StatusCode(StatusCodes.Status403Forbidden, new ApiResponseDto<object>
                    {
                        Success = false,
                        Message = "Access denied",
                        Data = null
                    });
                }

                if (!Enum.TryParse<UserRole>(targetRole, true, out UserRole userRole))
                {
                    return BadRequest(new ApiResponseDto<object>
                    {
                        Success = false,
                        Message = "Invalid role specified",
                        Data = null
                    });
                }

                var defaultStartDate = startDate ?? DateTime.Now.AddDays(-30);
                var defaultEndDate = endDate ?? DateTime.Now;

                var dateRange = new DateRangeDto
                {
                    StartDate = defaultStartDate,
                    EndDate = defaultEndDate
                };

                // For Admin/Manager querying other roles, pass null userId to get all data
                // For Doctor/Customer, pass their own userId to get filtered data
                int? serviceUserId = null;
                if (roleClaim == "Doctor" || roleClaim == "Customer")
                {
                    serviceUserId = userId;
                }
                // For Admin/Manager, serviceUserId remains null to get all data for the target role

                var stats = await _analyticsService.GetDashboardStatsAsync(userRole, serviceUserId, dateRange);

                return Ok(new ApiResponseDto<DashboardStatsDto>
                {
                    Success = true,
                    Message = "Dashboard statistics retrieved successfully",
                    Data = stats
                });
            }
            catch (NotFoundException ex)
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
        /// Get revenue reports with filtering
        /// </summary>
        [HttpGet("revenue-reports")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetRevenueReports(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] int? doctorId = null,
            [FromQuery] int? serviceId = null,
            [FromQuery] string groupBy = "month")
        {
            try
            {
                var filter = new RevenueFilterDto
                {
                    StartDate = startDate ?? DateTime.Now.AddDays(-30),
                    EndDate = endDate ?? DateTime.Now,
                    DoctorId = doctorId,
                    ServiceId = serviceId,
                    GroupBy = groupBy
                };

                var report = await _analyticsService.GetRevenueReportAsync(filter);

                return Ok(new ApiResponseDto<RevenueReportDto>
                {
                    Success = true,
                    Message = "Revenue report retrieved successfully",
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
        /// Get patient demographics
        /// </summary>
        [HttpGet("patient-demographics")]
        [Authorize(Roles = "Admin,Manager,Doctor")]
        public async Task<IActionResult> GetPatientDemographics(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            try
            {
                var dateRange = new DateRangeDto
                {
                    StartDate = startDate ?? DateTime.Now.AddYears(-1),
                    EndDate = endDate ?? DateTime.Now
                };

                var demographics = await _analyticsService.GetPatientDemographicsAsync(dateRange);

                return Ok(new ApiResponseDto<PatientDemographicsDto>
                {
                    Success = true,
                    Message = "Patient demographics retrieved successfully",
                    Data = demographics
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponseDto<string>.CreateError(ex.Message));
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
        /// Export analytics report
        /// </summary>
        [HttpPost("export-report")]
        [Authorize(Roles = "Admin,Manager,Doctor")]
        public async Task<IActionResult> ExportReport([FromBody] ExportReportDto exportRequest)
        {
            try
            {
                if (exportRequest == null)
                {
                    return BadRequest(new ApiResponseDto<object>
                    {
                        Success = false,
                        Message = "Export request is required",
                        Data = null
                    });
                }

                byte[] fileData;
                string contentType;
                string fileName;

                if (exportRequest.ExportFormat.ToLower() == "pdf")
                {
                    fileData = await _analyticsService.ExportReportToPdfAsync(exportRequest);
                    contentType = "application/pdf";
                    fileName = $"{exportRequest.ReportType}_report_{DateTime.Now:yyyyMMdd}.pdf";
                }
                else if (exportRequest.ExportFormat.ToLower() == "excel")
                {
                    fileData = await _analyticsService.ExportReportToExcelAsync(exportRequest);
                    contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    fileName = $"{exportRequest.ReportType}_report_{DateTime.Now:yyyyMMdd}.xlsx";
                }
                else
                {
                    return BadRequest(new ApiResponseDto<object>
                    {
                        Success = false,
                        Message = "Invalid export format. Supported formats: pdf, excel",
                        Data = null
                    });
                }

                return File(fileData, contentType, fileName);
            }
            catch (NotImplementedException)
            {
                return BadRequest(new ApiResponseDto<object>
                {
                    Success = false,
                    Message = "Export functionality is not yet implemented",
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
        /// Get treatment success rates with filtering
        /// </summary>
        [HttpGet("treatment-success-rates")]
        [Authorize(Roles = "Admin,Manager,Doctor")]
        public async Task<ActionResult<ApiResponseDto<PaginatedResultDto<TreatmentSuccessRateDto>>>> GetTreatmentSuccessRates(   
            [FromQuery] PaginationQueryDTO pagination,
            [FromQuery] int? serviceId = null,
            [FromQuery] int? doctorId = null
            )
        {
            try
            {
                pagination.PageNumber = pagination.PageNumber <= 0 ? 1 : pagination.PageNumber;
                pagination.PageSize = pagination.PageSize <= 0 ? 100 : pagination.PageSize;
                if (pagination.PageSize > 100) pagination.PageSize = 100;
                if (pagination.PageNumber < 1) pagination.PageNumber = 1;
                var filter = new SuccessRateFilterDto
                {
                    ServiceId = serviceId,
                    DoctorId = doctorId,
                };

                var successRates = await _analyticsService.GetTreatmentSuccessRatesAsync(filter, pagination);
                return Ok(ApiResponseDto<PaginatedResultDto<TreatmentSuccessRateDto>>.CreateSuccess(successRates, "Get Treatment Success Rates successfully."));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ApiResponseDto<string>.CreateError(ex.Message));
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
        /// Get doctor performance by ID
        /// </summary>
        [HttpGet("doctor-performance/{doctorId}")]
        [Authorize(Roles = "Admin,Manager,Doctor")]
        public async Task<IActionResult> GetDoctorPerformance(int doctorId)
        {
            try
            {
                var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

                // Doctor chỉ được xem performance của chính họ
                if (currentUserRole == "Doctor")
                {
                    // Cần check xem currentUserId có phải là doctor với doctorId này không

                    if (!int.TryParse(currentUserId, out int userId) || ! await _analyticsService.CheckIsDoctorIdWithUserId(userId, doctorId))
                    {
                        return StatusCode(StatusCodes.Status403Forbidden, new ApiResponseDto<object>
                        {
                            Success = false,
                            Message = "You can only view your own performance data",
                            Data = null
                        });
                    }

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

        [HttpGet("treatment-outcomes")]
        [Authorize(Roles = "Admin,Manager,Doctor")]
        public async Task<IActionResult> GetTreatmentOutcomes([FromQuery] OutcomeAnalysisDto filters)
        {
            try
            {
                if (filters == null)
                {
                    return BadRequest(new ApiResponseDto<object>
                    {
                        Success = false,
                        Message = "Filters parameter is required",
                        Data = null
                    });
                }

                var result = await _analyticsService.GetTreatmentOutcomesAsync(filters);
                return Ok(new ApiResponseDto<PaginatedResultDto<OutcomeAnalysisResultDto>>
                {
                    Success = true,
                    Message = "Treatment outcomes analytics retrieved successfully",
                    Data = result
                });
            }
            catch (NotFoundException ex)
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

        [HttpGet("efficiency-metrics")]
        [Authorize(Roles = "Admin,Manager,Doctor")]
        public async Task<IActionResult> GetEfficiencyMetrics([FromQuery] EfficiencyQueryDto query)
        {
            try
            {
                if (query == null)
                {
                    return BadRequest(new ApiResponseDto<object>
                    {
                        Success = false,
                        Message = "Query parameter is required",
                        Data = null
                    });
                }

                var result = await _analyticsService.GetEfficiencyMetricsAsync(query);
                return Ok(new ApiResponseDto<EfficiencyMetrics>
                {
                    Success = true,
                    Message = "Efficiency metrics retrieved successfully",
                    Data = result
                });
            }
            catch (NotFoundException ex)
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

        [HttpGet("patient-journey")]
        [Authorize(Roles = "Admin,Manager,Doctor")]
        public async Task<IActionResult> GetPatientJourneyAnalytics([FromQuery] PatientJourneyDto filters)
        {
            try
            {
                if (filters == null)
                {
                    return BadRequest(new ApiResponseDto<object>
                    {
                        Success = false,
                        Message = "Filters parameter is required",
                        Data = null
                    });
                }

                var result = await _analyticsService.GetPatientJourneyAnalyticsAsync(filters);
                return Ok(new ApiResponseDto<PatientJourneyResultDto>
                {
                    Success = true,
                    Message = "Patient journey analytics retrieved successfully",
                    Data = result
                });
            }
            catch (NotFoundException ex)
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

        [HttpGet("predictive-analytics")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetPredictiveAnalytics([FromQuery] PredictiveQueryDto query)
        {
            try
            {
                if (query == null)
                {
                    return BadRequest(new ApiResponseDto<object>
                    {
                        Success = false,
                        Message = "Query parameter is required",
                        Data = null
                    });
                }

                var result = await _analyticsService.GetPredictiveAnalyticsAsync(query);
                return Ok(new ApiResponseDto<PredictiveAnalyticsResultDto>
                {
                    Success = true,
                    Message = "Predictive analytics retrieved successfully",
                    Data = result
                });
            }
            catch (NotFoundException ex)
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

        [HttpPost("custom-report")]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<IActionResult> GenerateCustomReport([FromBody] CustomReportDto dto)
        {
            try
            {
                if (dto == null)
                {
                    return BadRequest(new ApiResponseDto<object>
                    {
                        Success = false,
                        Message = "Request body is required",
                        Data = null
                    });
                }

                var result = await _analyticsService.GenerateCustomReportAsync(dto);
                return Ok(new ApiResponseDto<CustomReportResultDto>
                {
                    Success = true,
                    Message = "Custom report generated successfully",
                    Data = result
                });
            }
            catch (NotFoundException ex)
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

        [HttpGet("treatment-outcomes/export")]
        [Authorize(Roles = "Admin,Manager,Doctor")]
        public async Task<IActionResult> ExportTreatmentOutcomes([FromQuery] OutcomeAnalysisDto filters, [FromQuery] string format = "csv")
        {
            try
            {
                var fileBytes = await _analyticsService.ExportTreatmentOutcomesAsync(filters, format);
                var contentType = "text/csv";
                var fileName = $"treatment-outcomes-{DateTime.Now:yyyyMMdd-HHmmss}.{format}";
                return File(fileBytes, contentType, fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error exporting Treatment Outcomes: {ex.Message}");
            }
        }

        [HttpGet("efficiency-metrics/export")]
        [Authorize(Roles = "Admin,Manager,Doctor")]
        public async Task<IActionResult> ExportEfficiencyMetrics([FromQuery] EfficiencyQueryDto query, [FromQuery] string format = "csv")
        {
            try
            {
                var fileBytes = await _analyticsService.ExportEfficiencyMetricsAsync(query, format);
                var contentType = "text/csv";
                var fileName = $"efficiency-metrics-{DateTime.Now:yyyyMMdd-HHmmss}.{format}";
                return File(fileBytes, contentType, fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error exporting Efficiency Metrics: {ex.Message}");
            }
        }

        [HttpGet("patient-journey/export")]
        [Authorize(Roles = "Admin,Manager,Doctor")]
        public async Task<IActionResult> ExportPatientJourney([FromQuery] PatientJourneyDto filters, [FromQuery] string format = "csv")
        {
            try
            {
                var fileBytes = await _analyticsService.ExportPatientJourneyAsync(filters, format);
                var contentType = "text/csv";
                var fileName = $"patient-journey-{DateTime.Now:yyyyMMdd-HHmmss}.{format}";
                return File(fileBytes, contentType, fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error exporting Patient Journey: {ex.Message}");
            }
        }

        [HttpGet("predictive-analytics/export")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> ExportPredictiveAnalytics([FromQuery] PredictiveQueryDto query, [FromQuery] string format = "csv")
        {
            try
            {
                var fileBytes = await _analyticsService.ExportPredictiveAnalyticsAsync(query, format);
                var contentType = "text/csv";
                var fileName = $"predictive-analytics-{DateTime.Now:yyyyMMdd-HHmmss}.{format}";
                return File(fileBytes, contentType, fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error exporting Predictive Analytics: {ex.Message}");
            }
        }

        [HttpPost("custom-report/export")]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<IActionResult> ExportCustomReport([FromBody] CustomReportDto dto, [FromQuery] string format = "csv")
        {
            try
            {
                var fileBytes = await _analyticsService.ExportCustomReportAsync(dto, format);
                var contentType = "text/csv";
                var fileName = $"custom-report-{DateTime.Now:yyyyMMdd-HHmmss}.{format}";
                return File(fileBytes, contentType, fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error exporting Custom Report: {ex.Message}");
            }
        }
    }
}
