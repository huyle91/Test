using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using InfertilityTreatment.Entity.DTOs.Common;
using InfertilityTreatment.Business.Interfaces;

namespace InfertilityTreatment.API.Controllers
{
    [Route("api/health")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        private readonly HealthCheckService _healthCheckService;
        private readonly ILogger<HealthController> _logger;

        public HealthController(HealthCheckService healthCheckService, ILogger<HealthController> logger)
        {
            _healthCheckService = healthCheckService;
            _logger = logger;
        }

        /// <summary>
        /// Basic health check endpoint
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Get()
        {
            var report = await _healthCheckService.CheckHealthAsync();
            
            var response = new
            {
                Status = report.Status.ToString(),
                TotalDuration = report.TotalDuration.TotalMilliseconds,
                Entries = report.Entries.Select(e => new
                {
                    Name = e.Key,
                    Status = e.Value.Status.ToString(),
                    Duration = e.Value.Duration.TotalMilliseconds,
                    Description = e.Value.Description,
                    Data = e.Value.Data
                })
            };

            var statusCode = report.Status switch
            {
                HealthStatus.Healthy => StatusCodes.Status200OK,
                HealthStatus.Degraded => StatusCodes.Status200OK,
                HealthStatus.Unhealthy => StatusCodes.Status503ServiceUnavailable,
                _ => StatusCodes.Status503ServiceUnavailable
            };

            return StatusCode(statusCode, new ApiResponseDto<object>
            {
                Success = report.Status == HealthStatus.Healthy,
                Message = $"Health status: {report.Status}",
                Data = response
            });
        }

        /// <summary>
        /// Database health check
        /// </summary>
        [HttpGet("database")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Database()
        {
            try
            {
                var report = await _healthCheckService.CheckHealthAsync(healthCheck => healthCheck.Name == "database");
                
                var dbCheck = report.Entries.FirstOrDefault();
                
                if (dbCheck.Key == null)
                {
                    return NotFound(new ApiResponseDto<object>
                    {
                        Success = false,
                        Message = "Database health check not found",
                        Data = null
                    });
                }

                var response = new
                {
                    Status = dbCheck.Value.Status.ToString(),
                    Duration = dbCheck.Value.Duration.TotalMilliseconds,
                    Description = dbCheck.Value.Description,
                    Data = dbCheck.Value.Data
                };

                return Ok(new ApiResponseDto<object>
                {
                    Success = dbCheck.Value.Status == HealthStatus.Healthy,
                    Message = $"Database health: {dbCheck.Value.Status}",
                    Data = response
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking database health");
                return StatusCode(500, new ApiResponseDto<object>
                {
                    Success = false,
                    Message = "Error checking database health",
                    Data = null
                });
            }
        }

        /// <summary>
        /// External services health check
        /// </summary>
        [HttpGet("external-services")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> ExternalServices()
        {
            try
            {
                var report = await _healthCheckService.CheckHealthAsync(healthCheck => healthCheck.Name == "external-services");
                
                var serviceCheck = report.Entries.FirstOrDefault();
                
                if (serviceCheck.Key == null)
                {
                    return NotFound(new ApiResponseDto<object>
                    {
                        Success = false,
                        Message = "External services health check not found",
                        Data = null
                    });
                }

                var response = new
                {
                    Status = serviceCheck.Value.Status.ToString(),
                    Duration = serviceCheck.Value.Duration.TotalMilliseconds,
                    Description = serviceCheck.Value.Description,
                    Data = serviceCheck.Value.Data
                };

                return Ok(new ApiResponseDto<object>
                {
                    Success = serviceCheck.Value.Status == HealthStatus.Healthy,
                    Message = $"External services health: {serviceCheck.Value.Status}",
                    Data = response
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking external services health");
                return StatusCode(500, new ApiResponseDto<object>
                {
                    Success = false,
                    Message = "Error checking external services health",
                    Data = null
                });
            }
        }

        /// <summary>
        /// System resources health check
        /// </summary>
        [HttpGet("system-resources")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> SystemResources()
        {
            try
            {
                var report = await _healthCheckService.CheckHealthAsync(healthCheck => healthCheck.Name == "system-resources");
                
                var resourceCheck = report.Entries.FirstOrDefault();
                
                if (resourceCheck.Key == null)
                {
                    return NotFound(new ApiResponseDto<object>
                    {
                        Success = false,
                        Message = "System resources health check not found",
                        Data = null
                    });
                }

                var response = new
                {
                    Status = resourceCheck.Value.Status.ToString(),
                    Duration = resourceCheck.Value.Duration.TotalMilliseconds,
                    Description = resourceCheck.Value.Description,
                    Data = resourceCheck.Value.Data
                };

                return Ok(new ApiResponseDto<object>
                {
                    Success = resourceCheck.Value.Status == HealthStatus.Healthy,
                    Message = $"System resources health: {resourceCheck.Value.Status}",
                    Data = response
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking system resources health");
                return StatusCode(500, new ApiResponseDto<object>
                {
                    Success = false,
                    Message = "Error checking system resources health",
                    Data = null
                });
            }
        }

        /// <summary>
        /// Get query performance statistics
        /// </summary>
        [HttpGet("query-stats")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> QueryStatistics()
        {
            try
            {
                var queryOptimizationService = HttpContext.RequestServices.GetRequiredService<IQueryOptimizationService>();
                var stats = await queryOptimizationService.GetQueryStatistics();

                return Ok(new ApiResponseDto<object>
                {
                    Success = true,
                    Message = "Query statistics retrieved successfully",
                    Data = stats
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting query statistics");
                return StatusCode(500, new ApiResponseDto<object>
                {
                    Success = false,
                    Message = "Error getting query statistics",
                    Data = null
                });
            }
        }
    }
}
