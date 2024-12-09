using Microsoft.AspNetCore.Http;
using System.Net;
using System.Text.Json;
using FluentValidation;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException validationEx)
        {
            await HandleValidationExceptionAsync(context, validationEx);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }


    private static Task HandleValidationExceptionAsync(HttpContext context, ValidationException validationEx)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

        var errors = validationEx.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(g => g.Key, g => g.Select(x => x.ErrorMessage).ToArray());

        var errorResponse = new
        {
          
            title = "Validation Errors",
            status = context.Response.StatusCode,
            errors = errors,
            message = "Please correct the errors and try again."
        };

        return context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
    }


    private static Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        var errorResponse = new
        {
            StatusCode = context.Response.StatusCode,
            Message = "Internal Server Error", // Customize your message
            Detail = ex.Message // Consider not exposing detail in production
        };

        return context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
    }
}
