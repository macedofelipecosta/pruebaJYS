using LogicaNegocio.InterfacesDominio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicaNegocio.Dominio.Notifications
{
    public class ReservationCreatedDomainEvent: IDomainEvent
    {
        public int ReservationId { get; }
        public int RoomId { get; }
        public int CreatedByUserId { get; }
        public DateTime StartDate { get; }
        public DateTime EndDate { get; }
        public string Subject { get; }
        public string Description { get; }
        public DateTime OccurredOn { get; }

        public ReservationCreatedDomainEvent(
            int reservationId,
            int roomId,
            int createdByUserId,
            DateTime startDate,
            DateTime endDate,
            string subject,
            string description)
        {
            ReservationId = reservationId;
            RoomId = roomId;
            CreatedByUserId = createdByUserId;
            StartDate = startDate;
            EndDate = endDate;
            Subject = subject;
            Description = description;
            OccurredOn = DateTime.UtcNow;
        }
    }
}
