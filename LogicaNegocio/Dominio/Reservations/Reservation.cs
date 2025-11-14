using LogicaNegocio.Dominio.Notifications;
using LogicaNegocio.Dominio.Rooms;
using LogicaNegocio.InterfacesDominio;

namespace LogicaNegocio.Dominio.Reservations
{
    public class Reservation : Entity, IValidable
    {
        public int Id { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        public int RoomId { get; private set; }
        public Room Room { get; private set; } = null!;
        public int ReservationStatusId { get; private set; }
        public ReservationStatus Status { get; private set; } = null!; // Pending, Confirmed, Cancelled
        public int CreatedByUserId { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public string Subject { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;
        public int OutlookEventId { get; private set; }
        public DateTime CancelledAt { get; private set; }
        public TimeSpan Duration => EndDate - StartDate;
        public bool Extended { get; private set; }
        public DateTime? ExtendedUntil { get; private set; }



        private Reservation()
        {
        }

        public static Reservation Create(
        int roomId,
        int createdByUserId,
        DateTime startDate,
        DateTime endDate,
        string subject,
        string description)
        {
            var reservation = new Reservation
            {
                RoomId = roomId,
                CreatedByUserId = createdByUserId,
                StartDate = startDate,
                EndDate = endDate,
                Subject = subject,
                Description = description,
                CreatedAt = DateTime.UtcNow,
                // acá podria setear Status = Pending, etc
            };

            reservation.Validate();

            // Disparamos evento de dominio
            reservation.AddDomainEvent(
                new ReservationCreatedDomainEvent(
                    reservation.Id,          // OJO: se seteara al guardar, se puede usar luego en handler si lo necesitas
                    reservation.RoomId,
                    reservation.CreatedByUserId,
                    reservation.StartDate,
                    reservation.EndDate,
                    reservation.Subject,
                    reservation.Description
                ));

            return reservation;
        }


        public void Validate()
        {
            if (StartDate >= EndDate)
            {
                throw new InvalidOperationException("La fecha de inicio debe ser anterior a la fecha de fin.");
            }
            if (string.IsNullOrWhiteSpace(Subject))
            {
                throw new InvalidOperationException("El asunto no puede estar vacío.");
            }
            if (!string.IsNullOrWhiteSpace(Subject))
            {
                if (Subject.Length > 100)
                {
                    throw new InvalidOperationException("El asunto no puede exceder los 100 caracteres.");
                }
            }
            if (Room == null)
            {
                throw new InvalidOperationException("La sala no puede ser nula.");
            }
            if (RoomId == 0)
            {
                throw new InvalidOperationException("El Id de la sala no puede ser 0.");
            }
            if (Status != null && !Status.Name.ToLower().Equals("pending") && !Status.Name.ToLower().Equals("confirmed") && !Status.Name.ToLower().Equals("cancelled"))
            {
                throw new InvalidOperationException("El estado de la reserva es inválido.");
            }
            if (CreatedByUserId == 0)
            {
                throw new InvalidOperationException("El Id del usuario que creó la reserva no puede ser 0.");
            }
            if (Extended && (!ExtendedUntil.HasValue || ExtendedUntil <= EndDate))
            {
                throw new InvalidOperationException("La fecha hasta la cual se extiende la reserva es inválida.");
            }
            if (CreatedAt > StartDate)
            {
                throw new InvalidOperationException("La fecha de creación no puede ser posterior a la fecha de inicio.");
            }
            if (Status != null && !Status.Name.ToLower().Equals("cancelled") && CancelledAt != default)
            {
                throw new InvalidOperationException("La fecha de cancelación debe estar establecida si la reserva está cancelada.");
            }
            if (OutlookEventId <= 0)
            {
                throw new InvalidOperationException("El Id del evento de Outlook no puede ser negativo o 0.");
            }
        }
    }
}
