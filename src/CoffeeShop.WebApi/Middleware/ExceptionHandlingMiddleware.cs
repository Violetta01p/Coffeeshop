using System.Net;
using System.Text.Json;
using CoffeeShop.Domain.Common.Exceptions;

namespace CoffeeShop.WebApi.Middleware;

/// <summary>
/// Глобальна обробка винятків.
/// DomainException → 400 Bad Request.
/// KeyNotFoundException → 404 Not Found.
/// Усе інше → 500 Internal Server Error.
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, title) = exception switch
        {
            DomainException => ((int)HttpStatusCode.BadRequest, "Помилка бізнес-правила"),
            KeyNotFoundException => ((int)HttpStatusCode.NotFound, "Ресурс не знайдено"),
            _ => ((int)HttpStatusCode.InternalServerError, "Внутрішня помилка сервера")
        };

        if (statusCode == (int)HttpStatusCode.InternalServerError)
            _logger.LogError(exception, "Необроблена помилка");
        else
            _logger.LogWarning(exception, "Оброблена помилка: {Message}", exception.Message);

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = statusCode;

        var problem = new
        {
            status = statusCode,
            title,
            detail = exception.Message
        };

        var json = JsonSerializer.Serialize(problem, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }
}
