using LogicaNegocio.ValueObjects.LogicaNegocio.ValueObjects;

namespace LogicaNegocio.Dominio.Reservations
{
    public class ReservationParticipant
    {
        public int Id { get; private set; }
        public int ReservationId { get; private set; }
        public int UserId { get; private set; }
        public bool IsOrganizer { get; private set; }
        public DateTime InvitationDate { get; private set; }
        public int ParticipantStatusId { get; private set; } //El participante debía confirmar la reserva o solo se lo invitaba?
        public ParticipantStatus ParticipantStatus { get; private set; } = null!;//ToDo: Pending, Accepted, Declined ??Necesario 
     
        
        
        private ReservationParticipant()
        {
            //Para EF
        }
        public ReservationParticipant(int userId, bool isOrganizer)
        {
            UserId = userId;
            IsOrganizer = isOrganizer;
            InvitationDate = DateTime.UtcNow;
        }
    }
}
