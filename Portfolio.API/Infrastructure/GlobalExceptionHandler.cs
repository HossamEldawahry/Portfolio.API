using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Portfolio.API.Infrastructure;

public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly IHostEnvironment _environment;
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(IHostEnvironment environment, ILogger<GlobalExceptionHandler> logger)
    {
        _environment = environment;
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Unhandled exception");

        var (statusCode, title, detail) = exception switch
        {
            KeyNotFoundException e => (StatusCodes.Status404NotFound, "Not found", e.Message),
            ArgumentException e => (StatusCodes.Status400BadRequest, "Invalid request", e.Message),
            InvalidOperationException e => (StatusCodes.Status400BadRequest, "Invalid operation", e.Message),
            _ => (
                StatusCodes.Status500InternalServerError,
                "Server error",
                _environment.IsDevelopment() ? exception.Message : "An unexpected error occurred.")
        };

        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = "application/problem+json";

        var problem = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Type = $"https://httpstatuses.io/{statusCode}"
        };

        await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken).ConfigureAwait(false);
        return true;
    }
}
