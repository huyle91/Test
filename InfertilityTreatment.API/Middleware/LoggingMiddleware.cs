using System.Diagnostics;

namespace InfertilityTreatment.API.Middleware
{
    public class LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
    {
        private readonly RequestDelegate _next = next;
        private readonly ILogger<LoggingMiddleware> _logger = logger;

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            
            // Log request
            _logger.LogInformation("Incoming {Method} request to {Path} from {RemoteIpAddress}",
                context.Request.Method,
                context.Request.Path,
                context.Connection.RemoteIpAddress);

            try
            {
                await _next(context);
            }
            finally
            {
                stopwatch.Stop();
                
                // Log response
                _logger.LogInformation("Completed {Method} request to {Path} with status {StatusCode} in {ElapsedMilliseconds}ms",
                    context.Request.Method,
                    context.Request.Path,
                    context.Response.StatusCode,
                    stopwatch.ElapsedMilliseconds);

                // Log slow requests
                if (stopwatch.ElapsedMilliseconds > 1000)
                {
                    _logger.LogWarning("Slow request detected: {Method} {Path} took {ElapsedMilliseconds}ms",
                        context.Request.Method,
                        context.Request.Path,
                        stopwatch.ElapsedMilliseconds);
                }
            }
        }
    }
}
