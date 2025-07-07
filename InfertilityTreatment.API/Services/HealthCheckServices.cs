using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using InfertilityTreatment.Data.Context;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace InfertilityTreatment.API.Services
{
    public class DatabaseHealthCheck : IHealthCheck
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DatabaseHealthCheck> _logger;

        public DatabaseHealthCheck(ApplicationDbContext context, ILogger<DatabaseHealthCheck> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                var stopwatch = Stopwatch.StartNew();
                
                // Simple database connectivity test
                await _context.Database.CanConnectAsync(cancellationToken);
                
                // Test query performance
                var userCount = await _context.Users.CountAsync(cancellationToken);
                
                stopwatch.Stop();
                
                var responseTime = stopwatch.ElapsedMilliseconds;
                
                var data = new Dictionary<string, object>
                {
                    { "ResponseTime", $"{responseTime}ms" },
                    { "UserCount", userCount },
                    { "Database", _context.Database.GetDbConnection().Database }
                };

                if (responseTime > 5000) // 5 seconds
                {
                    return HealthCheckResult.Degraded("Database response time is slow", null, data);
                }

                return HealthCheckResult.Healthy("Database is healthy", data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database health check failed");
                return HealthCheckResult.Unhealthy("Database is unhealthy", ex);
            }
        }
    }

    public class ExternalServiceHealthCheck : IHealthCheck
    {
        private readonly ILogger<ExternalServiceHealthCheck> _logger;
        private readonly HttpClient _httpClient;

        public ExternalServiceHealthCheck(ILogger<ExternalServiceHealthCheck> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient();
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                var checks = new List<(string Service, bool IsHealthy, string? Error)>();

                // Check VNPay (mock)
                try
                {
                    var vnpayHealthy = await CheckVNPayAsync(cancellationToken);
                    checks.Add(("VNPay", vnpayHealthy, null));
                }
                catch (Exception ex)
                {
                    checks.Add(("VNPay", false, ex.Message));
                }

                // Check Momo (mock)
                try
                {
                    var momoHealthy = await CheckMomoAsync(cancellationToken);
                    checks.Add(("Momo", momoHealthy, null));
                }
                catch (Exception ex)
                {
                    checks.Add(("Momo", false, ex.Message));
                }

                var data = checks.ToDictionary(
                    c => c.Service,
                    c => (object)new { IsHealthy = c.IsHealthy, Error = c.Error }
                );

                var unhealthyServices = checks.Where(c => !c.IsHealthy).ToList();

                if (unhealthyServices.Count == checks.Count)
                {
                    return HealthCheckResult.Unhealthy("All external services are down", null, data);
                }
                else if (unhealthyServices.Any())
                {
                    return HealthCheckResult.Degraded($"Some external services are down: {string.Join(", ", unhealthyServices.Select(s => s.Service))}", null, data);
                }

                return HealthCheckResult.Healthy("All external services are healthy", data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "External service health check failed");
                return HealthCheckResult.Unhealthy("External service health check failed", ex);
            }
        }

        private async Task<bool> CheckVNPayAsync(CancellationToken cancellationToken)
        {
            // Mock implementation - replace with actual VNPay health check
            await Task.Delay(100, cancellationToken);
            return true; // Assume healthy for now
        }

        private async Task<bool> CheckMomoAsync(CancellationToken cancellationToken)
        {
            // Mock implementation - replace with actual Momo health check
            await Task.Delay(100, cancellationToken);
            return true; // Assume healthy for now
        }
    }

    public class SystemResourceHealthCheck : IHealthCheck
    {
        private readonly ILogger<SystemResourceHealthCheck> _logger;

        public SystemResourceHealthCheck(ILogger<SystemResourceHealthCheck> logger)
        {
            _logger = logger;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                var process = Process.GetCurrentProcess();
                
                var memoryUsage = process.WorkingSet64;
                var memoryUsageMB = memoryUsage / 1024 / 1024;
                
                var cpuUsage = GetCpuUsage();
                
                var data = new Dictionary<string, object>
                {
                    { "MemoryUsageMB", memoryUsageMB },
                    { "CpuUsagePercent", cpuUsage },
                    { "ThreadCount", process.Threads.Count },
                    { "HandleCount", process.HandleCount }
                };

                var warnings = new List<string>();

                if (memoryUsageMB > 1000) // 1GB
                {
                    warnings.Add($"High memory usage: {memoryUsageMB}MB");
                }

                if (cpuUsage > 80)
                {
                    warnings.Add($"High CPU usage: {cpuUsage}%");
                }

                if (warnings.Any())
                {
                    return HealthCheckResult.Degraded($"Resource warnings: {string.Join(", ", warnings)}", null, data);
                }

                return HealthCheckResult.Healthy("System resources are healthy", data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "System resource health check failed");
                return HealthCheckResult.Unhealthy("System resource health check failed", ex);
            }
        }

        private double GetCpuUsage()
        {
            // Simplified CPU usage calculation
            // In production, you might want to use more sophisticated methods
            return Random.Shared.NextDouble() * 100; // Mock value
        }
    }
}
