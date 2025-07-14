using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
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
        catch (ArgumentException ex)
        {
            await HandleExceptionAsync(context, 400, ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            await HandleExceptionAsync(context, 400, ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            await HandleExceptionAsync(context, 404, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado.");
            await HandleExceptionAsync(context, 500, "Erro interno do servidor.");
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, int statusCode, string message)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json; charset=utf-8";

        var errorObj = new { error = message };
        var json = JsonSerializer.Serialize(errorObj);

        await context.Response.WriteAsync(json);
    }
}
