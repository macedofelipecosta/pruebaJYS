using LogicaNegocio.ValueObjects.LogicaNegocio.ValueObjects;

namespace LogicaNegocio.Dominio.Reservations
{
    public class ReservationParticipant
    {
        public int Id { get; private set; }
        public int ReservationId { get; private set; }
        public Reservation Reservation { get; private set; } = null!;
        public int UserId { get; private set; }
        public Email UserEmail { get; private set; } = null!;
        public bool IsOrganizer { get; private set; }
        public DateTime InvitationDate { get; private set; }
        public int ParticipantStatusId { get; private set; }
        public ParticipantStatus ParticipantStatus { get; private set; } = null!;//Pending, Accepted, Declined
        private ReservationParticipant()
        {
            //Para EF
        }
    }
}
