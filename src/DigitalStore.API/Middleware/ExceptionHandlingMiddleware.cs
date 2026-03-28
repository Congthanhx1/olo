using DigitalStore.Application.Common.Exceptions;
using System.Text.Json;

namespace DigitalStore.API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext ctx)
    {
        try
        {
            await _next(ctx);
        }
        catch (NotFoundException ex)
        {
            await WriteError(ctx, 404, ex.Message);
        }
        catch (ForbiddenException ex)
        {
            await WriteError(ctx, 403, ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            await WriteError(ctx, 400, ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            await WriteError(ctx, 401, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            await WriteError(ctx, 500, "Đã xảy ra lỗi hệ thống. Vui lòng thử lại sau.");
        }
    }

    private static async Task WriteError(HttpContext ctx, int statusCode, string message)
    {
        ctx.Response.StatusCode = statusCode;
        ctx.Response.ContentType = "application/json";
        var body = JsonSerializer.Serialize(new { message });
        await ctx.Response.WriteAsync(body);
    }
}
