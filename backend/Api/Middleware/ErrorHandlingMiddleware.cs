using System.Text.Json;
using Api.Models.DTOs;

namespace Api.Middleware;

public class ErrorHandlingMiddleware
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);

            // Si ASP.NET Core devolvió 401 o 403 sin cuerpo (auth/roles), estandarizamos la respuesta.
            if (!context.Response.HasStarted &&
                (context.Response.StatusCode == StatusCodes.Status401Unauthorized ||
                 context.Response.StatusCode == StatusCodes.Status403Forbidden))
            {
                var mensaje = context.Response.StatusCode == StatusCodes.Status401Unauthorized
                    ? "Sesión no válida. Vuelve a iniciar sesión."
                    : "No tienes permiso para esta acción.";

                await WriteErrorAsync(context, context.Response.StatusCode, mensaje);
            }
        }
        catch (ApiException ex)
        {
            await WriteErrorAsync(context, ex.StatusCode, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado procesando {Method} {Path}",
                context.Request.Method, context.Request.Path);

            await WriteErrorAsync(context, StatusCodes.Status500InternalServerError,
                "Ocurrió un error interno.");
        }
    }

    private static async Task WriteErrorAsync(HttpContext context, int statusCode, string message)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        var payload = ApiResponse<object>.Fail(message);
        await context.Response.WriteAsync(JsonSerializer.Serialize(payload, JsonOptions));
    }
}
