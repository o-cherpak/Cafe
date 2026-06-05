using CafeApi.Exceptions;
using CafeApi.Exceptions.NotFoundExceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace CafeApi.Middleware;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Exception has occurred: {Message}", exception.Message);

        var problemDetails = exception switch
        {
            NotFoundException => new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Not Found",
                Detail = exception.Message,
                Type = "https://tools.ietf.org/html/rfc9110#section-15.5.5"
            },
            ConflictException => new ProblemDetails
            {
                Status = StatusCodes.Status409Conflict,
                Title = "Conflict",
                Detail = exception.Message,
                Type = "https://tools.ietf.org/html/rfc9110#section-15.5.10"
            },
            InsufficientBonusException => new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Insufficient Bonuses",
                Detail = exception.Message,
                Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1"
            },
            PromotionNotActiveException => new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Promotion Not Active",
                Detail = exception.Message,
                Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1"
            },
            UnauthorizedException => new ProblemDetails
            {
                Status = StatusCodes.Status401Unauthorized,
                Title = "Unauthorized",
                Detail = exception.Message,
                Type = "https://tools.ietf.org/html/rfc9110#section-15.5.2"
            },
            PermissionException => new ProblemDetails
            {
                Status = StatusCodes.Status403Forbidden,
                Title = "Forbidden",
                Detail = exception.Message,
                Type = "https://tools.ietf.org/html/rfc9110#section-15.5.4"
            },
            _ => new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Server Error",
                Detail = "An unexpected error occurred.",
                Type = "https://tools.ietf.org/html/rfc9110#section-15.6.1"
            }
        };

        httpContext.Response.StatusCode = problemDetails.Status.Value;

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}
