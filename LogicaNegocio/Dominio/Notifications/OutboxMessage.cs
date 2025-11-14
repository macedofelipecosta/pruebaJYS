using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicaNegocio.Dominio.Notifications
{
    public class OutboxMessage
    {
        public Guid Id { get; private set; }
        public DateTime OccurredOn { get;  set; }
        public string Type { get;  set; } = string.Empty;      // nombre del evento
        public string Payload { get;  set; } = string.Empty;   // JSON del evento
        public DateTime? ProcessedOn { get; private set; }
        public string? Error { get; private set; }

        public OutboxMessage() { }

        public OutboxMessage(string type, string payload, DateTime occurredOn)
        {
            Id = Guid.NewGuid();
            Type = type;
            Payload = payload;
            OccurredOn = occurredOn;
        }

        public void MarkProcessed()
        {
            ProcessedOn = DateTime.UtcNow;
            Error = null;
        }

        public void MarkFailed(string error)
        {
            Error = error;
            // ProcessedOn queda null para reintento
        }
    }
}
