using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace SponsorMyAthlete.Api;

/// <summary>
/// Services signal rule violations with standard exceptions; this turns them into ProblemDetails
/// whose `detail` the frontend shows to the user, instead of an opaque 500.
/// </summary>
public class ApiExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken ct)
    {
        var status = exception switch
        {
            InvalidOperationException => StatusCodes.Status400BadRequest,
            KeyNotFoundException => StatusCodes.Status404NotFound,
            UnauthorizedAccessException => StatusCodes.Status403Forbidden,
            _ => 0,
        };
        if (status == 0)
            return false;

        httpContext.Response.StatusCode = status;
        await httpContext.Response.WriteAsJsonAsync(new ProblemDetails { Status = status, Detail = exception.Message }, ct);
        return true;
    }
}
