using API_GestionDeSalas_Jaume_Sere.Helpers;
using LogicaAplicacion.ServiceInterfaces;
using System.Diagnostics;

namespace API_GestionDeSalas_Jaume_Sere.Middleware
{
    public class AuditMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<AuditMiddleware> _logger;

        public AuditMiddleware(RequestDelegate next, ILogger<AuditMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, IAuditService auditService)
        {
            var stopwatch = Stopwatch.StartNew();
            var requestPath = context.Request.Path.Value ?? string.Empty;
            var httpMethod = context.Request.Method;

            var userId = HttpContextUserHelper.GetUserId(context);
            var username = HttpContextUserHelper.GetUsername(context);
            var userRole = HttpContextUserHelper.GetUserRole(context);
            var ipAddress = HttpContextUserHelper.GetIpAddress(context);

            _logger.LogInformation(
                "REQUEST INICIO: {HttpMethod} {RequestPath} - Usuario: {Username} ({UserId}, {UserRole}) desde {IpAddress}",
                httpMethod, requestPath, username, userId, userRole, ipAddress);

            string requestBody = string.Empty;
            if (ShouldLogRequestBody(httpMethod))
            {
                requestBody = await ReadRequestBodyAsync(context.Request);
            }

            var originalResponseBodyStream = context.Response.Body;

            try
            {
                using var responseBody = new MemoryStream();
                context.Response.Body = responseBody;

                await _next(context);

                stopwatch.Stop();
                var statusCode = context.Response.StatusCode;

                string responseBodyContent = string.Empty;
                if (ShouldLogResponseBody(statusCode))
                {
                    responseBody.Seek(0, SeekOrigin.Begin);
                    responseBodyContent = await new StreamReader(responseBody).ReadToEndAsync();
                    responseBody.Seek(0, SeekOrigin.Begin);
                }

                // ensure we copy the buffered response back to the original stream
                responseBody.Seek(0, SeekOrigin.Begin);
                await responseBody.CopyToAsync(originalResponseBodyStream);

                // restore original response body so subsequent middleware and the server use it
                context.Response.Body = originalResponseBodyStream;

                _logger.LogInformation(
                    "REQUEST FIN: {HttpMethod} {RequestPath} - Status: {StatusCode} - Duración: {Duration}ms - Usuario: {Username} ({UserId})",
                    httpMethod, requestPath, statusCode, stopwatch.ElapsedMilliseconds, username, userId);

                await LogAuditAsync(
                    auditService,
                    httpMethod,
                    requestPath,
                    statusCode,
                    stopwatch.ElapsedMilliseconds,
                    requestBody,
                    responseBodyContent);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                context.Response.Body = originalResponseBodyStream;

                _logger.LogError(ex,
                    "REQUEST ERROR: {HttpMethod} {RequestPath} - Duración: {Duration}ms - Usuario: {Username} ({UserId}) - Error: {ErrorMessage}",
                    httpMethod, requestPath, stopwatch.ElapsedMilliseconds, username, userId, ex.Message);

                throw;
            }
        }

        private async Task LogAuditAsync(
            IAuditService auditService,
            string httpMethod,
            string requestPath,
            int statusCode,
            long durationMs,
            string requestBody,
            string responseBody)
        {
            try
            {
                var action = DetermineAction(httpMethod, requestPath, statusCode);
                var entityType = ExtractEntityType(requestPath);
                var entityId = ExtractEntityId(requestPath);

                var details = new
                {
                    StatusCode = statusCode,
                    DurationMs = durationMs,
                    RequestBody = ShouldIncludeRequestBody(requestPath) ? TruncateIfNeeded(requestBody, 2000) : null,
                    ResponseBody = statusCode >= 400 ? TruncateIfNeeded(responseBody, 1000) : null
                };

                await auditService.LogAsync(action, entityType, entityId, details);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al registrar auditoría para {HttpMethod} {RequestPath}", httpMethod, requestPath);
            }
        }

        private static string DetermineAction(string httpMethod, string requestPath, int statusCode)
        {
            if (statusCode >= 400)
                return $"{httpMethod}_Error";

            return httpMethod.ToUpperInvariant() switch
            {
                "GET" => "Read",
                "POST" => requestPath.Contains("/login", StringComparison.OrdinalIgnoreCase) ? "Login" : "Create",
                "PUT" => "Update",
                "PATCH" => "PartialUpdate",
                "DELETE" => "Delete",
                _ => httpMethod
            };
        }

        private static string ExtractEntityType(string requestPath)
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

        private static int? ExtractEntityId(string requestPath)
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

        private static bool ShouldLogRequestBody(string httpMethod)
        {
            return httpMethod.Equals("POST", StringComparison.OrdinalIgnoreCase) ||
                   httpMethod.Equals("PUT", StringComparison.OrdinalIgnoreCase) ||
                   httpMethod.Equals("PATCH", StringComparison.OrdinalIgnoreCase);
        }

        private static bool ShouldLogResponseBody(int statusCode)
        {
            return statusCode >= 400;
        }

        private static bool ShouldIncludeRequestBody(string requestPath)
        {
            return !requestPath.Contains("/login", StringComparison.OrdinalIgnoreCase) &&
                   !requestPath.Contains("/auth", StringComparison.OrdinalIgnoreCase) &&
                   !requestPath.Contains("/password", StringComparison.OrdinalIgnoreCase);
        }

        private static async Task<string> ReadRequestBodyAsync(HttpRequest request)
        {
            try
            {
                request.EnableBuffering();
                var buffer = new byte[Convert.ToInt32(request.ContentLength ?? 0)];
                await request.Body.ReadAsync(buffer, 0, buffer.Length);
                var bodyAsText = System.Text.Encoding.UTF8.GetString(buffer);
                request.Body.Position = 0;
                return bodyAsText;
            }
            catch
            {
                return string.Empty;
            }
        }

        private static string TruncateIfNeeded(string text, int maxLength)
        {
            if (string.IsNullOrEmpty(text) || text.Length <= maxLength)
                return text;

            return text.Substring(0, maxLength) + "... [truncated]";
        }
    }
}
