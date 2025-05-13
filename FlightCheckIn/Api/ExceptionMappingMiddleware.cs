using System.Text.Json;
using FlightCheckIn.Domain;
using Microsoft.AspNetCore.Mvc;

namespace FlightCheckIn.Api;

public sealed class ExceptionMappingMiddleware(RequestDelegate next)
{
    public async Task Invoke(HttpContext ctx)
    {
        try { await next(ctx); }
        catch (AppException ex)
        {
            var (status, title) = ex switch
            {
                NotFoundException => (StatusCodes.Status404NotFound, "Not found"),
                ConflictException => (StatusCodes.Status409Conflict, "Conflict"),
                ValidationException => (StatusCodes.Status422UnprocessableEntity, "Validation error"),
                _ => (StatusCodes.Status400BadRequest, "Domain error"),
            };

            var prob = new ProblemDetails
            {
                Status = status,
                Title = title,
                Detail = ex.Message,
                Instance = ctx.Request.Path,
            };
            ctx.Response.StatusCode = status;
            ctx.Response.ContentType = "application/problem+json";
            await ctx.Response.WriteAsync(JsonSerializer.Serialize(prob));
        }
    }
}
