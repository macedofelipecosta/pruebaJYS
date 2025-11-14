using LogicaNegocio.Dominio.Enums;

namespace LogicaNegocio.Dominio.Notifications
{
    public class Notification
    {
        public int Id { get; private set; }
        public NotificationType Type { get; private set; }   // Email, Teams, etc.
        public int UserId { get; private set; }              // o Email directamente si no tenés UserId
        public string Subject { get; private set; } = string.Empty;
        public string Message { get; private set; } = string.Empty;
        public DateTime SentAt { get; private set; }
        public bool Success { get; private set; }
        public string ErrorDetail { get; private set; } = string.Empty;



        // EF
        private Notification() { }

        private Notification(NotificationType type, int userId, string subject, string message)
        {
            Type = type;
            UserId = userId;
            Subject = subject;
            Message = message;
        }
        public static Notification Create(NotificationType type, int userId, string subject, string message)
            => new Notification(type, userId, subject, message);

        public void MarkAsSent()
        {
            Success = true;
            SentAt = DateTime.UtcNow;
            ErrorDetail = string.Empty;
        }

        public void MarkAsFailed(string error)
        {
            Success = false;
            SentAt = DateTime.UtcNow;
            ErrorDetail = error;
        }

    }
}
