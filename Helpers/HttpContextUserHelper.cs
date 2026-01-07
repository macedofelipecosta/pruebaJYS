namespace API_GestionDeSalas_Jaume_Sere.Helpers
{
    public static class HttpContextUserHelper
    {
        public static int? GetUserId(HttpContext? context)
        {
            var userIdClaim = context?.User?.FindFirst("userId")?.Value
                           ?? context?.User?.FindFirst("sub")?.Value
                           ?? context?.User?.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;

            return int.TryParse(userIdClaim, out var userId) ? userId : null;
        }

        public static string GetUsername(HttpContext? context)
        {
            return context?.User?.FindFirst("username")?.Value
                ?? context?.User?.FindFirst("name")?.Value
                ?? context?.User?.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")?.Value
                ?? context?.User?.Identity?.Name
                ?? "Anonymous";
        }

        public static string GetUserRole(HttpContext? context)
        {
            return context?.User?.FindFirst("role")?.Value
                ?? context?.User?.FindFirst("http://schemas.microsoft.com/ws/2008/06/identity/claims/role")?.Value
                ?? "Unknown";
        }

        public static string GetIpAddress(HttpContext? context)
        {
            if (context == null) return string.Empty;

            var ipAddress = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (string.IsNullOrEmpty(ipAddress))
            {
                ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? string.Empty;
            }

            return ipAddress;
        }
    }
}
