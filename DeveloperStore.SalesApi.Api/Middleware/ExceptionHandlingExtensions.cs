using DeveloperStore.SalesApi.Application.Common.Exceptions;
using Microsoft.AspNetCore.Diagnostics;

namespace DeveloperStore.SalesApi.Api.Middleware;

public static class ExceptionHandlingExtensions
{
    public static IApplicationBuilder UseApiExceptionHandler(this IApplicationBuilder app)
    {
        app.UseExceptionHandler(handlerApp =>
        {
            handlerApp.Run(async context =>
            {
                var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
                var (statusCode, title) = exception switch
                {
                    NotFoundException => (StatusCodes.Status404NotFound, "Resource not found"),
                    BusinessRuleException => (StatusCodes.Status409Conflict, "Business rule violation"),
                    _ => (StatusCodes.Status500InternalServerError, "Unexpected error")
                };

                context.Response.StatusCode = statusCode;
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsJsonAsync(new ErrorResponse(title, exception?.Message ?? "Unexpected error"));
            });
        });

        return app;
    }

    private sealed record ErrorResponse(string Title, string Detail);
}
