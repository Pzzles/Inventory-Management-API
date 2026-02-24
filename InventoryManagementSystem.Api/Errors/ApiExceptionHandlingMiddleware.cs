using System.Text.Json;

namespace InventoryManagementSystem.Api.Errors;

public sealed class ApiExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ApiExceptionHandlingMiddleware> _logger;

    public ApiExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ApiExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Unhandled exception for {Path}", context.Request.Path);

            if (context.Response.HasStarted)
            {
                throw;
            }

            var apiError = new ApiError
            {
                Message = "An unexpected error occurred.",
                CorrelationId = context.TraceIdentifier
            };

            context.Response.Clear();
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";

            var payload = JsonSerializer.Serialize(apiError);
            await context.Response.WriteAsync(payload);
        }
    }
}
