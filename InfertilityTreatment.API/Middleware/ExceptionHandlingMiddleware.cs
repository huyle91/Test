using InfertilityTreatment.Entity.DTOs.Common;
using System.Net;
using System.Text.Json;

namespace InfertilityTreatment.API.Middleware
{
    public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        private readonly RequestDelegate _next = next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger = logger;
        
        // Cache JsonSerializerOptions to avoid creating new instance every time
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            
            var response = new ApiResponseDto<object>();

            switch (exception)
            {
                case UnauthorizedAccessException:
                    response.Success = false;
                    response.Message = exception.Message;
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    break;

                case KeyNotFoundException:
                    response.Success = false;
                    response.Message = "Resource not found";
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    break;

                case TimeoutException:
                    response.Success = false;
                    response.Message = "Request timeout";
                    context.Response.StatusCode = (int)HttpStatusCode.RequestTimeout;
                    break;

                case ArgumentNullException:
                case ArgumentException:
                case InvalidOperationException:
                    response.Success = false;
                    response.Message = exception.Message;
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    break;

                case NotSupportedException:
                    response.Success = false;
                    response.Message = "Operation not supported";
                    context.Response.StatusCode = (int)HttpStatusCode.NotImplemented;
                    break;

                default:
                    response.Success = false;
                    response.Message = "An internal server error occurred";
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    break;
            }

            response.Timestamp = DateTime.UtcNow;

            var jsonResponse = JsonSerializer.Serialize(response, JsonOptions);

            await context.Response.WriteAsync(jsonResponse);
        }
    }
}
