using API_GestionDeSalas_Jaume_Sere.Helpers;
using LogicaAplicacion.ServiceInterfaces;
using System.Net;
using System.Text.Json;

namespace API_GestionDeSalas_Jaume_Sere.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, IAuditService auditService)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex, auditService);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception, IAuditService auditService)
        {
            var userId = HttpContextUserHelper.GetUserId(context)?.ToString() ?? "Anonymous";
            var username = HttpContextUserHelper.GetUsername(context);
            var userRole = HttpContextUserHelper.GetUserRole(context);
            var requestPath = context.Request.Path.Value;
            var httpMethod = context.Request.Method;
            var ipAddress = HttpContextUserHelper.GetIpAddress(context);

            _logger.LogError(exception,
                "ERROR GLOBAL: Usuario {Username} ({UserId}, {UserRole}) en {HttpMethod} {RequestPath} desde {IpAddress} - {ExceptionMessage}",
                username, userId, userRole, httpMethod, requestPath, ipAddress, exception.Message);

            var (statusCode, message) = exception switch
            {
                KeyNotFoundException => (HttpStatusCode.NotFound, exception.Message),
                UnauthorizedAccessException => (HttpStatusCode.Unauthorized, "No autorizado."),
                InvalidOperationException => (HttpStatusCode.BadRequest, exception.Message),
                ArgumentException => (HttpStatusCode.BadRequest, exception.Message),
                NotSupportedException => (HttpStatusCode.BadRequest, exception.Message),
                _ => (HttpStatusCode.InternalServerError, "Error interno del servidor.")
            };

            try
            {
                var errorDetails = new
                {
                    ExceptionType = exception.GetType().Name,
                    Message = exception.Message,
                    StackTrace = context.Request.Host.Host.Contains("localhost") ? exception.StackTrace : null,
                    StatusCode = (int)statusCode,
                    IpAddress = ipAddress
                };

                await auditService.LogAsync(
                    action: "Exception",
                    entityType: ExtractEntityType(requestPath),
                    entityId: ExtractEntityId(requestPath),
                    details: errorDetails);
            }
            catch (Exception auditEx)
            {
                _logger.LogError(auditEx, "Error al registrar auditoría de excepción");
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var response = new
            {
                message,
                details = context.Request.Host.Host.Contains("localhost") ? exception.Message : null,
                stackTrace = context.Request.Host.Host.Contains("localhost") ? exception.StackTrace : null
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }

        private static string ExtractEntityType(string? requestPath)
        {
            if (string.IsNullOrWhiteSpace(requestPath))
                return "Unknown";

            var segments = requestPath.Split('/', StringSplitOptions.RemoveEmptyEntries);

            if (segments.Length == 0)
                return "Root";

            var entitySegment = segments.FirstOrDefault(s =>
                !s.Equals("api", StringComparison.OrdinalIgnoreCase)) ?? "Unknown";

            return entitySegment;
        }

        private static int? ExtractEntityId(string? requestPath)
        {
            if (string.IsNullOrWhiteSpace(requestPath))
                return null;

            var segments = requestPath.Split('/', StringSplitOptions.RemoveEmptyEntries);

            foreach (var segment in segments)
            {
                if (int.TryParse(segment, out var id))
                    return id;
            }

            return null;
        }
    }
}
