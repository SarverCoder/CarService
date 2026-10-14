using CarService.Application.Common.Exceptions;
using CarService.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace CarService.Api.Middlewares;

/// <summary>Xatolarni bir xil ProblemDetails javobiga aylantiradi.</summary>
public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            var problem = ex switch
            {
                ValidationException v => new ValidationProblemDetails(v.Errors) { Status = StatusCodes.Status400BadRequest, Title = v.Message },
                DomainException => new ProblemDetails { Status = StatusCodes.Status400BadRequest, Title = ex.Message },
                NotFoundException => new ProblemDetails { Status = StatusCodes.Status404NotFound, Title = ex.Message },
                UnauthorizedException => new ProblemDetails { Status = StatusCodes.Status401Unauthorized, Title = ex.Message },
                ConflictException => new ProblemDetails { Status = StatusCodes.Status409Conflict, Title = ex.Message },
                _ => new ProblemDetails { Status = StatusCodes.Status500InternalServerError, Title = "Serverda kutilmagan xatolik yuz berdi." }
            };

            if (problem.Status == StatusCodes.Status500InternalServerError)
                logger.LogError(ex, "Unhandled exception");

            problem.Instance = context.Request.Path;
            context.Response.StatusCode = problem.Status!.Value;
            await context.Response.WriteAsJsonAsync(problem, problem.GetType(), options: null, contentType: "application/problem+json");
        }
    }
}
